using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Craft.Logging;
using Craft.Utils.Linq;
using Craft.DataStructures.Graph;
using Games.Risk.Application.ComputerPlayerOptions;
using Games.Risk.Application.GameEvents;
using Games.Risk.Application.PlayerOptions;

namespace Games.Risk.Application
{
    public class Engine
    {
        private const int _targetScore = 100;
        private const int _dieFaces = 6;
        private IGraph<LabelledVertex, EmptyEdge> _graphOfTerritories;
        private Dictionary<int, TerritoryStatus> _territoryStatusMap;
        private bool _pseudoRandomNumbers;
        private bool _currentPlayerHasReinforced;
        private bool _currentPlayerMayTransferArmies;
        private List<Continent> _continents;

        // An array with a boolean for each player. A boolean with a value of true indicates that the given player is a computer player
        private bool[] _players;

        private Random _random;

        public int PlayerCount => _players.Length;

        public int CurrentPlayerIndex { get; private set; }

        public bool GameInProgress { get; private set; }

        public bool GameDecided { get; private set; }

        public int ExtraArmiesForCurrentPlayer { get; private set; }

        public bool CurrentPlayerMayReinforce { get; private set; }

        public bool NextEventOccursAutomatically
        {
            get => _players[CurrentPlayerIndex];
        }

        public ILogger Logger { get; set; }

        public Engine(
            bool[] players,
            bool pseudoRandomNumbers,
            IGraph<LabelledVertex, EmptyEdge> graphOfTerritories)
        {
            _pseudoRandomNumbers = pseudoRandomNumbers;
            _graphOfTerritories = graphOfTerritories;
            var playerCount = players.Count();

            if (playerCount < 2 || playerCount > 6)
            {
                throw new ArgumentOutOfRangeException("Invalid number of players");
            }

            _random = pseudoRandomNumbers
                ? new Random(0)
                : new Random((int)DateTime.UtcNow.Ticks);

            _players = players;
            _continents = new List<Continent>();
        }

        public void Initialize(
            IEnumerable<Continent> continents)
        {
            _continents.AddRange(continents);
        }

        public void StartGame()
        {
            // Distribute territories among players
            var vertexIds = _graphOfTerritories.Vertices
                .Select(_ => _.Id)
                .ToList();

            vertexIds = vertexIds.Shuffle(_random).ToList();

            _territoryStatusMap = new Dictionary<int, TerritoryStatus>();

            var playerId = 0;
            foreach (var vertexId in vertexIds)
            {
                _territoryStatusMap[vertexId] = new TerritoryStatus
                {
                    ControllingPlayerIndex = playerId,
                    Armies = 3
                };
                playerId = (playerId + 1) % PlayerCount;
            }

            GameInProgress = true;
            CurrentPlayerIndex = 0;
            CurrentPlayerMayReinforce = true;
            _currentPlayerMayTransferArmies = true;

            Logger?.WriteLine(LogMessageCategory.Information,
                $"New Game Started - Player {CurrentPlayerIndex + 1} begins");
        }

