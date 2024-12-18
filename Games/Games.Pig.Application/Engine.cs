﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Craft.Logging;
using Games.Pig.Application.GameEvents;
using Games.Pig.Application.PlayerOptions;

namespace Games.Pig.Application
{
    public class Engine
    {
        private const int _targetScore = 100;
        private const int _dieFaces = 6;

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
            CurrentPlayerIndex = 0;

            Logger?.WriteLine(LogMessageCategory.Information, $"New Game Started - Player {CurrentPlayerIndex + 1} begins");
        }

        public async Task<IGameEvent> ExecuteNextEvent()
        {
            await Task.Delay(1);

            if (Pot + PlayerScores[CurrentPlayerIndex] < _targetScore && (Pot < 20 || _random.Next(2) == 1))
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

        private IGameEvent TakePot()
        {
            PlayerScores[CurrentPlayerIndex] += Pot;
            Pot = 0;

            var gameEvent = new PlayerTakesPot(
                CurrentPlayerIndex,
                $"Player {CurrentPlayerIndex + 1} takes pot and now has a score of {PlayerScores[CurrentPlayerIndex]}",
                true)
            {
                NewScore = PlayerScores[CurrentPlayerIndex]
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
