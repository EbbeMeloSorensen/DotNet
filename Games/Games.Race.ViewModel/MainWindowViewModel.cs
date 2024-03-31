using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Craft.Logging;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Games.Race.Application;
using Games.Race.Application.PlayerOptions;

namespace Games.Race.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IDialogService _applicationDialogService;
        private const bool _pseudoRandomNumbers = false;
        private readonly Random _random;
        private const int _delay = 200;

        private ViewModelLogger _viewModelLogger;
        private bool _loggingActive;
        private readonly Application.Application _application;

        private bool _playerHasInitiative;
        private bool _gameInProgress;
        private bool _gameDecided;
        private string _gameResultMessage;

        private RelayCommand<object> _openSettingsDialogCommand;
        private AsyncCommand _startGameCommand;
        private AsyncCommand _rollDieCommand;

        public RelayCommand<object> OpenSettingsDialogCommand =>
            _openSettingsDialogCommand ??= new RelayCommand<object>(OpenSettingsDialog);

        public AsyncCommand StartGameCommand => _startGameCommand ??= new AsyncCommand(
            () =>
            {
                StartGame();
                return Proceed();
            }, CanStartGame);

        public AsyncCommand RollDieCommand => _rollDieCommand ??= new AsyncCommand(RollDie, CanRollDie);

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

        public MainWindowViewModel(
            Application.Application application,
            IDialogService applicationDialogService)
        {
            _application = application;
            _applicationDialogService = applicationDialogService;

            _random = _pseudoRandomNumbers
                ? new Random(0)
                : new Random((int)DateTime.UtcNow.Ticks);

            LogViewModel = new LogViewModel();
            _viewModelLogger = new ViewModelLogger(_application.Logger, LogViewModel);
            LoggingActive = true;

            PlayerViewModels = new ObservableCollection<PlayerViewModel>();

            _application.Logger?.WriteLine(LogMessageCategory.Debug, "Race Game - starting up");
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

                    UpdateScore(gameEvent.PlayerIndex);

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

        private void OpenSettingsDialog(
            object owner)
        {
            var dialogViewModel = new SettingsDialogViewModel();

            _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
        }

        private void StartGame()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            if (!int.TryParse(settings["PlayerCount"]?.Value, out var playerCount))
            {
                throw new InvalidDataException("Invalid Config file");
            }

            // Create engine
            var tempArray = Enumerable.Repeat(true, playerCount).ToArray();
            var indexOfPlayer = _random.Next(0, playerCount);
            tempArray[indexOfPlayer] = false;
            _application.Engine = new Engine(tempArray, _pseudoRandomNumbers);

            PlayerViewModels.Clear();

            Enumerable
                .Range(0, _application.Engine.PlayerCount)
                .ToList()
                .ForEach(playerIndex =>
                {
                    var description = playerIndex == indexOfPlayer ? "you" : "computer";
                    var name = $"Player {playerIndex + 1} ({description})";

                    PlayerViewModels.Add(new PlayerViewModel { Name = name, Score = 0 });
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

            UpdateScore(gameEvent.PlayerIndex);

            if (_application.Engine.GameDecided)
            {
                GameResultMessage = "Congratulations - You Win";
                GameInProgress = _application.Engine.GameInProgress;
                GameDecided = _application.Engine.GameDecided;
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

        private bool CanRollDie()
        {
            return GameInProgress && PlayerHasInitiative;
        }

        private void SyncControlsWithApplication()
        {
            GameInProgress = _application.Engine.GameInProgress;
            GameDecided = _application.Engine.GameDecided;
            PlayerHasInitiative = !_application.Engine.NextEventOccursAutomatically;
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
        }

        private void UpdateScore(
            int playerIndex)
        {
            PlayerViewModels[playerIndex].Score =
                _application.Engine.PlayerScores[playerIndex];
        }
    }
}