        public async Task<IGameEvent> ExecuteNextEvent()
        {
            await Task.Delay(1);

            if (ExtraArmiesForCurrentPlayer > 0)
            {
                if (!_currentPlayerHasReinforced)
                {
                    return DeployArmies(false);
                }

                _currentPlayerHasReinforced = false;
                return DeployArmies(true);
            }

            var attackOptions = IdentifyAttackOptionsForCurrentPlayer();

            if (attackOptions.Any())
            {
                attackOptions = attackOptions.OrderByDescending(_ => _.OpportunityRating).ToList();
                var highestOpportunityRating = attackOptions.First().OpportunityRating;

                var bestAttackOptions = attackOptions
                    .TakeWhile(_ => _.OpportunityRating == highestOpportunityRating);

                var chosenCandidate = bestAttackOptions.Shuffle(_random).First();

                if (chosenCandidate.OpportunityRating > 0)
                {
                    return Attack(
                        chosenCandidate.IndexOfTerritoryWhereAttackOriginates,
                        chosenCandidate.IndexOfTerritoryUnderAttack);
                }
            }

            if (CurrentPlayerMayReinforce &&
                _territoryStatusMap.Any(_ => _.Value.ControllingPlayerIndex == CurrentPlayerIndex))
            {
                return Reinforce();
            }

            if (_currentPlayerMayTransferArmies)
            {
                var armyTransferOptions = IdentifyArmyTransferOptionsForCurrentPlayer();

                if (armyTransferOptions.Any())
                {
                    armyTransferOptions = armyTransferOptions.OrderByDescending(_ => _.OpportunityRating).ToList();
                    var highestOpportunityRating = armyTransferOptions.First().OpportunityRating;

                    var bestArmyTransferOptions = armyTransferOptions
                        .TakeWhile(_ => _.OpportunityRating == highestOpportunityRating);

                    var chosenOption = bestArmyTransferOptions.Shuffle(_random).First();

                    return TransferArmies(
                        chosenOption.InitialTerritoryIndex,
                        chosenOption.DestinationTerritoryIndex,
                        _territoryStatusMap[chosenOption.InitialTerritoryIndex].Armies - 1);
                }
            }

            return Pass();
        }

        public async Task<IGameEvent> PlayerSelectsOption(
            IPlayerOption option)
        {
            await Task.Delay(1);

            switch (option)
            {
                case Deploy deploy:
                {
                    return DeployArmy(deploy.ActiveTerritoryIndex);
                }
                case Reinforce _:
                {
                    return Reinforce();
                }
                case Attack attack:
                {
                    return Attack(
                        attack.ActiveTerritoryIndex,
                        attack.TargetTerritoryIndex);
                }
                case Pass _:
                {
                    return Pass();
                }
                default:
                {
                    throw new InvalidOperationException("Invalid player option");
                }
            }
        }

        public TerritoryStatus GetTerritoryStatus(
            int territoryId)
        {
            return _territoryStatusMap[territoryId];
        }

        public IEnumerable<int> IndexesOfHostileNeighbourTerritories(
            int territoryId)
        {
            return _graphOfTerritories.NeighborIds(territoryId)
                .Except(IndexesOfControlledTerritories(CurrentPlayerIndex));
        }

        public List<string> AssignExtraArmiesForControlledContinents()
        {
            var result = new List<string>();

            var controlledTerritories = IndexesOfControlledTerritories(CurrentPlayerIndex).ToList();

            _continents.ForEach(c =>
            {
                if (controlledTerritories.Intersect(c.Territories).Count() != c.Territories.Length)
                {
                    return;
                }

                result.Add(c.Name);

                ExtraArmiesForCurrentPlayer += c.ExtraArmies;
            });

            return result;
        }

        private IEnumerable<int> IndexesOfControlledTerritories(
            int playerId)
        {
            return _territoryStatusMap
                .Where(_ => _.Value.ControllingPlayerIndex == playerId)
                .Select(_ => _.Key);
        }

