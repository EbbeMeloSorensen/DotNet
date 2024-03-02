using System;
using System.Linq;
using System.Threading.Tasks;
using Games.Race.Application.GameEvents;

namespace Games.Race.Application
{
    public class Engine
    {
        private Random _random;

        public int CurrentPlayerIndex { get; private set; }

        public int[] PlayerScores { get; private set; }

        public bool GameDecided { get; private set; }

        public Engine(
            int players)
        {
            if (players < 2 || players > 10)
            {
                throw new ArgumentOutOfRangeException("Invalid number of players");
            }

            _random = new Random(0);

            PlayerScores = Enumerable
                .Repeat(1, players)
                .ToArray();
        }

        public void StartGame()
        {
            GameDecided = false;
            CurrentPlayerIndex = 0;
        }

        public async Task<IGameEvent> ExecuteNextEvent()
        {
            await Task.Delay(1);

            var dieRoll = _random.Next(6);
            PlayerScores[CurrentPlayerIndex] += dieRoll;

            var result = new PlayerAdvances
            {
                Player = CurrentPlayerIndex + 1,
                Squares = dieRoll,
                Total = PlayerScores[CurrentPlayerIndex]
            };

            if (PlayerScores[CurrentPlayerIndex] >= 100)
            {
                GameDecided = true;
            }
            else
            {
                CurrentPlayerIndex = (CurrentPlayerIndex + 1) % PlayerScores.Length;
            }

            return result;
        }
    }
}
