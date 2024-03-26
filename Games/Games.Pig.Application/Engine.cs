using System;
using System.Linq;
using System.Threading.Tasks;
using Games.Pig.Application.GameEvents;
using Games.Pig.Application.PlayerOptions;

namespace Games.Pig.Application
{
    public class Engine
    {
        private const int _targetScore = 20;
        private const int _dieFaces = 4;

        // An array with a boolean for each player. A boolean with a value of true indicates that the given player is a computer player
        private bool[] _players;

        private Random _random;

        public int CurrentPlayerIndex { get; private set; }

        public int[] PlayerScores { get; }

        public int Pot { get; private set; }

        public bool GameDecided { get; private set; }

        public bool NextEventOccursAutomatically
        {
            get => _players[CurrentPlayerIndex]; 
        }

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
            for (var index = 0; index < PlayerScores.Length; index++)
            {
                PlayerScores[index] = 0;
            }

            GameDecided = false;
            CurrentPlayerIndex = 0;
        }

        public void Reset()
        {
            Pot = 0;
            GameDecided = false;
            CurrentPlayerIndex = 0;

            for (var i = 0; i < PlayerScores.Length; i++)
            {
                PlayerScores[i] = 0;
            }
        }

        public async Task<IGameEvent> ExecuteNextEvent()
        {
            await Task.Delay(1);

            if (Pot + PlayerScores[CurrentPlayerIndex] < _targetScore && (Pot < 10 || _random.Next(2) == 1))
            {
                return RollDie();
            }
                
            return TakePot();
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
                case TakePot _:
                {
                    return TakePot();
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

            var gameEvent = new PlayerRollsDie
            {
                Player = CurrentPlayerIndex + 1,
                DieRoll = dieRoll
            };

            if (dieRoll > 1)
            {
                Pot += dieRoll;
            }
            else
            {
                Pot = 0;
                CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Length;
            }

            return gameEvent;
        }

        private IGameEvent TakePot()
        {
            PlayerScores[CurrentPlayerIndex] += Pot;
            Pot = 0;

            var gameEvent = new PlayerTakesPot
            {
                Player = CurrentPlayerIndex + 1,
                NewScore = PlayerScores[CurrentPlayerIndex]
            };

            if (PlayerScores[CurrentPlayerIndex] >= _targetScore)
            {
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