        private IGameEvent Attack(
            int activeTerritoryIndex,
            int targetTerritoryIndex)
        {
            var armyCountAttacker = _territoryStatusMap[activeTerritoryIndex].Armies;
            var armyCountDefender = _territoryStatusMap[targetTerritoryIndex].Armies;

            var diceCountAttacker = Math.Min(armyCountAttacker - 1, 3);
            var diceCountDefender = Math.Min(armyCountDefender, 2);

            var diceRollsAttacker = Enumerable
                .Repeat(0, diceCountAttacker)
                .Select(_ => _random.Next(0, 7))
                .OrderByDescending(_ => _)
                .ToList();

            var diceRollsDefender = Enumerable
                .Repeat(0, diceCountDefender)
                .Select(_ => _random.Next(0, 7))
                .OrderByDescending(_ => _)
                .ToList();

            var comparisons = Math.Min(diceCountAttacker, diceCountDefender);

            diceRollsAttacker = diceRollsAttacker.Take(comparisons).ToList();
            diceRollsDefender = diceRollsDefender.Take(comparisons).ToList();

            var casualtiesDefender = diceRollsAttacker
                .Zip(diceRollsDefender, (a, d) => a > d)
                .Count(_ => _);

            var casualtiesAttacker = comparisons - casualtiesDefender;

            _territoryStatusMap[activeTerritoryIndex].Armies -= casualtiesAttacker;
            _territoryStatusMap[targetTerritoryIndex].Armies -= casualtiesDefender;

            var territoryConquered = false;
            var defendingPlayerIndex = _territoryStatusMap[targetTerritoryIndex].ControllingPlayerIndex;

            if (_territoryStatusMap[targetTerritoryIndex].Armies == 0)
            {
                territoryConquered = true;
                _territoryStatusMap[targetTerritoryIndex].ControllingPlayerIndex = CurrentPlayerIndex;

                var armiesLeft = _territoryStatusMap[activeTerritoryIndex].Armies;

                // Did we manage to establish an isolated territory?
                var isolatedTerritoryEstablished = _graphOfTerritories.NeighborIds(activeTerritoryIndex)
                    .All(neighborId => _territoryStatusMap[neighborId].ControllingPlayerIndex == CurrentPlayerIndex);

                var armyTransferCount = isolatedTerritoryEstablished
                    ? armiesLeft - 1
                    : (armiesLeft + 1) / 2;

                _territoryStatusMap[targetTerritoryIndex].Armies = armyTransferCount;
                _territoryStatusMap[activeTerritoryIndex].Armies = armiesLeft - armyTransferCount;

                if (_territoryStatusMap.All(_ => _.Value.ControllingPlayerIndex == CurrentPlayerIndex))
                {
                    GameDecided = true;
                    GameInProgress = false;
                }
            }

            var gameEvent = new PlayerAttacks(
                CurrentPlayerIndex)
            {
                Vertex1 = activeTerritoryIndex,
                Vertex2 = targetTerritoryIndex,
                DefendingPlayerIndex = defendingPlayerIndex,
                CasualtiesAttacker = casualtiesAttacker,
                CasualtiesDefender = casualtiesDefender,
                TerritoryConquered = territoryConquered
            };

            CurrentPlayerMayReinforce = false;

            return gameEvent;
        }

        private IGameEvent Pass()
        {
            var gameEvent = new PlayerPasses(
                CurrentPlayerIndex);

            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Length;
            CurrentPlayerMayReinforce = true;
            _currentPlayerMayTransferArmies = true;

            return gameEvent;
        }

        private IGameEvent Reinforce()
        {
            var territoryCount =
                IndexesOfControlledTerritories(CurrentPlayerIndex).Count();

            ExtraArmiesForCurrentPlayer = Math.Max(3, territoryCount / 3);
            _currentPlayerHasReinforced = true;
            CurrentPlayerMayReinforce = false;

            return new PlayerReinforces(CurrentPlayerIndex, false);
        }

        private IGameEvent DeployArmy(
            int territoryIndex)
        {
            ExtraArmiesForCurrentPlayer--;
            _territoryStatusMap[territoryIndex].Armies++;

            return new PlayerDeploysArmies(
                CurrentPlayerIndex,
                false)
            {
                TerritoryIndexes = new List<int>{ territoryIndex }
            };
        }

