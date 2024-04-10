using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Craft.Logging;
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

            // Shuffle the vertex ids;
            if (!_pseudoRandomNumbers)
            {
                vertexIds = vertexIds.OrderBy(_ => Guid.NewGuid()).ToList();
            }

            _territoryStatusMap = new Dictionary<int, TerritoryStatus>();

            var playerId = 0;
            foreach (var vertexId in vertexIds)
            {
                _territoryStatusMap[vertexId] = new TerritoryStatus
                {
                    ControllingPlayerIndex = playerId,
                    Armies = 16
                };
                playerId = (playerId + 1) % PlayerCount;
            }

            GameInProgress = true;
            CurrentPlayerIndex = 0;
            _currentPlayerMayReinforce = true;

            Logger?.WriteLine(LogMessageCategory.Information, $"New Game Started - Player {CurrentPlayerIndex + 1} begins");
        }

        public async Task<IGameEvent> ExecuteNextEvent()
        {
            await Task.Delay(1);

            var options = IdentifyOptionsForCurrentPlayer();

            if (options.Count == 0)
            {
                return Pass();
            }

            var bestOption = options.OrderByDescending(_ => _.OpportunityRating).First();

            if (bestOption.OpportunityRating > 0)
            {
                return Attack(
                    bestOption.IndexOfTerritoryWhereAttackOriginates,
                    bestOption.IndexOfTerritoryUnderAttack);
            }

            if (_currentPlayerMayReinforce)
            {
                return Reinforce();
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
            var adjacentEdges = _graphOfTerritories.GetAdjacentEdges(territoryId);
            var neighbourIds = adjacentEdges.Select(_ => _.VertexId1 == territoryId ? _.VertexId2 : _.VertexId1);

            return neighbourIds.Except(
                _territoryStatusMap.Where(_ => _.Value.ControllingPlayerIndex == CurrentPlayerIndex).Select(_ => _.Key));
        }

        private IGameEvent Attack(
            int activeTerritoryIndex,
            int targetTerritoryIndex)
        {
            // Todo: Replace with proper battle dynamics
            _territoryStatusMap[targetTerritoryIndex].Armies -= 1;

            if (_territoryStatusMap[targetTerritoryIndex].Armies == 0)
            {
                _territoryStatusMap[targetTerritoryIndex].ControllingPlayerIndex = CurrentPlayerIndex;

                // For nu flytter vi havldelen over (rundet ned)
                var armiesLeft = _territoryStatusMap[activeTerritoryIndex].Armies;
                _territoryStatusMap[targetTerritoryIndex].Armies = armiesLeft / 2;
                _territoryStatusMap[activeTerritoryIndex].Armies = armiesLeft - armiesLeft / 2;

                if (_territoryStatusMap.All(_ => _.Value.ControllingPlayerIndex == CurrentPlayerIndex))
                {
                    GameDecided = true;
                    GameInProgress = false;
                }
            }

            var gameEvent = new PlayerAttacks(
                CurrentPlayerIndex,
                $"Player {CurrentPlayerIndex + 1} attacks vertex {targetTerritoryIndex} from vertex {activeTerritoryIndex}",
                false)
            {
                Vertex1 = activeTerritoryIndex,
                Vertex2 = targetTerritoryIndex
            };

            _currentPlayerMayReinforce = false;

            return gameEvent;
        }

        private IGameEvent Pass()
        {
            var gameEvent = new PlayerPasses(
                CurrentPlayerIndex,
                $"Player {CurrentPlayerIndex + 1} passes",
                true);

            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Length;
            _currentPlayerMayReinforce = true;

            return gameEvent;
        }

        private IGameEvent Reinforce()
        {
            var territoryCount = _territoryStatusMap.Count(_ => _.Value.ControllingPlayerIndex == CurrentPlayerIndex);
            var extraArmies = Math.Max(3, territoryCount / 3);

            // For now, just distribute the armies on the territories with the fewest armies
            var territoryIndexes = new List<int>();

            Enumerable.Repeat(0, extraArmies)
                .ToList()
                .ForEach(_ =>
                {
                    var territoryIndex = _territoryStatusMap
                        .Where(_ => _.Value.ControllingPlayerIndex == CurrentPlayerIndex)
                        .OrderBy(_ => _.Value.Armies)
                        .First()
                        .Key;

                    _territoryStatusMap[territoryIndex].Armies += 1;

                    territoryIndexes.Add(territoryIndex);
                });

            var reinforcedTerritoryIndexesAsCSV  = territoryIndexes
                .Select(_ => _.ToString()).Aggregate((c, n) => $"{c}, {n}");

            var gameEvent = new PlayerReinforces(
                CurrentPlayerIndex,
                $"Player {CurrentPlayerIndex + 1} reinforces: {reinforcedTerritoryIndexesAsCSV}",
                true);

            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Length;
            _currentPlayerMayReinforce = true;

            return gameEvent;
        }

        private List<AttackOption> IdentifyOptionsForCurrentPlayer()
        {
            // Which vertices are controlled by the current player?
            var vertexIndexes = _territoryStatusMap
                .Where(_ => _.Value.ControllingPlayerIndex == CurrentPlayerIndex)
                .Select(_ => _.Key)
                .ToList();

            // Traverse all those vertices and identify neighbours controlled by other players
            var options = new List<AttackOption>();

            vertexIndexes.ForEach(vertexIndex =>
            {
                if (_territoryStatusMap[vertexIndex].Armies == 1)
                {
                    return;
                }
                
                var adjacentEdges = _graphOfTerritories.GetAdjacentEdges(vertexIndex);

                var neighbourIds = adjacentEdges.Select(_ => _.VertexId1 == vertexIndex ? _.VertexId2 : _.VertexId1);

                var armiesInTerritory = _territoryStatusMap[vertexIndex].Armies;

                foreach (var neighbourId in neighbourIds)
                {
                    if (!vertexIndexes.Contains(neighbourId))
                    {
                        var armiesInOpposingTerritory = _territoryStatusMap[neighbourId].Armies;

                        options.Add(new AttackOption
                        {
                            IndexOfTerritoryWhereAttackOriginates = vertexIndex,
                            IndexOfTerritoryUnderAttack = neighbourId,
                            OpportunityRating = armiesInTerritory - armiesInOpposingTerritory
                        });
                    }
                }
            });

            return options;
        }
    }
}
