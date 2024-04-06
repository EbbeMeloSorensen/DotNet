using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private Dictionary<int, int> _territoryToPlayerMap;
        private bool _pseudoRandomNumbers;

        // An array with a boolean for each player. A boolean with a value of true indicates that the given player is a computer player
        private bool[] _players;

        private Random _random;

        public int PlayerCount => PlayerScores.Length;

        public int CurrentPlayerIndex { get; private set; }

        public int[] PlayerScores { get; }

        public int Pot { get; private set; }

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

            _territoryToPlayerMap = new Dictionary<int, int>();

            var playerId = 0;
            foreach (var vertexId in vertexIds)
            {
                _territoryToPlayerMap[vertexId] = playerId;
                playerId = (playerId + 1) % PlayerCount;
            }

            GameInProgress = true;
            CurrentPlayerIndex = 0;

            Logger?.WriteLine(LogMessageCategory.Information, $"New Game Started - Player {CurrentPlayerIndex + 1} begins");
        }

        public async Task<IGameEvent> ExecuteNextEvent()
        {
            await Task.Delay(1);

            if (_random.Next(4) == 0)
            {
                return Pass();
            }

            return Attack();
        }

        public async Task<IGameEvent> PlayerSelectsOption(
            IPlayerOption option)
        {
            await Task.Delay(1);

            switch (option)
            {
                case Attack _:
                    {
                        return Attack();
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

        public int IdOfPlayerCurrentlyControllingTerritory(
            int territoryId)
        {
            return _territoryToPlayerMap[territoryId];
        }

        private IGameEvent RollDie()
        {
            var dieRoll = _random.Next(1, _dieFaces + 1);
            var sb = new StringBuilder($"Player {CurrentPlayerIndex + 1} rolls die and gets {dieRoll}");

            if (dieRoll == 1)
            {
                Pot = 0;
                sb.Append($" => Player {CurrentPlayerIndex + 1} looses turn and pot is reset");
            }
            else
            {
                Pot += dieRoll;
                sb.Append($" => Pot is now at {Pot}");
            }

            var gameEvent = new PlayerRollsDie(
                CurrentPlayerIndex,
                sb.ToString(),
                dieRoll == 1)
            {
                DieRoll = dieRoll
            };

            if (dieRoll == 1)
            {
                CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Length;
            }

            return gameEvent;
        }

        private IGameEvent Attack()
        {
            // Which vertices are controlled by the current player?
            var vertexIndexes = _territoryToPlayerMap
                .Where(_ => _.Value == CurrentPlayerIndex)
                .Select(_ => _.Key)
                .ToList();

            // Traverse all those vertices and identify neighbours controlled by other players
            var options = new List<Tuple<int, int>>();

            vertexIndexes.ForEach(vertexIndex =>
            //foreach (var vertexIndex in vertexIndexes)
            {
                var adjacentEdges = _graphOfTerritories.GetAdjacentEdges(vertexIndex);

                var neighbourIds = adjacentEdges.Select(_ => _.VertexId1 == vertexIndex ? _.VertexId2 : _.VertexId1);

                foreach (var neighbourId in neighbourIds)
                {
                    if (!vertexIndexes.Contains(neighbourId))
                    {
                        options.Add(new Tuple<int, int>(vertexIndex, neighbourId));
                    }
                }
            });

            var randomIndex = _random.Next(0, options.Count);
            var selectedOption = options[randomIndex];

            var indexOfSourceVertex = selectedOption.Item1;
            var indexOfTargetVertex = selectedOption.Item2;

            var gameEvent = new PlayerAttacks(
                CurrentPlayerIndex,
                $"Player {CurrentPlayerIndex + 1} attacks",
                false)
            {
                Vertex1 = indexOfSourceVertex,
                Vertex2 = indexOfTargetVertex
            };

            return gameEvent;
        }

        private IGameEvent Pass()
        {
            var gameEvent = new PlayerPasses(
                CurrentPlayerIndex,
                $"Player {CurrentPlayerIndex + 1} passes",
                true);

            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Length;

            return gameEvent;
        }
    }
}