        private IGameEvent DeployArmies(
            bool turnGoesToNextPlayer)
        {
            // Tactic: Distribute the armies randomly on the frontline territories with the fewest armies

            var indexesOfTerritoriesControlledByCurrentPlayer =
                IndexesOfControlledTerritories(CurrentPlayerIndex).ToList();

            var indexesOfFrontlineTerritories = new List<int>();

            indexesOfTerritoriesControlledByCurrentPlayer.ForEach(vertexId =>
            {
                var hostileNeighbourIndexes = _graphOfTerritories.NeighborIds(vertexId)
                    .Except(indexesOfTerritoriesControlledByCurrentPlayer)
                    .ToList();

                if (hostileNeighbourIndexes.Count > 0)
                {
                    indexesOfFrontlineTerritories.Add(vertexId);
                }
            });

            var territoryIndexes = new List<int>();

            Enumerable.Repeat(0, ExtraArmiesForCurrentPlayer)
                .ToList()
                .ForEach(_ =>
                {
                    var temp1 = _territoryStatusMap
                        .Where(_ => indexesOfFrontlineTerritories.Contains(_.Key))
                        .OrderBy(_ => _.Value.Armies)
                        .ToList();

                    var minNumberOfArmies = temp1.Min(_ => _.Value.Armies);

                    var temp2 = temp1
                        .TakeWhile(_ => _.Value.Armies == minNumberOfArmies)
                        .ToList();

                    var territoryIndex = temp2.Shuffle(_random).First().Key;

                    _territoryStatusMap[territoryIndex].Armies += 1;

                    territoryIndexes.Add(territoryIndex);
                });

            ExtraArmiesForCurrentPlayer = 0;

            var reinforcedTerritoryIndexesAsCSV = territoryIndexes
                .Select(_ => _.ToString()).Aggregate((c, n) => $"{c}, {n}");

            var gameEvent = new PlayerDeploysArmies(
                CurrentPlayerIndex,
                turnGoesToNextPlayer)
            {
                TerritoryIndexes = territoryIndexes
            };

            if (turnGoesToNextPlayer)
            {
                CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Length;
                CurrentPlayerMayReinforce = true; // This goes for the next player
                _currentPlayerMayTransferArmies = true; // This goes for the next player
            }

            return gameEvent;
        }

        private IGameEvent TransferArmies(
            int initialTerritoryIndex,
            int destinationTerritoryIndex,
            int armiesTransfered)
        {
            var gameEvent = new PlayerTransfersArmies(
                CurrentPlayerIndex)
            {
                Vertex1 = initialTerritoryIndex,
                Vertex2 = destinationTerritoryIndex,
                ArmiesTransfered = armiesTransfered
            };

            _territoryStatusMap[initialTerritoryIndex].Armies -= armiesTransfered;
            _territoryStatusMap[destinationTerritoryIndex].Armies += armiesTransfered;

            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Length;
            CurrentPlayerMayReinforce = true; // This goes for the next player
            _currentPlayerMayTransferArmies = true; // This goes for the next player
            return gameEvent;
        }

        private List<AttackOption> IdentifyAttackOptionsForCurrentPlayer()
        {
            var options = new List<AttackOption>();

            var indexesOfTerritoriesControlledByCurrentPlayer =
                IndexesOfControlledTerritories(CurrentPlayerIndex).ToList();

            // Traverse all those vertices and identify neighbours controlled by other players
            indexesOfTerritoriesControlledByCurrentPlayer.ForEach(vertexIndex =>
            {
                if (_territoryStatusMap[vertexIndex].Armies == 1)
                {
                    // Cannot attack from territory unless it has more than one army
                    return;
                }

                var armiesInTerritory = _territoryStatusMap[vertexIndex].Armies;

                var opposingNeighbourIds = _graphOfTerritories.NeighborIds(vertexIndex)
                    .Except(indexesOfTerritoriesControlledByCurrentPlayer);

                var opposingNeighbourCount = opposingNeighbourIds.Count();

                foreach (var neighbourId in opposingNeighbourIds)
                {
                    var armiesInOpposingTerritory = _territoryStatusMap[neighbourId].Armies;
                    var opportunityRating = armiesInTerritory - armiesInOpposingTerritory;

                    if (opportunityRating > 0 && opposingNeighbourCount == 1)
                    {
                        opportunityRating += 2;
                    }

                    options.Add(new AttackOption
                    {
                        IndexOfTerritoryWhereAttackOriginates = vertexIndex,
                        IndexOfTerritoryUnderAttack = neighbourId,
                        OpportunityRating = opportunityRating
                    });
                }
            });

            return options;
        }

