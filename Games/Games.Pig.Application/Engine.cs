using System;
using System.Linq;
using System.Threading.Tasks;
using Games.Pig.Application.GameEvents;

namespace Games.Pig.Application
{
    public class Engine
    {
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
            bool[] players)
        {
            var playerCount = players.Count();

            if (playerCount < 2 || playerCount > 10)
            {
                throw new ArgumentOutOfRangeException("Invalid number of players");
            }

            _random = new Random(0);

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

        public async Task<IGameEvent> ExecuteNextEvent()
        {
            await Task.Delay(1);

            if (PlayerScores[0] == 90 && CurrentPlayerIndex == 0)
            {
                var a = 0;
            }

            if (Pot + PlayerScores[CurrentPlayerIndex] < 100 && (Pot < 10 || _random.Next(2) == 1))
            {
                var dieRoll = _random.Next(1, 6);

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
            else
            {
                PlayerScores[CurrentPlayerIndex] += Pot;
                Pot = 0;

                var gameEvent = new PlayerTakesPot
                {
                    Player = CurrentPlayerIndex + 1,
                    NewScore = PlayerScores[CurrentPlayerIndex]
                };

                if (PlayerScores[CurrentPlayerIndex] >= 100)
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
}
