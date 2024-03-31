using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Craft.Logging;
using Games.Race.Application.GameEvents;
using Games.Race.Application.PlayerOptions;

namespace Games.Race.Application
{
    public class Engine
    {
        private const int _targetScore = 20;
        private const int _dieFaces = 6;

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
            bool pseudoRandomNumbers)
        {
            var playerCount = players.Count();

            if (playerCount < 2 || playerCount > 10)
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
            GameInProgress = true;
            //CurrentPlayerIndex = _random.Next(0, _players.Length);
            CurrentPlayerIndex = 0;

            Logger?.WriteLine(LogMessageCategory.Information, $"New Game Started - Player {CurrentPlayerIndex + 1} begins");
        }

        public async Task<IGameEvent> ExecuteNextEvent()
        {
            await Task.Delay(1);

            return RollDie();
        }

        public async Task<IGameEvent> PlayerSelectsOption(
            IPlayerOption option)
        {
            await Task.Delay(1);

            switch (option)
            {
                case RollDie _:
                    {
                        return RollDie();
                    }
                default:
                    {
                        throw new InvalidOperationException("Invalid player option");
                    }
            }
        }

        private IGameEvent RollDie()
        {
            var dieRoll = _random.Next(1, _dieFaces + 1);
            var sb = new StringBuilder($"Player {CurrentPlayerIndex + 1} rolls die and gets {dieRoll}");

            PlayerScores[CurrentPlayerIndex] += dieRoll;

            var gameEvent = new PlayerRollsDie(
                CurrentPlayerIndex,
                sb.ToString(),
                true)
            {
                DieRoll = dieRoll
            };

            if (PlayerScores[CurrentPlayerIndex] >= _targetScore)
            {
                GameInProgress = false;
                GameDecided = true;
            }
            else
            {
                CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Length;
            }

            return gameEvent;
        }
    }
}