        private List<ArmyTransferOption> IdentifyArmyTransferOptionsForCurrentPlayer()
        {
            var options = new List<ArmyTransferOption>();

            var connectedComponents = IdentifyConnectedTerritories();

            if (!connectedComponents.ContainsKey(CurrentPlayerIndex))
            {
                return options;
            }

            // Identify isolated territories (non-frontline) territories controlled by the current player
            var territoryIndexes =  IndexesOfControlledTerritories(CurrentPlayerIndex).ToList();
            var frontlineTerritories = new List<int>();

            territoryIndexes.ForEach(index =>
            {
                if (_graphOfTerritories.NeighborIds(index)
                    .Any(neighborId => _territoryStatusMap[neighborId].ControllingPlayerIndex != CurrentPlayerIndex))
                {
                    frontlineTerritories.Add(index);
                }
            });

            var isolatedTerritories = territoryIndexes.Except(frontlineTerritories).ToList();

            // Identify frontline territories guarding isolated territories
            var guardianTerritories = new List<int>();

            frontlineTerritories.ForEach(index =>
            {
                if (_graphOfTerritories.NeighborIds(index)
                    .Any(neighborId => isolatedTerritories.Contains(neighborId)))
                {
                    guardianTerritories.Add(index);
                }
            });

            connectedComponents[CurrentPlayerIndex]
                .Where(_ => _.Count > 2)
                .ToList()
                .ForEach(cc =>
                {
                    cc.ForEach(territoryIndex1 =>
                    {
                        var armiesInInitialTerritory = _territoryStatusMap[territoryIndex1].Armies;
                        
                        if (armiesInInitialTerritory == 1)
                        {
                            return;
                        }

                        if (!isolatedTerritories.Contains(territoryIndex1))
                        {
                            return;
                        }

                        cc.ForEach(territoryIndex2 =>
                        {
                            if (territoryIndex1 == territoryIndex2)
                            {
                                return;
                            }

                            if (!guardianTerritories.Contains(territoryIndex2))
                            {
                                return;
                            }

                            var armiesInDestinationTerritory = _territoryStatusMap[territoryIndex2].Armies;
                            var opportunityRating = armiesInInitialTerritory - armiesInDestinationTerritory;

                            options.Add(new ArmyTransferOption
                            {
                                InitialTerritoryIndex = territoryIndex1,
                                DestinationTerritoryIndex = territoryIndex2,
                                OpportunityRating = opportunityRating
                            });
                        });
                    });
                });

            return options;
        }

        private Dictionary<int, List<List<int>>> IdentifyConnectedTerritories()
        {
            var result = new Dictionary<int, List<List<int>>>();

            var handled = new HashSet<int>();

            var vertexIds = _graphOfTerritories.Vertices
                .Select(vertex => vertex.Id)
                .ToList();

            vertexIds.ForEach(vertexIdInGraph =>
            {
                if (handled.Contains(vertexIdInGraph))
                {
                    return;
                }

                var controllingPlayerIndex =
                    _territoryStatusMap[vertexIdInGraph].ControllingPlayerIndex;

                var connectedComponent = new HashSet<int>();
                var queue = new Queue<int>();
                queue.Enqueue(vertexIdInGraph);

                while (queue.Any())
                {
                    var vertexIdInComponent = queue.Dequeue();
                    connectedComponent.Add(vertexIdInComponent);
                    handled.Add(vertexIdInComponent);

                    var neighbours = _graphOfTerritories
                        .NeighborIds(vertexIdInComponent).ToList();

                    neighbours.ForEach(neighbourId =>
                    {
                        if (connectedComponent.Contains(neighbourId) ||
                            queue.Contains(neighbourId) ||
                            _territoryStatusMap[neighbourId].ControllingPlayerIndex != controllingPlayerIndex)
                        {
                            return;
                        }

                        queue.Enqueue(neighbourId);
                    });
                }

                if (!result.ContainsKey(controllingPlayerIndex))
                {
                    result[controllingPlayerIndex] = new List<List<int>>();
                }

                result[controllingPlayerIndex].Add(connectedComponent.ToList());
            });

            return result;
        }
    }
}