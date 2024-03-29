using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Craft.Logging;
using Craft.ViewModel.Utils;
using Games.Pig.Application;
using Games.Pig.Application.GameEvents;
using Games.Pig.Application.PlayerOptions;

namespace Games.Pig.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const int _delay = 200;

        private ViewModelLogger _viewModelLogger;
        private bool _loggingActive;
        private readonly Application.Application _application;

        private int _pot;
        private int _computerScore;
        private int _playerScore;
        private bool _playerHasInitiative;
        private bool _gameOngoing;
        private bool _gameDecided;
        private string _gameResultMessage;

        private AsyncCommand _startGameCommand;
        private AsyncCommand _rollDieCommand;
        private AsyncCommand _takePotCommand;

        public AsyncCommand StartGameCommand => _startGameCommand ??= new AsyncCommand(
            () =>
            {
                StartGame();
                return Proceed();
            }, CanStartGame);

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

        public bool GameOngoing
        {
            get => _gameOngoing;
            private set
            {
                _gameOngoing = value;
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
            _application.Engine = new Engine(new[] { true, false }, true);

            LogViewModel = new LogViewModel();
            _viewModelLogger = new ViewModelLogger(_application.Logger, LogViewModel);
            LoggingActive = true;

            _application.Logger?.WriteLine(LogMessageCategory.Debug, "Pig Game - starting up");
        }

        private async Task Proceed()
        {
            while (!_application.Engine.GameDecided)
            {
                if (_application.Engine.NextEventOccursAutomatically)
                {
                    var gameEvent = await _application.Engine.ExecuteNextEvent();

                    _application.Logger?.WriteLine(
                        LogMessageCategory.Information,
                        gameEvent.Description.Replace("Player 1", "Computer"));

                    Pot = _application.Engine.Pot;

                    switch (gameEvent)
                    {
                        case PlayerRollsDie _:
                        {
                            PlayerHasInitiative = _application.Engine.CurrentPlayerIndex == 1;
                            break;
                        }
                        case PlayerTakesPot _:
                        {
                            ComputerScore = _application.Engine.PlayerScores[0];
                            PlayerHasInitiative = !_application.Engine.GameDecided;
                            break;
                        }
                    }

                    if (!PlayerHasInitiative && !_application.Engine.GameDecided)
                    {
                        await Task.Delay(_delay);
                        continue;
                    }
                }

                UpdateCommandAvailability();
                break;
            }

            if (_application.Engine.GameDecided)
            {
                GameResultMessage = "Game Over - You Lost";
                GameOngoing = false;
                GameDecided = true;

                UpdateCommandAvailability();
            }
        }

        private void StartGame()
        {
            _application.Engine.Reset();
            _application.Engine.StartGame();

            GameOngoing = true;
            GameDecided = false;
            PlayerHasInitiative = !_application.Engine.NextEventOccursAutomatically;
            ComputerScore = 0;
            PlayerScore = 0;
            Pot = 0;

            UpdateCommandAvailability();
        }

        private bool CanStartGame()
        {
            return !GameOngoing || PlayerHasInitiative;
        }

        private async Task RollDie()
        {
            var gameEvent = await _application.Engine.PlayerSelectsOption(new RollDie()) as PlayerRollsDie;

            _application.Logger?.WriteLine(
                LogMessageCategory.Information,
                gameEvent.Description.Replace("Player 2", "Player"));

            Pot = _application.Engine.Pot;

            if (Pot == 0)
            {
                PlayerHasInitiative = false;
                UpdateCommandAvailability();
                await Proceed();
            }
            else
            {
                UpdateCommandAvailability();
            }
        }

        private bool CanRollDie()
        {
            return GameOngoing && PlayerHasInitiative;
        }

        private async Task TakePot()
        {
            var gameEvent = await _application.Engine.PlayerSelectsOption(new TakePot()) as PlayerTakesPot;

            _application.Logger?.WriteLine(
                LogMessageCategory.Information,
                gameEvent.Description.Replace("Player 2", "Player"));

            Pot = _application.Engine.Pot;
            PlayerScore = _application.Engine.PlayerScores[1];

            if (_application.Engine.GameDecided)
            {
                GameResultMessage = "Congratulations - You Win";
                GameOngoing = false;
                GameDecided = true;
                await Task.Delay(_delay);
            }
            else
            {
                PlayerHasInitiative = false;
                UpdateCommandAvailability();
                await Task.Delay(_delay);
                await Proceed();
            }
        }

        private bool CanTakePot()
        {
            return _application.Engine.Pot > 0 && 
                   GameOngoing &&
                   PlayerHasInitiative;
        }

        private void UpdateCommandAvailability()
        {
            StartGameCommand.RaiseCanExecuteChanged();
            RollDieCommand.RaiseCanExecuteChanged();
            TakePotCommand.RaiseCanExecuteChanged();
        }
    }
}