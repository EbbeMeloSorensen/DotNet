using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Text;
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
        private Dictionary<int, string> _territoryNameMap;

        private Window _owner;
        private readonly IDialogService _applicationDialogService;
        private const bool _pseudoRandomNumbers = true;
        private readonly Random _random;
        private const int _delay = 0;
        private IGraph<LabelledVertex, EmptyEdge> _graphOfTerritories;
        private List<Continent> _continents;
        private Dictionary<int, Brush> _colorPalette;
        private PointD _selectedVertexCanvasPosition;
        private PointD _selectedTargetVertexCanvasPosition;
        private bool _activeTerritoryHighlighted;
        private bool _attackVectorVisible;

        private ViewModelLogger _viewModelLogger;
        private bool _loggingActive;
        private readonly Application.Application _application;

        private bool _playerHasInitiative;
        private bool _gameInProgress;
        private bool _gameDecided;
        private string _gameResultMessage;
        private int? _indexOfActiveTerritory;
        private int? _indexOfTargetTerritory;
        private int[] _indexesOfHostileNeighbours;
        private bool _displayAttackVector;
        private int _armiesToDeploy;

        private RelayCommand<object> _openSettingsDialogCommand;
        private AsyncCommand _startGameCommand;
        private AsyncCommand _reinforceCommand;
        private AsyncCommand _deployCommand;
        private AsyncCommand _attackCommand;
        private AsyncCommand _passCommand;

        public RelayCommand<object> OpenSettingsDialogCommand =>
            _openSettingsDialogCommand ??= new RelayCommand<object>(OpenSettingsDialog);

        public AsyncCommand StartGameCommand => _startGameCommand ??= new AsyncCommand(
            () =>
            {
                StartGame();
                return Proceed();
            }, CanStartGame);

        public AsyncCommand ReinforceCommand => _reinforceCommand ??= new AsyncCommand(Reinforce, CanReinforce);
        public AsyncCommand DeployCommand => _deployCommand ??= new AsyncCommand(Deploy, CanDeploy);
        public AsyncCommand AttackCommand => _attackCommand ??= new AsyncCommand(Attack, CanAttack);
        public AsyncCommand PassCommand => _passCommand ??= new AsyncCommand(Pass, CanPass);

        public GraphViewModel MapViewModel { get; }
        public ObservableCollection<TerritoryLabelViewModel> TerritoryLabelViewModels { get; }

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

        public bool DisplayAttackVector
        {
            get => _displayAttackVector;
            set
            {
                _displayAttackVector = value;
                RaisePropertyChanged();
            }
        }

        public int ArmiesToDeploy
        {
            get => _armiesToDeploy;
            set
            {
                _armiesToDeploy = value;
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

        public PointD SelectedVertexCanvasPosition
        {
            get => _selectedVertexCanvasPosition;
            set
            {
                _selectedVertexCanvasPosition = value;
                RaisePropertyChanged();
            }
        }

        public PointD SelectedTargetVertexCanvasPosition
        {
            get => _selectedTargetVertexCanvasPosition;
            set
            {
                _selectedTargetVertexCanvasPosition = value;
                RaisePropertyChanged();
            }
        }

        public bool ActiveTerritoryHighlighted
        {
            get => _activeTerritoryHighlighted;
            set
            {
                _activeTerritoryHighlighted = value;
                RaisePropertyChanged();
            }
        }

        public bool AttackVectorVisible
        {
            get => _attackVectorVisible;
            set
            {
                _attackVectorVisible = value;
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

            var maxLineCount = 50;
            LogViewModel = new LogViewModel(maxLineCount);
            _viewModelLogger = new ViewModelLogger(_application.Logger, LogViewModel);
            LoggingActive = true;
            DisplayAttackVector = true;
            PlayerViewModels = new ObservableCollection<PlayerViewModel>();

            _continents = GenerateContinents();
            _colorPalette = GenerateColorPalette();
            _graphOfTerritories = GenerateGraphOfTerritories();

            _territoryNameMap = _graphOfTerritories.Vertices.ToDictionary(
                _ => _.Id,
                _ => _.Label);

            _indexesOfHostileNeighbours = new int[]{};
            
            MapViewModel = new GraphViewModel(_graphOfTerritories, 1100, 500)
            {
                AllowMovingVertices = false
            };

            ArrangeMapVertices(MapViewModel);

            TerritoryLabelViewModels = new ObservableCollection<TerritoryLabelViewModel>(
                MapViewModel.PointViewModels.Select(_ => new TerritoryLabelViewModel
                {
                    Point = _.Point + new PointD(5, -30),
                    Text = _.Label
                }));

            MapViewModel.PointViewModels.ToList().ForEach(_ => _.Label = "");

            MapViewModel.VertexClicked += (s, e) =>
            {
                if (!PlayerHasInitiative)
                {
                    return;
                }

                var territoryId = e.ElementId;

                if (ArmiesToDeploy == 0 && _indexesOfHostileNeighbours.Contains(territoryId))
                {
                    SelectedTargetVertexCanvasPosition = MapViewModel.PointViewModels[territoryId].Point - new PointD(20, 20);
                    _indexOfTargetTerritory = territoryId;
                    AttackVectorVisible = true;
                    UpdateCommandAvailability();
                    return;
                }
                
                if (_application.Engine.CurrentPlayerIndex !=
                    _application.Engine.GetTerritoryStatus(territoryId).ControllingPlayerIndex)
                {
                    return;
                }

                _indexOfActiveTerritory = territoryId;
                _indexOfTargetTerritory = null;
                SelectedVertexCanvasPosition = MapViewModel.PointViewModels[territoryId].Point - new PointD(20, 20);
                ActiveTerritoryHighlighted = true;
                AttackVectorVisible = false;

                _indexesOfHostileNeighbours = _application.Engine
                    .IndexesOfHostileNeighbourTerritories(territoryId)
                    .ToArray();

                UpdateCommandAvailability();
            };

            _application.Logger?.WriteLine(LogMessageCategory.Information, "Risk Game - starting up");
        }

        private async Task Proceed()
        {
            while (!_application.Engine.GameDecided)
            {
                if (_application.Engine.NextEventOccursAutomatically)
                {
                    var gameEvent = await _application.Engine.ExecuteNextEvent();

                    switch (gameEvent)
                    {
                        case PlayerAttacks playerAttacks:
                        {
                            if (DisplayAttackVector)
                            {
                                var point1 = MapViewModel.PointViewModels[playerAttacks.Vertex1].Point;
                                var point2 = MapViewModel.PointViewModels[playerAttacks.Vertex2].Point;

                                SelectedVertexCanvasPosition = point1 - new PointD(20, 20);
                                SelectedTargetVertexCanvasPosition = point2 - new PointD(20, 20);
                                ActiveTerritoryHighlighted = true;
                                AttackVectorVisible = true;
                            }

                            break;
                        }
                        case PlayerReinforces:
                        case PlayerDeploysArmies:
                        case PlayerTransfersArmies:
                        {
                            ActiveTerritoryHighlighted = false;
                            AttackVectorVisible = false;
                            break;
                        }
                        case PlayerPasses playerPasses:
                        {
                            ActiveTerritoryHighlighted = false;
                            AttackVectorVisible = false;
                            break;
                        }
                    }

                    LogGameEvent(gameEvent);

                    await Delay(_delay);

                    // Måske lidt overkill - prøv bare at opdatere de 2 vertices, der er i spil
                    SyncControlsWithApplication();

                    await Delay(_delay);

                    if (gameEvent.TurnGoesToNextPlayer)
                    {
                        SwitchToNextPlayer();
                    }

                    continue;
                }

                UpdateCommandAvailability();
                break;
            }

            if (_application.Engine.GameDecided)
            {
                ActiveTerritoryHighlighted = false;
                AttackVectorVisible = false;
                GameResultMessage = $"Game Over\nPlayer {_application.Engine.CurrentPlayerIndex + 1} Wins";
                GameInProgress = _application.Engine.GameInProgress;
                GameDecided = _application.Engine.GameDecided;

                _application.Logger?.WriteLine(
                    LogMessageCategory.Information,
                    $"Game decided. Player {_application.Engine.CurrentPlayerIndex + 1} wins");
            }
            else
            {
                PlayerHasInitiative = true;
            }

            UpdateCommandAvailability();

            // Diagnotics: Make the game fully automatic by making the player pass
            if (PlayerHasInitiative && false)
            {
                await Pass();
            }
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
            _application.Engine = new Engine(tempArray, _pseudoRandomNumbers, _graphOfTerritories);
            _application.Engine.Initialize(_continents);

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
                        Brush = _colorPalette[playerIndex]
                    });
                });

            _application.Engine.StartGame();

            SyncControlsWithApplication();
            HighlightCurrentPlayer();
            UpdateCommandAvailability();

            _application.Logger?.WriteLine(
                LogMessageCategory.Information,
                "New game started");
        }

        private bool CanStartGame()
        {
            return !GameInProgress || PlayerHasInitiative;
        }

        private async Task Reinforce()
        {
            var gameEvent = await _application.Engine.PlayerSelectsOption(
                new Reinforce());

            ArmiesToDeploy = _application.Engine.ExtraArmiesForCurrentPlayer;
            ActiveTerritoryHighlighted = false;
            AttackVectorVisible = false;
            _indexOfActiveTerritory = null;
            _indexOfTargetTerritory = null;

            SyncControlsWithApplication();
            UpdateCommandAvailability();
            LogGameEvent(gameEvent);
        }

        private bool CanReinforce()
        {
            return GameInProgress &&
                   PlayerHasInitiative &&
                   _application.Engine.CurrentPlayerMayReinforce &&
                   _application.Engine.ExtraArmiesForCurrentPlayer == 0;
        }

        private async Task Deploy()
        {
            if (!_indexOfActiveTerritory.HasValue)
            {
                throw new InvalidOperationException("Deploy called without having selected a territory - should not be possible");
            }

            var gameEvent = await _application.Engine.PlayerSelectsOption(
                new Deploy
                {
                    ActiveTerritoryIndex = _indexOfActiveTerritory.Value,
                });

            ArmiesToDeploy = _application.Engine.ExtraArmiesForCurrentPlayer;

            SyncControlsWithApplication();
            UpdateCommandAvailability();
            LogGameEvent(gameEvent);
        }

        private bool CanDeploy()
        {
            return GameInProgress &&
                   PlayerHasInitiative &&
                   _indexOfActiveTerritory.HasValue &&
                   ArmiesToDeploy > 0;
        }

        private async Task Attack()
        {
            if (!_indexOfActiveTerritory.HasValue || !_indexOfTargetTerritory.HasValue)
            {
                throw new InvalidOperationException("Deploy called without having selected two territories - should not be possible");
            }

            var gameEvent = await _application.Engine.PlayerSelectsOption(
                new Attack
                {
                    ActiveTerritoryIndex = _indexOfActiveTerritory.Value,
                    TargetTerritoryIndex = _indexOfTargetTerritory.Value
                });

            SyncControlsWithApplication();
            UpdateCommandAvailability();
            LogGameEvent(gameEvent);

            var playerAttacks = gameEvent as PlayerAttacks;

            if (playerAttacks.TerritoryConquered)
            {
                var a = _application.Engine.GetTerritoryStatus(_indexOfActiveTerritory.Value).Armies;
                var b = _application.Engine.GetTerritoryStatus(_indexOfTargetTerritory.Value).Armies;

                var dialog = new TransferArmiesDialogViewModel(playerAttacks.DiceRolledByAttacker, a + b - 1);

                _applicationDialogService.ShowDialog(dialog, null);

                var c = dialog.ArmiesToTransfer;
                var d = 0;
            }
        }

        private bool CanAttack()
        {
            return GameInProgress && 
                   PlayerHasInitiative &&
                   ArmiesToDeploy == 0 &&
                   _indexOfActiveTerritory.HasValue &&
                   _indexOfTargetTerritory.HasValue &&
                   _application.Engine.GetTerritoryStatus(_indexOfActiveTerritory.Value).Armies > 1 &&
                   _application.Engine.GetTerritoryStatus(_indexOfTargetTerritory.Value).ControllingPlayerIndex != _application.Engine.CurrentPlayerIndex;
        }

        private async Task Pass()
        {
            var gameEvent = await _application.Engine.PlayerSelectsOption(new Pass());
            LogGameEvent(gameEvent);

            _indexOfActiveTerritory = null;
            _indexOfTargetTerritory = null;
            _indexesOfHostileNeighbours = new int[] { };

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
                SwitchToNextPlayer();
                UpdateCommandAvailability();
                await Proceed();
            }
        }

        private bool CanPass()
        {
            return GameInProgress &&
                   PlayerHasInitiative &&
                   ArmiesToDeploy == 0;
        }

        private void SyncControlsWithApplication()
        {
            GameInProgress = _application.Engine.GameInProgress;
            GameDecided = _application.Engine.GameDecided;
            PlayerHasInitiative = !_application.Engine.NextEventOccursAutomatically;

            // Colors and numbers on map
            _graphOfTerritories.Vertices.ForEach(_ =>
            {
                var territoryStatus = _application.Engine.GetTerritoryStatus(_.Id);
                MapViewModel.StylePoint(
                    _.Id, 
                    _colorPalette[territoryStatus.ControllingPlayerIndex],
                    territoryStatus.Armies.ToString());
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
            ReinforceCommand.RaiseCanExecuteChanged();
            DeployCommand.RaiseCanExecuteChanged();
            AttackCommand.RaiseCanExecuteChanged();
            PassCommand.RaiseCanExecuteChanged();
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
            graph.AddEdge(0, 29);
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
            graphViewModel.PlacePoint(1, new PointD(150, 100));
            graphViewModel.PlacePoint(2, new PointD(300, 100));
            graphViewModel.PlacePoint(3, new PointD(100, 150));
            graphViewModel.PlacePoint(4, new PointD(200, 150));
            graphViewModel.PlacePoint(5, new PointD(300, 150));
            graphViewModel.PlacePoint(6, new PointD(100, 200));
            graphViewModel.PlacePoint(7, new PointD(250, 200));
            graphViewModel.PlacePoint(8, new PointD(175, 250));

            // South America
            graphViewModel.PlacePoint(9, new PointD(175, 350));
            graphViewModel.PlacePoint(10, new PointD(125, 400));
            graphViewModel.PlacePoint(11, new PointD(175, 450));
            graphViewModel.PlacePoint(12, new PointD(225, 400));

            // Europe
            graphViewModel.PlacePoint(13, new PointD(400, 100));
            graphViewModel.PlacePoint(14, new PointD(500, 100));
            graphViewModel.PlacePoint(15, new PointD(450, 150));
            graphViewModel.PlacePoint(16, new PointD(500, 200));
            graphViewModel.PlacePoint(17, new PointD(550, 150));
            graphViewModel.PlacePoint(18, new PointD(450, 250));
            graphViewModel.PlacePoint(19, new PointD(550, 250));

            // Africa
            graphViewModel.PlacePoint(20, new PointD(450, 350));
            graphViewModel.PlacePoint(21, new PointD(550, 350));
            graphViewModel.PlacePoint(22, new PointD(600, 400));
            graphViewModel.PlacePoint(23, new PointD(500, 400));
            graphViewModel.PlacePoint(24, new PointD(500, 450));
            graphViewModel.PlacePoint(25, new PointD(600, 450));

            // Asia
            graphViewModel.PlacePoint(26, new PointD(825, 150));
            graphViewModel.PlacePoint(27, new PointD(725, 150));
            graphViewModel.PlacePoint(28, new PointD(875, 100));
            graphViewModel.PlacePoint(29, new PointD(1025, 50));
            graphViewModel.PlacePoint(30, new PointD(925, 150));
            graphViewModel.PlacePoint(31, new PointD(725, 225));
            graphViewModel.PlacePoint(32, new PointD(925, 225));
            graphViewModel.PlacePoint(33, new PointD(1025, 225));
            graphViewModel.PlacePoint(34, new PointD(825, 225));
            graphViewModel.PlacePoint(35, new PointD(675, 300));
            graphViewModel.PlacePoint(36, new PointD(775, 300));
            graphViewModel.PlacePoint(37, new PointD(875, 300));

            // Oceania
            graphViewModel.PlacePoint(38, new PointD(875, 400));
            graphViewModel.PlacePoint(39, new PointD(975, 400));
            graphViewModel.PlacePoint(40, new PointD(875, 450));
            graphViewModel.PlacePoint(41, new PointD(975, 450));
        }

        private async Task Delay(
            int milliSeconds)
        {
            if (milliSeconds > 0)
            {
                await Task.Delay(milliSeconds);
            }
        }

        private void LogGameEvent(
            IGameEvent gameEvent)
        {
            if (!LoggingActive)
            {
                return;
            }

            var playerId = gameEvent.PlayerIndex + 1;
            var sb = new StringBuilder($"  Player {playerId}");

            switch (gameEvent)
            {
                case PlayerAttacks playerAttacks:
                {
                    sb.Append($" attacks {_territoryNameMap[playerAttacks.Vertex2]}");
                    sb.Append($" from {_territoryNameMap[playerAttacks.Vertex1]}. Outcome: ");

                    if (playerAttacks.CasualtiesAttacker > 0)
                    {
                        sb.Append($"Player {playerId} looses {playerAttacks.CasualtiesAttacker} ");
                        sb.Append(playerAttacks.CasualtiesAttacker == 1 ? "army" : "armies");
                    }

                    if (playerAttacks.CasualtiesAttacker > 0 && playerAttacks.CasualtiesDefender > 0)
                    {
                        sb.Append(", and ");
                    }

                    if (playerAttacks.CasualtiesDefender > 0)
                    {
                        sb.Append($"Defending player (Player {playerAttacks.DefendingPlayerIndex + 1}) looses {playerAttacks.CasualtiesDefender} ");
                        sb.Append(playerAttacks.CasualtiesDefender == 1 ? "army" : "armies");
                    }

                    if (playerAttacks.TerritoryConquered)
                    {
                        sb.Append(". Territory is conquered");
                    }

                    break;
                }
                case PlayerReinforces playerReinforces:
                {
                    sb.Append($" reinforces and gets {_application.Engine.ExtraArmiesForCurrentPlayer} extra armies");

                    break;
                }
                case PlayerDeploysArmies playerDeploysArmies:
                {
                    sb.Append(" deploys ");
                    
                    if (playerDeploysArmies.TerritoryIndexes.Count == 1)
                    {
                        sb.Append(" an army");
                    }
                    else
                    {
                        sb.Append($"{playerDeploysArmies.TerritoryIndexes.Count} armies");
                    }

                    sb.Append(" in: ");

                    sb.Append(playerDeploysArmies.TerritoryIndexes
                        .Select(_ => _territoryNameMap[_])
                        .Aggregate((c, n) => $"{c}, {n}"));

                    break;
                }
                case PlayerPasses:
                {
                    sb.Append(" passes");
                    break;
                }
                case PlayerTransfersArmies playerTransfersArmies:
                {
                    sb.Append($" transfers {playerTransfersArmies.ArmiesTransfered} armies");
                    sb.Append($" from {_territoryNameMap[playerTransfersArmies.Vertex1]}");
                    sb.Append($" to {_territoryNameMap[playerTransfersArmies.Vertex2]}");
                    break;
                }
            }

            _application.Logger?.WriteLine(
                LogMessageCategory.Information,
                sb.ToString());
        }

        private void SwitchToNextPlayer()
        {
            HighlightCurrentPlayer();

            _application.Logger?.WriteLine(
                LogMessageCategory.Information,
                $"Turn goes to Player {_application.Engine.CurrentPlayerIndex + 1}");

            var continents =  _application.Engine.AssignExtraArmiesForControlledContinents();

            if (!continents.Any())
            {
                return;
            }

            if (PlayerHasInitiative)
            {
                ArmiesToDeploy = _application.Engine.ExtraArmiesForCurrentPlayer;
            }

            var sb = new StringBuilder($"  Player {_application.Engine.CurrentPlayerIndex + 1}");
            sb.Append($" gets {_application.Engine.ExtraArmiesForCurrentPlayer} extra armies");
            sb.Append(" for entirely controlling the continent");
            
            if (continents.Count() > 1)
            {   
                sb.Append("s");
            }

            sb.Append($": {continents.Aggregate((c, n) => $"{c}, {n}")}");

            _application.Logger?.WriteLine(
                LogMessageCategory.Information,
                sb.ToString());
        }

        private List<Continent> GenerateContinents()
        {
            return new List<Continent>
            {
                new Continent
                {
                    Name = "North America",
                    Territories = Enumerable.Range(0, 9).ToArray(),
                    ExtraArmies = 5
                },
                new Continent
                {
                    Name = "South America",
                    Territories = Enumerable.Range(9, 4).ToArray(),
                    ExtraArmies = 2
                },
                new Continent
                {
                    Name = "Europe",
                    Territories = Enumerable.Range(13, 7).ToArray(),
                    ExtraArmies = 5
                },
                new Continent
                {
                    Name = "Africa",
                    Territories = Enumerable.Range(20, 6).ToArray(),
                    ExtraArmies = 3
                },
                new Continent
                {
                    Name = "Asia",
                    Territories = Enumerable.Range(26, 12).ToArray(),
                    ExtraArmies = 7
                },
                new Continent
                {
                    Name = "Oceania",
                    Territories = Enumerable.Range(38, 4).ToArray(),
                    ExtraArmies = 2
                }
            };
        }

        private Dictionary<int, Brush> GenerateColorPalette()
        {
            return new Dictionary<int, Brush>
            {
                {0, new SolidColorBrush(Colors.IndianRed)},
                {1, new SolidColorBrush(Colors.CornflowerBlue)},
                {2, new SolidColorBrush(Colors.LightGreen)},
                {3, new SolidColorBrush(Colors.Yellow)},
                {4, new SolidColorBrush(Colors.Orange)},
                {5, new SolidColorBrush(Colors.MediumPurple)}
            };
        }
    }
}