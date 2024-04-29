using System;
using System.Linq;
using System.Collections.Generic;
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
        private IGraph<LabelledVertex, EmptyEdge> _graphOfTerritories;
        private Dictionary<int, TerritoryStatus> _territoryStatusMap;
        private bool _currentPlayerMayTransferArmies;
        private List<Continent> _continents;
        private bool _territoryWasJustConquered;
        private bool _currentPlayerHasConqueredATerritory;
        private int _conqueringTerritoryId;
        private int _conqueredTerritoryId;
        private int _armiesInFinalAttack;
        private List<Card>[] _hands;
        private List<Card> _drawPile;
        private int _cardSetsTradedForTroops;

        // An array with a boolean for each player. A boolean with a value of true indicates that the given player is a computer player
        private bool[] _players;

        private Random _random;

        public int PlayerCount => _players.Length;

        public int CurrentPlayerIndex { get; private set; }

        public bool GameInProgress { get; private set; }

        public bool GameDecided { get; private set; }

        public int ExtraArmiesForCurrentPlayer { get; private set; }

        public bool CurrentPlayerMayReinforce { get; private set; }

        public bool CurrentPlayerHasReinforced { get; private set; }

        public bool CurrentPlayerHasMovedTroops { get; private set; }

        public bool NextEventOccursAutomatically
        {
            get => _players[CurrentPlayerIndex];
        }

        public ILogger Logger { get; set; }

        public Engine(
            bool[] players,
            Random random,
            IGraph<LabelledVertex, EmptyEdge> graphOfTerritories)
        {
            _graphOfTerritories = graphOfTerritories;
            var playerCount = players.Count();

            if (playerCount < 2 || playerCount > 6)
            {
                throw new ArgumentOutOfRangeException("Invalid number of players");
            }

            _random = random;
            _players = players;
            _continents = new List<Continent>();
            _hands = Enumerable.Repeat(0, playerCount).Select(_ => new List<Card>()).ToArray();
            _drawPile = GenerateCards().Shuffle(_random).ToList();
            _cardSetsTradedForTroops = 0;
        }

        public void Initialize(
            IEnumerable<Continent> continents)
        {
            _continents.AddRange(continents);
        }

        public void StartGame()
        {
            DistributeTerritoriesAmongPlayers();
            //DistributeTerritoriesAmongPlayers2();

            GameInProgress = true;
            CurrentPlayerIndex = 0;
            CurrentPlayerMayReinforce = true;
            _currentPlayerMayTransferArmies = true;
            _currentPlayerHasConqueredATerritory = false;

            Logger?.WriteLine(LogMessageCategory.Information,
                $"New Game Started - Player {CurrentPlayerIndex + 1} begins");
        }

        public async Task<IGameEvent> ExecuteNextEvent()
        {
            await Task.Delay(1);

            if (_territoryWasJustConquered)
            {
                _territoryWasJustConquered = false;

                var armiesLeftInConqueringTerritory = _territoryStatusMap[_conqueringTerritoryId].Armies;

                if (armiesLeftInConqueringTerritory - 1 == _armiesInFinalAttack)
                {
                    // Transfer all armies except one to the conquered territory (since all armies participating in the last attack must be conquered)
                    return TransferArmies(
                        _conqueringTerritoryId,
                        _conqueredTerritoryId,
                        _armiesInFinalAttack,
                        false,
                        true);
                }

                // If leaving behind an isolated territory, then transfer as many troops as possible. Otherwise transfer half, rounded up
                var isolatedTerritoryEstablished = _graphOfTerritories.NeighborIds(_conqueringTerritoryId)
                    .All(neighborId => _territoryStatusMap[neighborId].ControllingPlayerIndex == CurrentPlayerIndex);

                var armyTransferCount = isolatedTerritoryEstablished
                    ? armiesLeftInConqueringTerritory - 1
                    : (armiesLeftInConqueringTerritory + 1) / 2;

                return TransferArmies(
                    _conqueringTerritoryId,
                    _conqueredTerritoryId,
                    armyTransferCount,
                    false,
                    true);
            }

            if (ExtraArmiesForCurrentPlayer > 0)
            {
                return DeployArmies(CurrentPlayerHasReinforced);
            }

            var attackOptions = IdentifyAttackOptionsForCurrentPlayer();

            if (attackOptions.Any())
            {
                attackOptions = attackOptions.OrderByDescending(_ => _.OpportunityRating).ToList();
                var highestOpportunityRating = attackOptions.First().OpportunityRating;

                var bestAttackOptions = attackOptions
                    .TakeWhile(_ => _.OpportunityRating == highestOpportunityRating);

                var chosenCandidate = bestAttackOptions.Shuffle(_random).First();

                if (chosenCandidate.OpportunityRating > 1)
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
                        _territoryStatusMap[chosenOption.InitialTerritoryIndex].Armies - 1,
                        true,
                        false);
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
                case Reinforce _:
                {
                    return Reinforce();
                }
                case Deploy deploy:
                {
                    return DeployArmies(deploy.ActiveTerritoryIndex, deploy.Armies);
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

        public List<Card> GetHand(
            int playerIndex)
        {
            return _hands[playerIndex];
        }

        public IEnumerable<int> IndexesOfHostileNeighbourTerritories(
            int territoryId)
        {
            return _graphOfTerritories.NeighborIds(territoryId)
                .Except(IndexesOfControlledTerritories(CurrentPlayerIndex));
        }

        public IEnumerable<int> IndexesOfReachableTerritories(
            int territoryId)
        {
            var handled = new HashSet<int>();
            var reachable = GetConnectedComponent(territoryId, CurrentPlayerIndex, handled);
            reachable.Remove(territoryId);
            return reachable;
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

        public void AssignExtraArmiesForCards(
            out int extraArmiesForControlledTerritories,
            out int extraArmiesInTotalForCards)
        {
            extraArmiesForControlledTerritories = 0;
            extraArmiesInTotalForCards = 0;

            var hand = _hands[CurrentPlayerIndex];

            if (hand.Count < 3)
            {
                return;
            }

            var cards = new List<Card>();

            if (hand.Count(_ => _.Type == CardType.Cannon) > 0 &&
                hand.Count(_ => _.Type == CardType.Horse) > 0 &&
                hand.Count(_ => _.Type == CardType.Soldier) > 0)
            {
                cards.AddRange(GetCards(hand, CardType.Soldier, 1));
                cards.AddRange(GetCards(hand, CardType.Horse, 1));
                cards.AddRange(GetCards(hand, CardType.Cannon, 1));
            }

            if (hand.Count(_ => _.Type == CardType.Cannon) > 2)
            {
                cards.AddRange(GetCards(hand, CardType.Cannon, 3));
            }

            if (hand.Count(_ => _.Type == CardType.Horse) > 2)
            {
                cards.AddRange(GetCards(hand, CardType.Horse, 3));
            }

            if (hand.Count(_ => _.Type == CardType.Soldier) > 2)
            {
                cards.AddRange(GetCards(hand, CardType.Soldier, 3));
            }

            if (!cards.Any())
            {
                return;
            }

            var temp = 0;

            cards.ForEach(_ =>
            {
                if (_territoryStatusMap[_.TerritoryIndex].ControllingPlayerIndex != CurrentPlayerIndex)
                {
                    return;
                }

                temp += 2;
                _territoryStatusMap[_.TerritoryIndex].Armies += 2;
            });

            extraArmiesForControlledTerritories += temp;

            if (_cardSetsTradedForTroops < 5)
            {
                extraArmiesInTotalForCards = 2 + 2 * (_cardSetsTradedForTroops + 1);
            }
            else
            {
                extraArmiesInTotalForCards = 5 * (_cardSetsTradedForTroops - 2);
            }

            extraArmiesInTotalForCards += extraArmiesForControlledTerritories;

            ExtraArmiesForCurrentPlayer += extraArmiesInTotalForCards;
            _hands[CurrentPlayerIndex] = _hands[CurrentPlayerIndex].Except(cards).ToList();
            _drawPile.AddRange(cards.Shuffle(_random));
            _cardSetsTradedForTroops++;
        }

        private IEnumerable<Card> GetCards(
            IEnumerable<Card> hand,
            CardType cardType,
            int nCards)
        {
            return hand
                .Where(_ => _.Type == cardType)
                .Select(_ => new
                {
                    ExtraArmiesForIndividualCard = _territoryStatusMap[_.TerritoryIndex].ControllingPlayerIndex ==
                                                   CurrentPlayerIndex
                        ? 2
                        : 0,
                    Card = _
                }).OrderByDescending(_ => _.ExtraArmiesForIndividualCard)
                .Select(_ => _.Card)
                .Take(nCards);
        }

        public IGameEvent TransferArmies(
            int initialTerritoryIndex,
            int destinationTerritoryIndex,
            int armiesToTransfer,
            bool turnGoesToNextPlayer,
            bool postAttack)
        {
            var gameEvent = new PlayerTransfersArmies(
                CurrentPlayerIndex, turnGoesToNextPlayer)
            {
                Vertex1 = initialTerritoryIndex,
                Vertex2 = destinationTerritoryIndex,
                ArmiesTransfered = armiesToTransfer
            };

            _territoryStatusMap[initialTerritoryIndex].Armies -= armiesToTransfer;
            _territoryStatusMap[destinationTerritoryIndex].Armies += armiesToTransfer;

            if (turnGoesToNextPlayer)
            {
                CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Length;
                CurrentPlayerMayReinforce = true; // This goes for the next player
                CurrentPlayerHasReinforced = false; // This goes for the next player
                CurrentPlayerHasMovedTroops = false; // This goes for the next player
                _currentPlayerMayTransferArmies = true; // This goes for the next player
                _currentPlayerHasConqueredATerritory = false; // This goes for the next player
            }

            CurrentPlayerHasMovedTroops = !postAttack;

            return gameEvent;
        }

        private IEnumerable<int> IndexesOfControlledTerritories(
            int playerId)
        {
            return _territoryStatusMap
                .Where(_ => _.Value.ControllingPlayerIndex == playerId)
                .Select(_ => _.Key);
        }

        // Kaldes fra ExecuteNextEvent samt PlayerSelectsOption, dvs både når computeren agerer og når en spiller agerer
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

            _territoryWasJustConquered = false;
            var defendingPlayerIndex = _territoryStatusMap[targetTerritoryIndex].ControllingPlayerIndex;
            Card card = null;

            if (_territoryStatusMap[targetTerritoryIndex].Armies == 0)
            {
                _territoryWasJustConquered = true;
                _conqueringTerritoryId = activeTerritoryIndex;
                _conqueredTerritoryId = targetTerritoryIndex;
                _armiesInFinalAttack = diceCountAttacker;

                _territoryStatusMap[targetTerritoryIndex].ControllingPlayerIndex = CurrentPlayerIndex;

                if (_territoryStatusMap.All(_ => _.Value.ControllingPlayerIndex == CurrentPlayerIndex))
                {
                    var armiesLeft = _territoryStatusMap[activeTerritoryIndex].Armies;
                    var armyTransferCount = diceCountAttacker;
                    _territoryStatusMap[targetTerritoryIndex].Armies = armyTransferCount;
                    _territoryStatusMap[activeTerritoryIndex].Armies = armiesLeft - armyTransferCount;
                    GameDecided = true;
                    GameInProgress = false;
                }

                if (!_currentPlayerHasConqueredATerritory)
                {
                    card = _drawPile.First();
                    _drawPile = _drawPile.Skip(1).ToList();
                    _hands[CurrentPlayerIndex].Add(card);
                }

                _currentPlayerHasConqueredATerritory = true;
            }

            var gameEvent = new PlayerAttacks(
                CurrentPlayerIndex)
            {
                Vertex1 = activeTerritoryIndex,
                Vertex2 = targetTerritoryIndex,
                DefendingPlayerIndex = defendingPlayerIndex,
                CasualtiesAttacker = casualtiesAttacker,
                CasualtiesDefender = casualtiesDefender,
                TerritoryConquered = _territoryWasJustConquered,
                DiceRolledByAttacker = diceCountAttacker,
                Card = card
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
            _currentPlayerHasConqueredATerritory = false;
            _territoryWasJustConquered = false;
            _currentPlayerMayTransferArmies = true;

            return gameEvent;
        }

        private IGameEvent Reinforce()
        {
            var territoryCount =
                IndexesOfControlledTerritories(CurrentPlayerIndex).Count();

            ExtraArmiesForCurrentPlayer = Math.Max(3, territoryCount / 3);
            CurrentPlayerHasReinforced = true;
            CurrentPlayerMayReinforce = false;

            return new PlayerReinforces(CurrentPlayerIndex, false);
        }

        private IGameEvent DeployArmies(
            int territoryIndex,
            int armies)
        {
            ExtraArmiesForCurrentPlayer -= armies;
            _territoryStatusMap[territoryIndex].Armies += armies;

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
                CurrentPlayerHasReinforced = false; // This goes for the next player
                CurrentPlayerHasMovedTroops = false; // This goes for the next player
                _currentPlayerMayTransferArmies = true; // This goes for the next player
                _currentPlayerHasConqueredATerritory = false; // This goes for the next player
            }

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

        // Returns a dictionary with player ids as keys and a list of connected collections of territories as values
        private Dictionary<int, List<List<int>>> IdentifyConnectedTerritories()
        {
            var result = new Dictionary<int, List<List<int>>>();
            var handled = new HashSet<int>();

            var vertexIds = _graphOfTerritories.Vertices
                .Select(vertex => vertex.Id)
                .ToList();

            vertexIds.ForEach(vertexId=>
            {
                if (handled.Contains(vertexId))
                {
                    return;
                }

                var controllingPlayerIndex =
                    _territoryStatusMap[vertexId].ControllingPlayerIndex;

                var connectedComponent = GetConnectedComponent(
                    controllingPlayerIndex,
                    vertexId, 
                    handled);

                if (!result.ContainsKey(controllingPlayerIndex))
                {
                    result[controllingPlayerIndex] = new List<List<int>>();
                }

                result[controllingPlayerIndex].Add(connectedComponent.ToList());
            });

            return result;
        }

        private List<int> GetConnectedComponent(
            int vertexId,
            int controllingPlayerIndex,
            ISet<int> handled)
        {
            var connectedComponent = new HashSet<int>();
            var queue = new Queue<int>();
            queue.Enqueue(vertexId);

            while (queue.Any())
            {
                var vertexIdInComponent = queue.Dequeue();
                connectedComponent.Add(vertexIdInComponent);
                handled.Add(vertexIdInComponent);

                var neighbors = _graphOfTerritories
                    .NeighborIds(vertexIdInComponent)
                    .ToList();

                neighbors.ForEach(neighbourId =>
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

            return connectedComponent.ToList();
        }

        private void DistributeTerritoriesAmongPlayers2()
        {
            _territoryStatusMap = new Dictionary<int, TerritoryStatus>
            {
                [0] = new TerritoryStatus { ControllingPlayerIndex = 0, Armies = 3 },
                [1] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [2] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [3] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [4] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [5] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [6] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [7] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [8] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [9] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [10] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [11] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [12] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [13] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [14] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [15] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [16] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [17] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [18] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [19] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [20] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [21] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [22] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [23] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [24] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [25] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [26] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [27] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [28] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [29] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [30] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [31] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [32] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [33] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [34] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [35] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [36] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [37] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [38] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [39] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [40] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 },
                [41] = new TerritoryStatus { ControllingPlayerIndex = 2, Armies = 3 }
            };
        }

        private void DistributeTerritoriesAmongPlayers()
        {
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
        }

        private List<Card> GenerateCards()
        {
            return new List<Card>
            {
                new Card(0, CardType.Soldier),
                new Card(1, CardType.Cannon),
                new Card(2, CardType.Horse),
                new Card(3, CardType.Horse),
                new Card(4, CardType.Horse),
                new Card(5, CardType.Horse),
                new Card(6, CardType.Cannon),
                new Card(7, CardType.Cannon),
                new Card(8, CardType.Cannon),

                new Card(9, CardType.Soldier),
                new Card(10, CardType.Soldier),
                new Card(11, CardType.Soldier),
                new Card(12, CardType.Cannon),

                new Card(13, CardType.Soldier),
                new Card(14, CardType.Horse),
                new Card(15, CardType.Cannon),
                new Card(16, CardType.Cannon),
                new Card(17, CardType.Horse),
                new Card(18, CardType.Cannon),
                new Card(19, CardType.Cannon),

                new Card(20, CardType.Horse),
                new Card(21, CardType.Soldier),
                new Card(22, CardType.Soldier),
                new Card(23, CardType.Soldier),
                new Card(24, CardType.Cannon),
                new Card(25, CardType.Horse),

                new Card(26, CardType.Horse),
                new Card(27, CardType.Horse),
                new Card(28, CardType.Horse),
                new Card(29, CardType.Soldier),
                new Card(30, CardType.Horse),
                new Card(31, CardType.Horse),
                new Card(32, CardType.Soldier),
                new Card(33, CardType.Cannon),
                new Card(34, CardType.Soldier),
                new Card(35, CardType.Soldier),
                new Card(36, CardType.Horse),
                new Card(37, CardType.Soldier),

                new Card(38, CardType.Cannon),
                new Card(39, CardType.Soldier),
                new Card(40, CardType.Cannon),
                new Card(41, CardType.Cannon)
            };
        }
    }
}