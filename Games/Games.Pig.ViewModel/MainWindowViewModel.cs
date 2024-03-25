using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.ViewModel.Utils;
using Games.Pig.Application;
using Games.Pig.Application.GameEvents;
using Games.Pig.Application.PlayerOptions;

namespace Games.Pig.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Engine _engine;
        private int _pot;
        private int _computerScore;
        private int _playerScore;
        private bool _playerHasInitiative;

        private AsyncCommand _startGameCommand;
        private AsyncCommand _rollDieCommand;
        private AsyncCommand _takePotCommand;

        public AsyncCommand StartGameCommand => _startGameCommand ??= new AsyncCommand(StartGame, CanStartGame);
        public AsyncCommand RollDieCommand => _rollDieCommand ??= new AsyncCommand(RollDie, CanRollDie);
        public AsyncCommand TakePotCommand => _takePotCommand ??= new AsyncCommand(TakePot, CanTakePot);

        public int Pot
        {
            get => _pot;
            private set
            {
                _pot = value;
                RaisePropertyChanged();
            }
        }

        public int ComputerScore
        {
            get => _computerScore;
            private set
            {
                _computerScore = value;
                RaisePropertyChanged();
            }
        }

        public int PlayerScore
        {
            get => _playerScore;
            private set
            {
                _playerScore = value;
                RaisePropertyChanged();
            }
        }

        public bool PlayerHasInitiative
        {
            get => _playerHasInitiative;
            private set
            {
                _playerHasInitiative = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            var players = new[] { true, false };
            _engine = new Engine(players);
            PlayerHasInitiative = !players[0];
        }

        private async Task Proceed()
        {
            while (!_engine.GameDecided)
            {
                if (_engine.NextEventOccursAutomatically)
                {
                    var nextEvent = await _engine.ExecuteNextEvent();

                    switch (nextEvent)
                    {
                        case TakePot gameEvent:
                        {
                            ComputerScore = _engine.PlayerScores[0];
                            break;
                        }
                    }

                    Pot = _engine.Pot;
                    PlayerHasInitiative = _engine.CurrentPlayerIndex == 1;

                    if (!PlayerHasInitiative)
                    {
                        await Task.Delay(500);
                        continue;
                    }

                    RollDieCommand.RaiseCanExecuteChanged();
                    break;
                }
            }
        }

        private async Task StartGame()
        {
            _engine.StartGame();
            await Proceed();
        }

        private bool CanStartGame()
        {
            return true;
        }

        private async Task RollDie()
        {
            var gameEvent = await _engine.PlayerSelectsOption(new RollDie()) as PlayerRollsDie;

            Pot = _engine.Pot;

            if (Pot == 0)
            {
                PlayerHasInitiative = false;
                await Proceed();
            }
            else
            {
                TakePotCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanRollDie()
        {
            return PlayerHasInitiative;
        }

        private async Task TakePot()
        {
            var gameEvent = await _engine.PlayerSelectsOption(new TakePot()) as PlayerTakesPot;

            PlayerScore = _engine.PlayerScores[1];
            Pot = _engine.Pot;
            PlayerHasInitiative = false;
            await Proceed();
        }

        private bool CanTakePot()
        {
            return _engine.Pot > 0 && PlayerHasInitiative;
        }
    }
}
