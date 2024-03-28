using System.Threading.Tasks;
using Craft.Logging;
using GalaSoft.MvvmLight;
using Craft.ViewModel.Utils;
using Games.Pig.Application;
using Games.Pig.Application.GameEvents;
using Games.Pig.Application.PlayerOptions;

namespace Games.Pig.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelLogger _viewModelLogger;
        private bool _loggingActive;
        private readonly Application.Application _application;

        private Engine _engine;
        private int _pot;
        private int _computerScore;
        private int _playerScore;
        private bool _playerHasInitiative;
        private bool _gameDecided;
        private string _gameResultMessage;

        private AsyncCommand _startGameCommand;
        private AsyncCommand _rollDieCommand;
        private AsyncCommand _takePotCommand;

        public AsyncCommand StartGameCommand => _startGameCommand ??= new AsyncCommand(StartGame, CanStartGame);
        public AsyncCommand RollDieCommand => _rollDieCommand ??= new AsyncCommand(RollDie, CanRollDie);
        public AsyncCommand TakePotCommand => _takePotCommand ??= new AsyncCommand(TakePot, CanTakePot);

        public LogViewModel LogViewModel { get; }

        public bool LoggingActive
        {
            get => _loggingActive;
            set
            {
                _loggingActive = value;

                RaisePropertyChanged();

                _application.Logger = _loggingActive
                    ? _viewModelLogger
                    : null;
            }
        }

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

        public bool GameDecided
        {
            get => _gameDecided;
            private set
            {
                _gameDecided = value;
                RaisePropertyChanged();
            }
        }

        public string GameResultMessage
        {
            get => _gameResultMessage;
            set
            {
                _gameResultMessage = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel(
            Application.Application application)
        {
            _application = application;

            LogViewModel = new LogViewModel();

            _viewModelLogger = new ViewModelLogger(_application.Logger, LogViewModel);

            LoggingActive = true;

            var players = new[] { true, false };
            _engine = new Engine(players, true);

            _application.Logger?.WriteLine(LogMessageCategory.Debug, "Pig Game - starting up");
        }

        private async Task Proceed()
        {
            while (!_engine.GameDecided)
            {
                if (_engine.NextEventOccursAutomatically)
                {
                    var nextEvent = await _engine.ExecuteNextEvent();

                    Pot = _engine.Pot;

                    switch (nextEvent)
                    {
                        case PlayerRollsDie gameEvent:
                        {
                            PlayerHasInitiative = _engine.CurrentPlayerIndex == 1;
                            break;
                        }
                        case PlayerTakesPot gameEvent:
                        {
                            ComputerScore = _engine.PlayerScores[0];
                            PlayerHasInitiative = !_engine.GameDecided;
                            break;
                        }
                    }

                    if (!PlayerHasInitiative && !_engine.GameDecided)
                    {
                        await Task.Delay(500);
                        continue;
                    }

                    RollDieCommand.RaiseCanExecuteChanged();
                    break;
                }
            }

            if (_engine.GameDecided)
            {
                GameResultMessage = "Game Over - You Lost";
                GameDecided = true;
            }
        }

        private async Task StartGame()
        {
            _engine.Reset();

            GameDecided = false;
            PlayerHasInitiative = !_engine.NextEventOccursAutomatically;
            ComputerScore = 0;
            PlayerScore = 0;
            Pot = 0;
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

            Pot = _engine.Pot;
            PlayerScore = _engine.PlayerScores[1];

            if (_engine.GameDecided)
            {
                GameResultMessage = "Congratulations - You Win";
                GameDecided = true;
            }
            else
            {
                PlayerHasInitiative = false;
                await Proceed();
            }
        }

        private bool CanTakePot()
        {
            return _engine.Pot > 0 && PlayerHasInitiative;
        }
    }
}
