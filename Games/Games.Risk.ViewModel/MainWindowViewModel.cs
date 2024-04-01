using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.Logging;
using Craft.DataStructures.Graph;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using Craft.ViewModels.Graph;
using Games.Risk.Application;
using Games.Risk.Application.GameEvents;
using Games.Risk.Application.PlayerOptions;

namespace Games.Risk.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IDialogService _applicationDialogService;
        private const bool _pseudoRandomNumbers = true;
        private readonly Random _random;
        private const int _delay = 200;
        private IGraph<LabelledVertex, EmptyEdge> _graphOfTerritories;
        private Dictionary<int, Brush> _colorPalette;

        private ViewModelLogger _viewModelLogger;
        private bool _loggingActive;
        private readonly Application.Application _application;

        private bool _playerHasInitiative;
        private bool _gameInProgress;
        private bool _gameDecided;
        private string _gameResultMessage;
        private int _pot;

        private RelayCommand<object> _openSettingsDialogCommand;
        private AsyncCommand _startGameCommand;
        private AsyncCommand _rollDieCommand;
        private AsyncCommand _takePotCommand;

        public RelayCommand<object> OpenSettingsDialogCommand =>
            _openSettingsDialogCommand ??= new RelayCommand<object>(OpenSettingsDialog);

        public AsyncCommand StartGameCommand => _startGameCommand ??= new AsyncCommand(
            () =>
            {
                StartGame();
                return Proceed();
            }, CanStartGame);

        public AsyncCommand RollDieCommand => _rollDieCommand ??= new AsyncCommand(RollDie, CanRollDie);
        public AsyncCommand TakePotCommand => _takePotCommand ??= new AsyncCommand(TakePot, CanTakePot);

        public GraphViewModel MapViewModel { get; }
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

            _colorPalette = new Dictionary<int, Brush>
            {
                {0, new SolidColorBrush(Colors.Red)},
                {1, new SolidColorBrush(Colors.Blue)},
                {2, new SolidColorBrush(Colors.Green)},
                {3, new SolidColorBrush(Colors.Yellow)},
                {4, new SolidColorBrush(Colors.DarkOrange)},
                {5, new SolidColorBrush(Colors.MediumPurple)}
            };

            _graphOfTerritories = GenerateGraphOfTerritories();
            MapViewModel = new GraphViewModel(_graphOfTerritories, 1100, 450);
            ArrangeMapVertices(MapViewModel);

            _application.Logger?.WriteLine(LogMessageCategory.Debug, "Risk Game - starting up");
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
            _application.Engine = new Engine(tempArray, false, _graphOfTerritories);

            PlayerViewModels.Clear();

            Enumerable
                .Range(0, _application.Engine.PlayerCount)
                .ToList()
                .ForEach(playerIndex =>
                {
                    var description = playerIndex == indexOfPlayer ? "you" : "computer";
                    var name = $"Player {playerIndex + 1} ({description})";

                    PlayerViewModels.Add(new PlayerViewModel
                    {
                        Name = name,
                        Brush = _colorPalette[playerIndex],
                        Score = 0
                    });
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
            return _application.Engine != null &&
                   _application.Engine.Pot > 0 &&
                   GameInProgress &&
                   PlayerHasInitiative;
        }

        private void SyncControlsWithApplication()
        {
            GameInProgress = _application.Engine.GameInProgress;
            GameDecided = _application.Engine.GameDecided;
            PlayerHasInitiative = !_application.Engine.NextEventOccursAutomatically;
            Pot = _application.Engine.Pot;

            // Colors on map
            _graphOfTerritories.Vertices.ForEach(_ =>
            {
                var playerId = _application.Engine.IdOfPlayerCurrentlyControllingTerritory(_.Id);

                MapViewModel.AssignBrushToPoint(_.Id, _colorPalette[playerId]);
            });
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

        private static IGraph<LabelledVertex, EmptyEdge> GenerateGraphOfTerritories()
        {
            var vertices = new List<LabelledVertex>
            {
                // North America
                new LabelledVertex("Alaska"),                //  0
                new LabelledVertex("Northwest Territory"),   //  1
                new LabelledVertex("Greenland"),             //  2
                new LabelledVertex("Alberta"),               //  3
                new LabelledVertex("Ontario"),               //  4
                new LabelledVertex("Quebec"),                //  5
                new LabelledVertex("Western United States"), //  6
                new LabelledVertex("Eastern United States"), //  7
                new LabelledVertex("Central America"),       //  8

                // South America
                new LabelledVertex("Venezuela"),   //  9
                new LabelledVertex("Peru"),        // 10
                new LabelledVertex("Argentina"),   // 11
                new LabelledVertex("Brazil"),      // 12

                // Europe
                new LabelledVertex("Iceland"),         // 13
                new LabelledVertex("Scandinavia"),     // 14
                new LabelledVertex("Great Britain"),   // 15
                new LabelledVertex("Northern Europe"), // 16
                new LabelledVertex("Ukraine"),         // 17
                new LabelledVertex("Western Europe"),  // 18
                new LabelledVertex("Southern Europe"), // 19

                // Africa
                new LabelledVertex("North Africa"), // 20
                new LabelledVertex("Egypt"),        // 21
                new LabelledVertex("East Africa"),  // 22
                new LabelledVertex("Congo"),        // 23
                new LabelledVertex("South Africa"), // 24
                new LabelledVertex("Madagascar"),   // 25

                // Asia
                new LabelledVertex("Siberia"),     // 26
                new LabelledVertex("Ural"),        // 27
                new LabelledVertex("Yakutsk"),     // 28
                new LabelledVertex("Kamchatka"),   // 29
                new LabelledVertex("Irkutsk"),     // 30
                new LabelledVertex("Afghanistan"), // 31
                new LabelledVertex("Mongolia"),    // 32
                new LabelledVertex("Japan"),       // 33
                new LabelledVertex("China"),       // 34
                new LabelledVertex("Middle East"), // 35
                new LabelledVertex("India"),       // 36
                new LabelledVertex("Siam"),        // 37

                // Oceania
                new LabelledVertex("Indonesia"),         // 38
                new LabelledVertex("New Guinea"),        // 39
                new LabelledVertex("Western Australia"), // 40
                new LabelledVertex("Eastern Australia"), // 41
            };

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, false);

            graph.AddEdge(0, 1);
            graph.AddEdge(0, 3);
            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(1, 4);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 5);
            graph.AddEdge(2, 13);
            graph.AddEdge(3, 4);
            graph.AddEdge(3, 6);
            graph.AddEdge(4, 5);
            graph.AddEdge(4, 6);
            graph.AddEdge(4, 7);
            graph.AddEdge(5, 7);
            graph.AddEdge(6, 7);
            graph.AddEdge(6, 8);
            graph.AddEdge(7, 8);
            graph.AddEdge(8, 9);
            graph.AddEdge(9, 10);
            graph.AddEdge(9, 12);
            graph.AddEdge(10, 11);
            graph.AddEdge(10, 12);
            graph.AddEdge(11, 12);
            graph.AddEdge(12, 20);
            graph.AddEdge(13, 14);
            graph.AddEdge(13, 15);
            graph.AddEdge(14, 15);
            graph.AddEdge(14, 16);
            graph.AddEdge(14, 17);
            graph.AddEdge(15, 16);
            graph.AddEdge(15, 18);
            graph.AddEdge(16, 17);
            graph.AddEdge(16, 18);
            graph.AddEdge(16, 19);
            graph.AddEdge(17, 19);
            graph.AddEdge(18, 19);
            graph.AddEdge(18, 20);
            graph.AddEdge(19, 20);
            graph.AddEdge(19, 21);
            graph.AddEdge(19, 35);
            graph.AddEdge(17, 27);
            graph.AddEdge(17, 31);
            graph.AddEdge(17, 35);
            graph.AddEdge(20, 21);
            graph.AddEdge(20, 22);
            graph.AddEdge(20, 23);
            graph.AddEdge(21, 22);
            graph.AddEdge(21, 35);
            graph.AddEdge(22, 23);
            graph.AddEdge(22, 24);
            graph.AddEdge(22, 25);
            graph.AddEdge(22, 35);
            graph.AddEdge(23, 24);
            graph.AddEdge(24, 25);
            graph.AddEdge(26, 27);
            graph.AddEdge(26, 28);
            graph.AddEdge(26, 30);
            graph.AddEdge(26, 32);
            graph.AddEdge(26, 34);
            graph.AddEdge(27, 31);
            graph.AddEdge(27, 34);
            graph.AddEdge(28, 29);
            graph.AddEdge(28, 30);
            graph.AddEdge(29, 30);
            graph.AddEdge(29, 32);
            graph.AddEdge(29, 33);
            graph.AddEdge(30, 32);
            graph.AddEdge(31, 34);
            graph.AddEdge(31, 35);
            graph.AddEdge(31, 36);
            graph.AddEdge(32, 33);
            graph.AddEdge(32, 34);
            graph.AddEdge(34, 36);
            graph.AddEdge(34, 37);
            graph.AddEdge(35, 36);
            graph.AddEdge(36, 37);
            graph.AddEdge(37, 38);
            graph.AddEdge(38, 39);
            graph.AddEdge(38, 40);
            graph.AddEdge(39, 40);
            graph.AddEdge(39, 41);
            graph.AddEdge(40, 41);

            return graph;
        }

        private static void ArrangeMapVertices(
            GraphViewModel graphViewModel)
        {
            // North America
            graphViewModel.PlacePoint(0, new PointD(50, 50));
            graphViewModel.PlacePoint(1, new PointD(150, 50));
            graphViewModel.PlacePoint(2, new PointD(300, 50));
            graphViewModel.PlacePoint(3, new PointD(100, 100));
            graphViewModel.PlacePoint(4, new PointD(200, 100));
            graphViewModel.PlacePoint(5, new PointD(300, 100));
            graphViewModel.PlacePoint(6, new PointD(100, 150));
            graphViewModel.PlacePoint(7, new PointD(250, 150));
            graphViewModel.PlacePoint(8, new PointD(175, 200));

            // South America
            graphViewModel.PlacePoint(9, new PointD(175, 300));
            graphViewModel.PlacePoint(10, new PointD(125, 350));
            graphViewModel.PlacePoint(11, new PointD(175, 400));
            graphViewModel.PlacePoint(12, new PointD(225, 350));

            // Europe
            graphViewModel.PlacePoint(13, new PointD(400, 50));
            graphViewModel.PlacePoint(14, new PointD(500, 50));
            graphViewModel.PlacePoint(15, new PointD(450, 100));
            graphViewModel.PlacePoint(16, new PointD(500, 150));
            graphViewModel.PlacePoint(17, new PointD(550, 100));
            graphViewModel.PlacePoint(18, new PointD(450, 200));
            graphViewModel.PlacePoint(19, new PointD(550, 200));

            // Africa
            graphViewModel.PlacePoint(20, new PointD(450, 300));
            graphViewModel.PlacePoint(21, new PointD(550, 300));
            graphViewModel.PlacePoint(22, new PointD(600, 350));
            graphViewModel.PlacePoint(23, new PointD(500, 350));
            graphViewModel.PlacePoint(24, new PointD(500, 400));
            graphViewModel.PlacePoint(25, new PointD(600, 400));

            // Asia
            graphViewModel.PlacePoint(26, new PointD(900, 100));
            graphViewModel.PlacePoint(27, new PointD(800, 100));
            graphViewModel.PlacePoint(28, new PointD(1000, 100));
            graphViewModel.PlacePoint(29, new PointD(1000, 150));
            graphViewModel.PlacePoint(30, new PointD(900, 150));
            graphViewModel.PlacePoint(31, new PointD(725, 175));
            graphViewModel.PlacePoint(32, new PointD(900, 200));
            graphViewModel.PlacePoint(33, new PointD(1000, 200));
            graphViewModel.PlacePoint(34, new PointD(825, 175));
            graphViewModel.PlacePoint(35, new PointD(675, 250));
            graphViewModel.PlacePoint(36, new PointD(775, 250));
            graphViewModel.PlacePoint(37, new PointD(850, 250));

            // Oceania
            graphViewModel.PlacePoint(38, new PointD(850, 350));
            graphViewModel.PlacePoint(39, new PointD(950, 350));
            graphViewModel.PlacePoint(40, new PointD(850, 400));
            graphViewModel.PlacePoint(41, new PointD(950, 400));
        }
    }
}