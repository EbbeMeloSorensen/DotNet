using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Craft.Logging;
using Craft.Utils.Linq;
using Craft.DataStructures.Graph;
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
        private bool _currentPlayerMayReinforce;
        private bool _currentPlayerMayTransferArmies;

        // An array with a boolean for each player. A boolean with a value of true indicates that the given player is a computer player
        private bool[] _players;

        private Random _random;

        public int PlayerCount => PlayerScores.Length;

        public int CurrentPlayerIndex { get; private set; }

        public int[] PlayerScores { get; }

        public bool GameInProgress { get; private set; }

        public bool GameDecided { get; private set; }

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
            PlayerScores = new int[playerCount];
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
            _currentPlayerMayReinforce = true;
            _currentPlayerMayTransferArmies = true;

            Logger?.WriteLine(LogMessageCategory.Information, $"New Game Started - Player {CurrentPlayerIndex + 1} begins");
        }

        public async Task<IGameEvent> ExecuteNextEvent()
        {
            await Task.Delay(1);

            var options = IdentifyOptionsForCurrentPlayer();

            if (options.Count > 0)
            {
                var bestOption = options.OrderByDescending(_ => _.OpportunityRating).First();

                if (bestOption.OpportunityRating > 0)
                {
                    return Attack(
                        bestOption.IndexOfTerritoryWhereAttackOriginates,
                        bestOption.IndexOfTerritoryUnderAttack);
                }
            }

            if (_currentPlayerMayReinforce &&
                _territoryStatusMap.Any(_ => _.Value.ControllingPlayerIndex == CurrentPlayerIndex))
            {
                return Reinforce();
            }

            if (_currentPlayerMayTransferArmies)
            {
                return TransferArmies();
            }

            return Pass();
        }

        public async Task<IGameEvent> PlayerSelectsOption(
            IPlayerOption option)
        {
            await Task.Delay(1);

            switch (option)
            {
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

            _currentPlayerMayReinforce = false;

            return gameEvent;
        }

        private IGameEvent Pass()
        {
            var gameEvent = new PlayerPasses(
                CurrentPlayerIndex);

            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Length;
            _currentPlayerMayReinforce = true;
            _currentPlayerMayTransferArmies = true;

            return gameEvent;
        }

        private IGameEvent Reinforce()
        {
            // Tactic: Distribute the armies randomly on the frontline territories with the fewest armies

            var indexesOfTerritoriesControlledByCurrentPlayer = _territoryStatusMap
                .Where(_ => _.Value.ControllingPlayerIndex == CurrentPlayerIndex)
                .Select(_ => _.Key)
                .ToList();

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

            var territoryCount = indexesOfTerritoriesControlledByCurrentPlayer.Count();
            var extraArmies = Math.Max(3, territoryCount / 3);
            var territoryIndexes = new List<int>();

            Enumerable.Repeat(0, extraArmies)
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

            var reinforcedTerritoryIndexesAsCSV  = territoryIndexes
                .Select(_ => _.ToString()).Aggregate((c, n) => $"{c}, {n}");

            var gameEvent = new PlayerReinforces(
                CurrentPlayerIndex)
            {
                TerritoryIndexes = territoryIndexes
            };

            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Length;
            _currentPlayerMayReinforce = true; // This goes for the next player
            _currentPlayerMayTransferArmies = true; // This goes for the next player

            return gameEvent;
        }

        private IGameEvent TransferArmies()
        {
            var gameEvent = new PlayerTransfersArmies(
                CurrentPlayerIndex);

            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Length;
            _currentPlayerMayReinforce = true; // This goes for the next player

            _currentPlayerMayTransferArmies = true; // This goes for the next player
            return gameEvent;
        }

        private List<AttackOption> IdentifyOptionsForCurrentPlayer()
        {
            // Which vertices are controlled by the current player?
            var indexesOfTerritoriesControlledByCurrentPlayer = _territoryStatusMap
                .Where(_ => _.Value.ControllingPlayerIndex == CurrentPlayerIndex)
                .Select(_ => _.Key)
                .ToList();

            // Traverse all those vertices and identify neighbours controlled by other players
            var options = new List<AttackOption>();

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
    }
}
