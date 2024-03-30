using System.Collections.ObjectModel;
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
        private const int _delay = 800;

        private ViewModelLogger _viewModelLogger;
        private bool _loggingActive;
        private readonly Application.Application _application;

        private int _currentPlayerIndex;
        private bool _playerHasInitiative;
        private bool _gameInProgress;
        private bool _gameDecided;
        private string _gameResultMessage;
        private int _pot;

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

        public ObservableCollection<PlayerViewModel> PlayerViewModels { get; }

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

        public int CurrentPlayerIndex
        {
            get { return _currentPlayerIndex; }
            set
            {
                _currentPlayerIndex = value;
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

        public bool GameInProgress
        {
            get => _gameInProgress;
            private set
            {
                _gameInProgress = value;
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

        public int Pot
        {
            get => _pot;
            private set
            {
                _pot = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel(
            Application.Application application)
        {
            _application = application;
            _application.Engine = new Engine(new[] { true, true, false, true }, true);

            PlayerViewModels = new ObservableCollection<PlayerViewModel>
            {
                new PlayerViewModel{ Name = "Player 1 (computer)", Score = 0 },
                new PlayerViewModel{ Name = "Player 2 (computer)", Score = 0 },
                new PlayerViewModel{ Name = "Player 3 (you)", Score = 0 },
                new PlayerViewModel{ Name = "Player 4 (computer)", Score = 0 }
            };

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
                    await Task.Delay(_delay);

                    _application.Logger?.WriteLine(
                        LogMessageCategory.Information,
                        gameEvent.Description);

                    Pot = _application.Engine.Pot;

                    switch (gameEvent)
                    {
                        case PlayerRollsDie _:
                            {
                                break;
                            }
                        case PlayerTakesPot _:
                            {
                                var indexOfPreviousPlayer= 
                                    (_application.Engine.CurrentPlayerIndex + _application.Engine.PlayerCount - 1) % _application.Engine.PlayerCount;

                                PlayerViewModels[indexOfPreviousPlayer].Score =
                                    _application.Engine.PlayerScores[indexOfPreviousPlayer];

                                PlayerViewModels[indexOfPreviousPlayer].HasInitiative = false;
                                PlayerViewModels[_application.Engine.CurrentPlayerIndex].HasInitiative = true;

                                break;
                            }
                    }

                    if (gameEvent.TurnGoesToNextPlayer)
                    {
                        var indexOfPreviousPlayer =
                            (_application.Engine.CurrentPlayerIndex + _application.Engine.PlayerCount - 1) % _application.Engine.PlayerCount;

                        PlayerViewModels[indexOfPreviousPlayer].HasInitiative = false;
                        PlayerViewModels[_application.Engine.CurrentPlayerIndex].HasInitiative = true;
                    }

                    continue;
                }

                UpdateCommandAvailability();
                break;
            }

            if (_application.Engine.GameDecided)
            {
                GameResultMessage = "Game Over - You Lost";
                GameInProgress = _application.Engine.GameInProgress;
                GameDecided = _application.Engine.GameDecided;

                UpdateCommandAvailability();
            }
        }

        private void StartGame()
        {
            _application.Engine.Reset();
            _application.Engine.StartGame();

            SyncControlsWithApplication();
            UpdateCommandAvailability();
        }

        private bool CanStartGame()
        {
            return !GameInProgress || PlayerHasInitiative;
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
            return GameInProgress && PlayerHasInitiative;
        }

        private async Task TakePot()
        {
            var gameEvent = await _application.Engine.PlayerSelectsOption(new TakePot()) as PlayerTakesPot;

            _application.Logger?.WriteLine(
                LogMessageCategory.Information,
                gameEvent.Description.Replace("Player 2", "Player"));

            Pot = _application.Engine.Pot;

            if (_application.Engine.GameDecided)
            {
                GameResultMessage = "Congratulations - You Win";
                GameInProgress = _application.Engine.GameInProgress;
                GameDecided = _application.Engine.GameDecided;
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
                   GameInProgress &&
                   PlayerHasInitiative;
        }

        private void SyncControlsWithApplication()
        {
            CurrentPlayerIndex = _application.Engine.CurrentPlayerIndex;
            GameInProgress = _application.Engine.GameInProgress;
            GameDecided = _application.Engine.GameDecided;
            PlayerHasInitiative = !_application.Engine.NextEventOccursAutomatically;
            Pot = _application.Engine.Pot;
            PlayerViewModels[CurrentPlayerIndex].HasInitiative = true;
        }

        private void UpdateCommandAvailability()
        {
            StartGameCommand.RaiseCanExecuteChanged();
            RollDieCommand.RaiseCanExecuteChanged();
            TakePotCommand.RaiseCanExecuteChanged();
        }
    }
}