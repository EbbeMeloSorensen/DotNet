using System.Collections.ObjectModel;
using System.Linq;
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
        private const int _delay = 100;

        private ViewModelLogger _viewModelLogger;
        private bool _loggingActive;
        private readonly Application.Application _application;

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
                        case PlayerTakesPot _:
                        {
                            UpdateScore(gameEvent.PlayerIndex);
                            break;
                        }
                    }

                    if (gameEvent.TurnGoesToNextPlayer)
                    {
                        HighlightCurrentPlayer();
                    }

                    continue;
                }

                UpdateCommandAvailability();
                break;
            }

            if (_application.Engine.GameDecided)
            {
                //UpdateScore(_application.Engine.CurrentPlayerIndex);
                GameResultMessage = $"Game Over\nPlayer {_application.Engine.CurrentPlayerIndex + 1} Wins";
                GameInProgress = _application.Engine.GameInProgress;
                GameDecided = _application.Engine.GameDecided;
            }
            else
            {
                PlayerHasInitiative = true;
            }

            UpdateCommandAvailability();
        }

        private void StartGame()
        {
            _application.Engine.Reset();

            Enumerable
                .Range(0, _application.Engine.PlayerCount)
                .ToList()
                .ForEach(playerIndex =>
                {
                    UpdateScore(playerIndex);
                    PlayerViewModels[playerIndex].HasInitiative = false;
                });

            _application.Engine.StartGame();

            SyncControlsWithApplication();
            HighlightCurrentPlayer();
            UpdateCommandAvailability();
        }

        private bool CanStartGame()
        {
            return !GameInProgress || PlayerHasInitiative;
        }

        private async Task RollDie()
        {
            var gameEvent = await _application.Engine.PlayerSelectsOption(new RollDie());

            _application.Logger?.WriteLine(
                LogMessageCategory.Information,
                gameEvent.Description);

            Pot = _application.Engine.Pot;

            if (Pot == 0)
            {
                PlayerHasInitiative = false;
                HighlightCurrentPlayer();
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
            var gameEvent = await _application.Engine.PlayerSelectsOption(new TakePot());

            _application.Logger?.WriteLine(
                LogMessageCategory.Information,
                gameEvent.Description);

            Pot = _application.Engine.Pot;
            UpdateScore(gameEvent.PlayerIndex);

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
                SyncControlsWithApplication();
                HighlightCurrentPlayer();
                UpdateCommandAvailability();
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
            GameInProgress = _application.Engine.GameInProgress;
            GameDecided = _application.Engine.GameDecided;
            PlayerHasInitiative = !_application.Engine.NextEventOccursAutomatically;
            Pot = _application.Engine.Pot;
        }

        private void HighlightCurrentPlayer()
        {
            var indexOfPreviousPlayer =
                (_application.Engine.CurrentPlayerIndex - 1 + _application.Engine.PlayerCount) %
                _application.Engine.PlayerCount;

            PlayerViewModels[indexOfPreviousPlayer].HasInitiative = false;
            PlayerViewModels[_application.Engine.CurrentPlayerIndex].HasInitiative = true;
        }

        private void UpdateCommandAvailability()
        {
            StartGameCommand.RaiseCanExecuteChanged();
            RollDieCommand.RaiseCanExecuteChanged();
            TakePotCommand.RaiseCanExecuteChanged();
        }

        private void UpdateScore(
            int playerIndex)
        {
            PlayerViewModels[playerIndex].Score =
                _application.Engine.PlayerScores[playerIndex];
        }
    }
}