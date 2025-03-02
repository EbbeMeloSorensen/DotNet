﻿using System;
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
using Craft.Utils.Linq;
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

        private readonly IDialogService _applicationDialogService;
        private const bool _pseudoRandomNumbers = false;
        private readonly Random _random;
        private int _playerCount;
        private int _humanPlayerCount;
        private int _delay;
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

        private bool _gameInProgress;
        private bool _gameDecided;
        private string _gameResultMessage;
        private int? _indexOfActiveTerritory;
        private int? _indexOfTargetTerritory;
        private int[] _indexesOfHostileNeighbours;
        private int[] _indexesOfReachableTerritories;
        private string _selectedDeployOption;
        private List<Card> _selectedCards;
        private bool _currentPlayerCanTradeInSelectedCards;
        private bool _tradingCardsAfterDefeatingOpponent;
        private int?[] _activeTerritoryDuringSetupPhase;
        private int[] _repetitions;

        private RelayCommand<object> _openSettingsDialogCommand;
        private AsyncCommand _startGameCommand;
        private AsyncCommand _tradeInSelectedCardsCommand;
        private AsyncCommand _reinforceCommand;
        private AsyncCommand _deployCommand;
        private AsyncCommand _attackCommand;
        private AsyncCommand _moveCommand;
        private AsyncCommand _passCommand;

        public string SelectedDeployOption
        {
            get => _selectedDeployOption;
            set
            {
                _selectedDeployOption = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<object> OpenSettingsDialogCommand =>
            _openSettingsDialogCommand ??= new RelayCommand<object>(OpenSettingsDialog);

        public AsyncCommand StartGameCommand => _startGameCommand ??= new AsyncCommand(
            async () =>
            {
                await StartGame();
                await Proceed();
            }, CanStartGame);

        public AsyncCommand TradeInSelectedCardsCommand => _tradeInSelectedCardsCommand ??= new AsyncCommand(TradeInSelectedCards, CanTradeInSelectedCards);
        public AsyncCommand ReinforceCommand => _reinforceCommand ??= new AsyncCommand(Reinforce, CanReinforce);
        public AsyncCommand DeployCommand => _deployCommand ??= new AsyncCommand(Deploy, CanDeploy);
        public AsyncCommand AttackCommand => _attackCommand ??= new AsyncCommand(Attack, CanAttack);
        public AsyncCommand MoveCommand => _moveCommand ??= new AsyncCommand(Move, CanMove);
        public AsyncCommand PassCommand => _passCommand ??= new AsyncCommand(Pass, CanPass);

        public GraphViewModel MapViewModel { get; }
        public ObservableCollection<TerritoryLabelViewModel> TerritoryLabelViewModels { get; }

        public LogViewModel LogViewModel { get; }
        public ObservableCollection<PlayerViewModel> PlayerViewModels { get; }

        public ObservableCollection<string> DeployOptions { get; }

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
            PlayerViewModels = new ObservableCollection<PlayerViewModel>();

            _continents = GenerateContinents();
            _colorPalette = GenerateColorPalette();
            _graphOfTerritories = GenerateGraphOfTerritories();

            _territoryNameMap = _graphOfTerritories.Vertices.ToDictionary(
                _ => _.Id,
                _ => _.Label);

            _indexesOfHostileNeighbours = new int[] { };
            _indexesOfReachableTerritories = new int[] {};
            
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

            DeployOptions = new ObservableCollection<string> { "1", "2", "5", "10", "All" };
            SelectedDeployOption = "1";

            MapViewModel.VertexClicked += (s, e) =>
            {
                if (_application.Engine.CurrentPlayerIsAutomatic)
                {
                    return;
                }

                var territoryId = e.ElementId;

                if (_indexOfActiveTerritory.HasValue && _indexOfActiveTerritory.Value == territoryId)
                {
                    // Deselect active territory
                    _indexOfActiveTerritory = null;
                    _indexesOfHostileNeighbours = new int[] { };
                    _indexesOfReachableTerritories = new int[] { };
                    ActiveTerritoryHighlighted = false;
                    AttackVectorVisible = false;
                    UpdateCommandAvailability();
                    return;
                }

                if (PlayerViewModels[_application.Engine.CurrentPlayerIndex].ArmiesToDeploy == 0)
                {
                    if (_indexesOfHostileNeighbours.Contains(territoryId) &&
                        !_application.Engine.CurrentPlayerHasReinforced)
                    {
                        if (!_indexOfTargetTerritory.HasValue ||
                            _indexOfTargetTerritory.Value != territoryId)
                        {
                            // Select hostile neighbor to active territory
                            SelectedTargetVertexCanvasPosition = MapViewModel.PointViewModels[territoryId].Point - new PointD(20, 20);
                            _indexOfTargetTerritory = territoryId;
                            AttackVectorVisible = true;
                        }
                        else
                        {
                            // Deselect active territory, target territory, and vector
                            _indexOfActiveTerritory = null;
                            _indexesOfHostileNeighbours = new int[] { };
                            _indexesOfReachableTerritories = new int[] { };
                            ActiveTerritoryHighlighted = false;
                            AttackVectorVisible = false;
                        }

                        UpdateCommandAvailability();
                        return;
                    }
                    
                    if (_indexesOfReachableTerritories.Contains(territoryId))
                    {
                        if (!_indexOfTargetTerritory.HasValue ||
                            _indexOfTargetTerritory.Value != territoryId)
                        {
                            // Select destination territory reachable from active territory
                            SelectedTargetVertexCanvasPosition = MapViewModel.PointViewModels[territoryId].Point - new PointD(20, 20);
                            _indexOfTargetTerritory = territoryId;
                            AttackVectorVisible = true;
                        }
                        else
                        {
                            // Deselect active territory, destination territory, and vector
                            _indexOfActiveTerritory = null;
                            _indexesOfHostileNeighbours = new int[] { };
                            _indexesOfReachableTerritories = new int[] { };
                            ActiveTerritoryHighlighted = false;
                            AttackVectorVisible = false;
                        }

                        UpdateCommandAvailability();
                        return;
                    }
                }
                
                if (_application.Engine.CurrentPlayerIndex !=
                    _application.Engine.GetTerritoryStatus(territoryId).ControllingPlayerIndex)
                {
                    // User clicked on a hostile territory that is not a neighbour of the active territory - we ignore that
                    return;
                }

                // Select active territory
                _indexOfActiveTerritory = territoryId;
                _indexOfTargetTerritory = null;
                SelectedVertexCanvasPosition = MapViewModel.PointViewModels[territoryId].Point - new PointD(20, 20);
                ActiveTerritoryHighlighted = true;
                AttackVectorVisible = false;

                if (_application.Engine.SetupPhaseComplete)
                {
                    // Identify hostile neighboring territories the player can attack
                    _indexesOfHostileNeighbours = _application.Engine
                        .IndexesOfHostileNeighbourTerritories(territoryId)
                        .ToArray();

                    // Identify other controlled territories reachable from active territory
                    _indexesOfReachableTerritories = _application.Engine
                        .IndexesOfReachableTerritories(territoryId)
                        .ToArray();
                }

                UpdateCommandAvailability();
            };

            LoadSettingsFromConfigFile();
            _application.Logger?.WriteLine(LogMessageCategory.Information, "Risk Game - starting up");
        }

        private async Task Proceed()
        {
            while (!_application.Engine.GameDecided)
            {
                // Denne konstruktion faciliterer muligheden for at en spiller kan angive, at et antal af de næste armeer skal placeres samme sted,
                // så man undgår at skulle klikke mange gange under setup
                if (!_application.Engine.SetupPhaseComplete &&
                    !_application.Engine.CurrentPlayerIsAutomatic &&
                    _indexOfActiveTerritory.HasValue)
                {
                    if (_repetitions[_application.Engine.CurrentPlayerIndex] > 0)
                    {
                        await Delay(_delay, "E");
                        await Deploy();
                        continue;
                    }
                }

                if (_application.Engine.NextEventOccursAutomatically)
                {
                    var gameEvent = await _application.Engine.ExecuteNextEvent();

                    await HandleGameEvent(gameEvent);
                    LogGameEvent(gameEvent);
                    SyncControlsWithApplication();

                    if (!(gameEvent is PlayerPasses))
                    {
                        //await Delay(_delay, "(generally after event)");
                    }

                    if (!gameEvent.TurnGoesToNextPlayer)
                    {
                        continue;
                    }

                    await SwitchToNextPlayer();

                    if (_application.Engine.CurrentPlayerIsAutomatic)
                    {
                        await Delay(_delay, "after switching to next automatic player");
                    }
                }
                else
                {
                    break;
                }
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

            UpdateCommandAvailability();
        }

        private void OpenSettingsDialog(
            object owner)
        {
            var settingsDialogViewModel = new SettingsDialogViewModel(_playerCount, _humanPlayerCount, _delay);

            if (_applicationDialogService.ShowDialog(settingsDialogViewModel, owner as Window) == DialogResult.OK)
            {
                _playerCount = settingsDialogViewModel.PlayerCount;
                _humanPlayerCount = settingsDialogViewModel.HumanPlayerCount;
                _delay = settingsDialogViewModel.Delay;
            }
        }

        private async Task StartGame()
        {
            // Create engine
            var tempArray = Enumerable.Repeat(true, _playerCount).ToArray();

            for (var i = 0; i < _humanPlayerCount; i++)
            {
                tempArray[i] = false;
            }

            tempArray = tempArray.Shuffle(_random).ToArray();

            _activeTerritoryDuringSetupPhase = new int?[_playerCount];
            _application.Engine = new Engine(tempArray, _random, _graphOfTerritories);
            _application.Engine.Initialize(_continents);
            _indexOfActiveTerritory = null;
            _indexOfTargetTerritory = null;
            _repetitions = new int[_playerCount];

            _application.Engine.StartGame();

            _application.Logger?.WriteLine(
                LogMessageCategory.Information,
                $"New game started ({_playerCount} players)");

            PlayerViewModels.Clear();

            Enumerable
                .Range(0, _application.Engine.PlayerCount)
                .ToList()
                .ForEach(playerIndex =>
                {
                    var name = $"Player {playerIndex + 1}";

                    if (tempArray[playerIndex])
                    {
                        name += " (computer)";
                    }

                    PlayerViewModels.Add(new PlayerViewModel
                    {
                        Name = name,
                        Brush = _colorPalette[playerIndex],
                        ArmiesToDeploy = _application.Engine.ArmiesLeftInPool(playerIndex)
                    });
                });

            for (var playerIndex = 0; playerIndex < _playerCount; playerIndex++)
            {
                if (tempArray[playerIndex])
                {
                    continue;
                }

                var dummy = playerIndex;

                PlayerViewModels[dummy].SelectedCards.PropertyChanged += (s, e) =>
                {
                    _selectedCards = PlayerViewModels[dummy].SelectedCards.Object;

                    _currentPlayerCanTradeInSelectedCards =
                        _selectedCards.Count == 3 &&
                        (_selectedCards.Count(_ => _.Type == CardType.Soldier) == 3 ||
                         _selectedCards.Count(_ => _.Type == CardType.Horse) == 3 ||
                         _selectedCards.Count(_ => _.Type == CardType.Cannon) == 3 ||
                         (_selectedCards.Count(_ => _.Type == CardType.Soldier) == 1 &&
                          _selectedCards.Count(_ => _.Type == CardType.Horse) == 1 &&
                          _selectedCards.Count(_ => _.Type == CardType.Cannon) == 1));

                    UpdateCommandAvailability();
                };
            }

            // Diagnostics (only makes a difference when players start with cards, which they usually don't)
            for (var i = 0; i < tempArray.Length; i++)
            {
                UpdateCardViewModels(i);
            }

            for (var i = 0; i < _playerCount; i++)
            {
                var territoryNames = new List<string>();

                for (var j = 0; j < _graphOfTerritories.Vertices.Count; j++)
                {
                    if (_application.Engine.GetTerritoryStatus(j).ControllingPlayerIndex == i)
                    {
                        territoryNames.Add(_territoryNameMap[j]);
                    }
                }

                territoryNames.Sort();

                var sb = new StringBuilder($"Player {i + 1} controls {territoryNames.Count} territories: ");
                sb.Append($"{territoryNames.Aggregate((c, n) => $"{c}, {n}")}");

                _application.Logger?.WriteLine(
                    LogMessageCategory.Information,
                    sb.ToString());
            }

            for (var i = 0; i < _playerCount; i++)
            {
                _application.Logger?.WriteLine(
                    LogMessageCategory.Information,
                    $"Player {i + 1} has {_application.Engine.ArmiesLeftInPool(i)} armies left to deploy");
            }

            SelectedDeployOption = "1";
            SyncControlsWithApplication();
            await Delay(_delay, "A");
            await SwitchToNextPlayer();
            await Delay(_delay, "B");
        }

        private bool CanStartGame()
        {
            return !GameInProgress || !_application.Engine.CurrentPlayerIsAutomatic;
        }

        private async Task TradeInSelectedCards()
        {
            var gameEvent = await _application.Engine.PlayerSelectsOption(
                new TradeInCards
                {
                    Cards = _selectedCards
                });

            _selectedCards = null;
            _currentPlayerCanTradeInSelectedCards = false;

            if (_tradingCardsAfterDefeatingOpponent &&
                _application.Engine.GetHand(_application.Engine.CurrentPlayerIndex).Count < 5)
            {
                _tradingCardsAfterDefeatingOpponent = false;
            }

            PlayerViewModels[_application.Engine.CurrentPlayerIndex].ArmiesToDeploy = 
                _application.Engine.ExtraArmiesForCurrentPlayer;

            ActiveTerritoryHighlighted = false;
            AttackVectorVisible = false;
            _indexOfActiveTerritory = null;
            _indexOfTargetTerritory = null;

            UpdateCardViewModels(_application.Engine.CurrentPlayerIndex);
            SyncControlsWithApplication();
            UpdateCommandAvailability();
            LogGameEvent(gameEvent);
        }

        private bool CanTradeInSelectedCards()
        {
            return GameInProgress &&
                   !_application.Engine.CurrentPlayerIsAutomatic &&
                   _currentPlayerCanTradeInSelectedCards &&
                   _application.Engine.SetupPhaseComplete &&
                   (_application.Engine.CurrentPlayerMayReinforce || _tradingCardsAfterDefeatingOpponent)&&
                   !_application.Engine.CurrentPlayerHasMovedTroops;
        }

        private async Task Reinforce()
        {
            var gameEvent = await _application.Engine.PlayerSelectsOption(
                new Reinforce());

            PlayerViewModels[_application.Engine.CurrentPlayerIndex].ArmiesToDeploy = _application.Engine.ExtraArmiesForCurrentPlayer;
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
                   !_application.Engine.CurrentPlayerIsAutomatic &&
                   _application.Engine.SetupPhaseComplete &&
                   _application.Engine.CurrentPlayerMayReinforce &&
                   !_application.Engine.CurrentPlayerHasMovedTroops &&
                   _application.Engine.ExtraArmiesForCurrentPlayer == 0 &&
                   _application.Engine.GetHand(_application.Engine.CurrentPlayerIndex).Count() < 5;
        }

        private async Task Deploy()
        {
            if (!_indexOfActiveTerritory.HasValue)
            {
                throw new InvalidOperationException(
                    "Deploy called without having selected a territory - should not be possible");
            }

            var armiesToDeploy = _application.Engine.ExtraArmiesForCurrentPlayer;

            if (SelectedDeployOption != "All")
            {
                armiesToDeploy = Math.Min(armiesToDeploy, int.Parse(SelectedDeployOption));
            }

            var gameEvent = await _application.Engine.PlayerSelectsOption(
                new Deploy
                {
                    ActiveTerritoryIndex = _indexOfActiveTerritory.Value,
                    Armies = armiesToDeploy
                });

            if (_application.Engine.SetupPhaseComplete)
            {
                // Update the number of armies to deploy for the current player
                PlayerViewModels[_application.Engine.CurrentPlayerIndex].ArmiesToDeploy = _application.Engine.ExtraArmiesForCurrentPlayer;
            }
            else
            {
                // Update the number of armies to deploy for the previous player (since the turn went to the next player)
                var previousPlayerIndex =
                    (_application.Engine.CurrentPlayerIndex + _application.Engine.PlayerCount - 1) %
                    _application.Engine.PlayerCount;

                var armiesLeftInPoolForPreviousPlayer = _application.Engine.ArmiesLeftInPool(previousPlayerIndex);
                PlayerViewModels[previousPlayerIndex].ArmiesToDeploy = armiesLeftInPoolForPreviousPlayer;
                _activeTerritoryDuringSetupPhase[previousPlayerIndex] = _indexOfActiveTerritory.Value;
                _application.Engine.CompleteSetupPhaseIfPoolIsEmpty();

                // Determine repetitions during setup phase
                if (_repetitions[previousPlayerIndex] > 0)
                {
                    _repetitions[previousPlayerIndex]--;
                }
                else
                {
                    var repetitions = SelectedDeployOption switch
                    {
                        "All" => armiesLeftInPoolForPreviousPlayer,
                        "10" => 9,
                        "5" => 4,
                        "2" => 1,
                        _ => 0
                    };

                    repetitions = Math.Min(repetitions, armiesLeftInPoolForPreviousPlayer);
                    _repetitions[previousPlayerIndex] = repetitions;
                }
            }

            SyncControlsWithApplication();
            UpdateCommandAvailability();
            LogGameEvent(gameEvent);

            //await Delay(_delay, "after deployment");

            if (gameEvent.TurnGoesToNextPlayer)
            {
                await SwitchToNextPlayer();
                await Delay(_delay, "(after switching to next player in Deploy method)");
                await Proceed();
            }
        }

        private bool CanDeploy()
        {
            return GameInProgress &&
                   !_application.Engine.CurrentPlayerIsAutomatic &&
                   _indexOfActiveTerritory.HasValue &&
                   PlayerViewModels[_application.Engine.CurrentPlayerIndex].ArmiesToDeploy > 0 &&
                   _application.Engine.GetHand(_application.Engine.CurrentPlayerIndex).Count() < 5;
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
                if (_application.Engine.GameDecided)
                {
                    GameResultMessage = "Congratulations\nYou won!";
                    ActiveTerritoryHighlighted = false;
                    PlayerViewModels[playerAttacks.DefendingPlayerIndex].Defeated = true;
                }
                else
                {
                    if (playerAttacks.DefendingPlayerDefeated)
                    {
                        UpdateCardViewModels(_application.Engine.CurrentPlayerIndex);
                        UpdateCardViewModels(playerAttacks.DefendingPlayerIndex);
                        PlayerViewModels[playerAttacks.DefendingPlayerIndex].Defeated = true;

                        if (_application.Engine.GetHand(_application.Engine.CurrentPlayerIndex).Count > 5)
                        {
                            _tradingCardsAfterDefeatingOpponent = true;
                        }
                    }
                    else if (playerAttacks.Card != null)
                    {
                        PlayerViewModels[_application.Engine.CurrentPlayerIndex].AddCardViewModel(
                            _territoryNameMap[playerAttacks.Card.TerritoryIndex],
                            playerAttacks.Card);
                    }

                    PlayerViewModels[_application.Engine.CurrentPlayerIndex].WatchCardsButtonVisible =
                        PlayerViewModels[_application.Engine.CurrentPlayerIndex].CardViewModels.Any();

                    var armiesInTotal =
                        _application.Engine.GetTerritoryStatus(_indexOfActiveTerritory.Value).Armies +
                        _application.Engine.GetTerritoryStatus(_indexOfTargetTerritory.Value).Armies;

                    var sb = new StringBuilder($"You succesfully conquered {_territoryNameMap[_indexOfTargetTerritory.Value]}");
                    sb.Append($" from {_territoryNameMap[_indexOfActiveTerritory.Value]}");

                    var dialog = new TransferArmiesDialogViewModel(
                        sb.ToString(),
                        playerAttacks.DiceRolledByAttacker,
                        armiesInTotal - 1);

                    _applicationDialogService.ShowDialog(dialog, null);

                    gameEvent = _application.Engine.TransferArmies(
                        _indexOfActiveTerritory.Value,
                        _indexOfTargetTerritory.Value,
                        dialog.ArmiesToTransfer,
                        false,
                        true);

                    _indexOfActiveTerritory = _indexOfTargetTerritory;
                    _indexOfTargetTerritory = null;

                    _indexesOfHostileNeighbours = _application.Engine
                        .IndexesOfHostileNeighbourTerritories(_indexOfActiveTerritory.Value)
                        .ToArray();

                    _indexesOfReachableTerritories = _application.Engine
                        .IndexesOfReachableTerritories(_indexOfActiveTerritory.Value)
                        .ToArray();

                    SelectedVertexCanvasPosition = MapViewModel.PointViewModels[_indexOfActiveTerritory.Value].Point - new PointD(20, 20);
                    ActiveTerritoryHighlighted = true;
                }

                AttackVectorVisible = false;
                SyncControlsWithApplication();
                UpdateCommandAvailability();
                LogGameEvent(gameEvent);
            }
        }

        private bool CanAttack()
        {
            if (_tradingCardsAfterDefeatingOpponent ||
                (_application.Engine != null &&
                 _application.Engine.GetHand(_application.Engine.CurrentPlayerIndex).Count() >= 5 &&
                 !_application.Engine.CurrentPlayerReceivedACard))
            {
                // Player must trade cards
                return false;
            }

            return GameInProgress &&
                   !_application.Engine.CurrentPlayerIsAutomatic &&
                   PlayerViewModels[_application.Engine.CurrentPlayerIndex].ArmiesToDeploy == 0 &&
                   _indexOfActiveTerritory.HasValue &&
                   _indexOfTargetTerritory.HasValue &&
                   _application.Engine.SetupPhaseComplete &&
                   !_application.Engine.CurrentPlayerHasReinforced &&
                   !_application.Engine.CurrentPlayerHasMovedTroops &&
                   _application.Engine.GetTerritoryStatus(_indexOfActiveTerritory.Value).Armies > 1 &&
                   _application.Engine.GetTerritoryStatus(_indexOfTargetTerritory.Value).ControllingPlayerIndex != _application.Engine.CurrentPlayerIndex;
        }

        private async Task Move()
        {
            var armiesInTerritory = _application.Engine.GetTerritoryStatus(_indexOfActiveTerritory.Value).Armies;

            var dialog = new TransferArmiesDialogViewModel(
                "Transfer armies? (this will end your turn)", 
                1,
                armiesInTerritory - 1);

            if (_applicationDialogService.ShowDialog(dialog, null) == DialogResult.OK)
            {
                var gameEvent = _application.Engine.TransferArmies(
                    _indexOfActiveTerritory.Value,
                    _indexOfTargetTerritory.Value,
                    dialog.ArmiesToTransfer,
                    false,
                    false);

                AttackVectorVisible = false;
                ActiveTerritoryHighlighted = false;

                SyncControlsWithApplication();
                UpdateCommandAvailability();
                LogGameEvent(gameEvent);
            }
        }

        private bool CanMove()
        {
            if (_tradingCardsAfterDefeatingOpponent ||
                (_application.Engine != null &&
                 _application.Engine.GetHand(_application.Engine.CurrentPlayerIndex).Count() >= 5 &&
                 !_application.Engine.CurrentPlayerReceivedACard))
            {
                // Player must trade cards
                return false;
            }

            return GameInProgress &&
                   !_application.Engine.CurrentPlayerIsAutomatic &&
                   PlayerViewModels[_application.Engine.CurrentPlayerIndex].ArmiesToDeploy == 0 &&
                   _indexOfActiveTerritory.HasValue &&
                   _indexOfTargetTerritory.HasValue &&
                   _application.Engine.SetupPhaseComplete &&
                   !_application.Engine.CurrentPlayerHasMovedTroops &&
                   _application.Engine.GetTerritoryStatus(_indexOfActiveTerritory.Value).Armies > 1 &&
                   _application.Engine.GetTerritoryStatus(_indexOfTargetTerritory.Value).ControllingPlayerIndex == _application.Engine.CurrentPlayerIndex;
        }

        private async Task Pass()
        {
            var gameEvent = await _application.Engine.PlayerSelectsOption(new Pass());
            LogGameEvent(gameEvent);

            _indexOfActiveTerritory = null;
            _indexOfTargetTerritory = null;
            _indexesOfHostileNeighbours = new int[] { };
            _indexesOfReachableTerritories = new int[] { };

            if (_application.Engine.GameDecided)
            {
                GameResultMessage = "Congratulations - You Win";
                GameInProgress = _application.Engine.GameInProgress;
                GameDecided = _application.Engine.GameDecided;
            }
            else
            {
                await SwitchToNextPlayer();
                SyncControlsWithApplication();
                UpdateCommandAvailability();
                await Proceed();
            }
        }

        private bool CanPass()
        {
            if (_tradingCardsAfterDefeatingOpponent ||
                (_application.Engine != null &&
                 _application.Engine.GetHand(_application.Engine.CurrentPlayerIndex).Count() >= 5 &&
                 !_application.Engine.CurrentPlayerReceivedACard))
            {
                // Player must trade cards
                return false;
            }

            return GameInProgress &&
                   !_application.Engine.CurrentPlayerIsAutomatic &&
                   PlayerViewModels[_application.Engine.CurrentPlayerIndex].ArmiesToDeploy == 0;
        }

        private void SyncControlsWithApplication()
        {
            GameInProgress = _application.Engine.GameInProgress;
            GameDecided = _application.Engine.GameDecided;

            // Colors and numbers on map
            _graphOfTerritories.Vertices.ForEach(_ =>
            {
                var territoryStatus = _application.Engine.GetTerritoryStatus(_.Id);

                MapViewModel.StylePoint(
                    _.Id, 
                    _colorPalette[territoryStatus.ControllingPlayerIndex],
                    territoryStatus.Armies == 0 ? "" : $"{territoryStatus.Armies}");
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
            if (LoggingActive && false)
            {
                _application.Logger?.WriteLine(
                    LogMessageCategory.Information,
                    "Updating command availability");
            }

            StartGameCommand.RaiseCanExecuteChanged();
            TradeInSelectedCardsCommand.RaiseCanExecuteChanged();
            ReinforceCommand.RaiseCanExecuteChanged();
            DeployCommand.RaiseCanExecuteChanged();
            AttackCommand.RaiseCanExecuteChanged();
            MoveCommand.RaiseCanExecuteChanged();
            PassCommand.RaiseCanExecuteChanged();
        }

        private static IGraph<LabelledVertex, EmptyEdge> GenerateGraphOfTerritories()
        {
            var vertices = new List<LabelledVertex>
            {
                // North America
                new LabelledVertex("Alaska"),                //  0 (soldier)
                new LabelledVertex("Northwest Territory"),   //  1 (canon)
                new LabelledVertex("Greenland"),             //  2 (horse)
                new LabelledVertex("Alberta"),               //  3 (horse)
                new LabelledVertex("Ontario"),               //  4 (horse)
                new LabelledVertex("Quebec"),                //  5 (horse)
                new LabelledVertex("Western United States"), //  6 (canon)
                new LabelledVertex("Eastern United States"), //  7 (canon)
                new LabelledVertex("Central America"),       //  8 (canon)

                // South America
                new LabelledVertex("Venezuela"),   //  9 (soldier)
                new LabelledVertex("Peru"),        // 10 (soldier)
                new LabelledVertex("Argentina"),   // 11 (soldier)
                new LabelledVertex("Brazil"),      // 12 (canon)

                // Europe
                new LabelledVertex("Iceland"),         // 13 (soldier)
                new LabelledVertex("Scandinavia"),     // 14 (horse)
                new LabelledVertex("Great Britain"),   // 15 (canon)
                new LabelledVertex("Northern Europe"), // 16 (canon)
                new LabelledVertex("Ukraine"),         // 17 (horse)
                new LabelledVertex("Western Europe"),  // 18 (canon)
                new LabelledVertex("Southern Europe"), // 19 (canon)

                // Africa
                new LabelledVertex("North Africa"), // 20 (horse)
                new LabelledVertex("Egypt"),        // 21 (soldier)
                new LabelledVertex("East Africa"),  // 22 (soldier)
                new LabelledVertex("Congo"),        // 23 (soldier)
                new LabelledVertex("South Africa"), // 24 (canon)
                new LabelledVertex("Madagascar"),   // 25 (horse)

                // Asia
                new LabelledVertex("Siberia"),     // 26 (horse)
                new LabelledVertex("Ural"),        // 27 (horse)
                new LabelledVertex("Yakutsk"),     // 28 (horse)
                new LabelledVertex("Kamchatka"),   // 29 (soldier)
                new LabelledVertex("Irkutsk"),     // 30 (horse)
                new LabelledVertex("Afghanistan"), // 31 (horse)
                new LabelledVertex("Mongolia"),    // 32 (soldier)
                new LabelledVertex("Japan"),       // 33 (canon)
                new LabelledVertex("China"),       // 34 (soldier)
                new LabelledVertex("Middle East"), // 35 (soldier)
                new LabelledVertex("India"),       // 36 (horse)
                new LabelledVertex("Siam"),        // 37 (soldier)

                // Oceania
                new LabelledVertex("Indonesia"),         // 38 (canon)
                new LabelledVertex("New Guinea"),        // 39 (soldier)
                new LabelledVertex("Western Australia"), // 40 (canon)
                new LabelledVertex("Eastern Australia"), // 41 (canon)
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
            int milliSeconds,
            string tag)
        {
            if (LoggingActive && !string.IsNullOrEmpty(tag) && false)
            {
                _application.Logger?.WriteLine(
                    LogMessageCategory.Information,
                    tag);
            }

            if (milliSeconds > 0)
            {
                await Task.Delay(milliSeconds);
            }
        }

        private async Task HandleGameEvent(
            IGameEvent gameEvent)
        {
            switch (gameEvent)
            {
                case PlayerAttacks playerAttacks:
                    {
                        var point1 = MapViewModel.PointViewModels[playerAttacks.Vertex1].Point;
                        var point2 = MapViewModel.PointViewModels[playerAttacks.Vertex2].Point;

                        if (_indexOfActiveTerritory != playerAttacks.Vertex1)
                        {
                            AttackVectorVisible = false;
                        }

                        SelectedVertexCanvasPosition = point1 - new PointD(20, 20);
                        SelectedTargetVertexCanvasPosition = point2 - new PointD(20, 20);
                        ActiveTerritoryHighlighted = true;

                        if (_indexOfActiveTerritory != playerAttacks.Vertex1)
                        {
                            await Delay(_delay, "(after highlighting active territory)");
                        }

                        AttackVectorVisible = true;

                        if (_indexOfActiveTerritory != playerAttacks.Vertex1)
                        {
                            await Delay(_delay, "(after highligting target territory)");
                        }

                        _indexOfActiveTerritory = playerAttacks.Vertex1;

                        if (playerAttacks.DefendingPlayerDefeated)
                        {
                            PlayerViewModels[playerAttacks.DefendingPlayerIndex].Defeated = true;
                        }

                        if (!_application.Engine.GameDecided)
                        {
                            if (playerAttacks.DefendingPlayerDefeated)
                            {
                                UpdateCardViewModels(_application.Engine.CurrentPlayerIndex);
                                UpdateCardViewModels(playerAttacks.DefendingPlayerIndex);
                            }
                            else if (playerAttacks.Card != null)
                            {
                                PlayerViewModels[_application.Engine.CurrentPlayerIndex]
                                    .AddCardViewModel(
                                        _territoryNameMap[playerAttacks.Card.TerritoryIndex],
                                        playerAttacks.Card);
                            }
                        }

                        break;
                    }
                case PlayerTradesInCards:
                    {
                        UpdateCardViewModels(_application.Engine.CurrentPlayerIndex);
                        ActiveTerritoryHighlighted = false;
                        AttackVectorVisible = false;
                        break;
                    }
                case PlayerDeploysArmies playerDeploysArmies:
                    {
                        AttackVectorVisible = false;

                        foreach (var territoryIndex in playerDeploysArmies.TerritoryIndexes)
                        {
                            _indexOfActiveTerritory = territoryIndex;
                            var point = MapViewModel.PointViewModels[territoryIndex].Point;
                            SelectedVertexCanvasPosition = point - new PointD(20, 20);
                            ActiveTerritoryHighlighted = true;
                            await Delay(_delay, "C");
                        };

                        if (_application.Engine.SetupPhaseComplete)
                        {
                            _indexOfActiveTerritory = null;
                            ActiveTerritoryHighlighted = false;
                            AttackVectorVisible = false;
                        }

                        if (playerDeploysArmies.TurnGoesToNextPlayer)
                        {
                            var indexOfPreviousPlayer =
                                (_application.Engine.CurrentPlayerIndex + _application.Engine.PlayerCount - 1) %
                                _application.Engine.PlayerCount;

                            PlayerViewModels[indexOfPreviousPlayer].ArmiesToDeploy =
                                _application.Engine.ArmiesLeftInPool(indexOfPreviousPlayer);
                        }

                        break;
                    }
                case PlayerReinforces:
                case PlayerTransfersArmies:
                case PlayerPasses:
                case PlayerIsSkipped:
                    {
                        ActiveTerritoryHighlighted = false;
                        AttackVectorVisible = false;
                        break;
                    }
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

                        if (playerAttacks.Card != null)
                        {
                            sb.Append($", and player {playerAttacks.PlayerIndex + 1} gets a card");
                        }

                        if (playerAttacks.DefendingPlayerDefeated)
                        {
                            sb.Append($". Player {playerAttacks.DefendingPlayerIndex + 1} is defeated");
                        }
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

                    if (!_application.Engine.SetupPhaseComplete)
                    {
                        sb.Append($" ({_application.Engine.ArmiesLeftInPool(playerDeploysArmies.PlayerIndex)} armies left to deploy)");
                    }

                    break;
                }
                case PlayerPasses:
                {
                    sb.Append(" passes");
                    break;
                }
                case PlayerIsSkipped:
                {
                    sb.Append(" is skipped");
                    break;
                }
                case PlayerTransfersArmies playerTransfersArmies:
                {
                    sb.Append($" transfers {playerTransfersArmies.ArmiesTransfered} ");
                    sb.Append(playerTransfersArmies.ArmiesTransfered == 1 ? "army" : "armies");
                    sb.Append($" from {_territoryNameMap[playerTransfersArmies.Vertex1]}");
                    sb.Append($" to {_territoryNameMap[playerTransfersArmies.Vertex2]}");
                    break;
                }
                case PlayerTradesInCards playerTradesInCards:
                {
                    var armiesReceivedInTotal =
                        playerTradesInCards.ArmiesReceivedForCards +
                        playerTradesInCards.ArmiesReceivedForControlledTerritories;

                    sb.Append($" trades in 3 cards for {armiesReceivedInTotal} armies");

                    if (playerTradesInCards.ArmiesReceivedForControlledTerritories > 0)
                    {
                        sb.Append($" ({playerTradesInCards.ArmiesReceivedForControlledTerritories} for controlled territories)");
                    }

                    break;
                }
            }

            _application.Logger?.WriteLine(
                LogMessageCategory.Information,
                sb.ToString());
        }

        private async Task SwitchToNextPlayer()
        {
            _selectedCards = null;
            _indexOfActiveTerritory = null;
            _indexOfTargetTerritory = null;
            _currentPlayerCanTradeInSelectedCards = false;
            _tradingCardsAfterDefeatingOpponent = false;

            var indexOfPreviousPlayer = 
                (_application.Engine.CurrentPlayerIndex + _application.Engine.PlayerCount - 1) %
                _application.Engine.PlayerCount;

            PlayerViewModels[indexOfPreviousPlayer].WatchCardsButtonVisible = false;
            PlayerViewModels[indexOfPreviousPlayer].HandHidden = true;

            ActiveTerritoryHighlighted = false;
            AttackVectorVisible = false;
            HighlightCurrentPlayer();
            UpdateCommandAvailability();

            if (!_application.Engine.CurrentPlayerIsAutomatic)
            {
                var activePlayerViewModel = PlayerViewModels[_application.Engine.CurrentPlayerIndex];

                if (activePlayerViewModel.CardViewModels.Any())
                {
                    activePlayerViewModel.WatchCardsButtonVisible = true;
                }
            }

            if (_application.Engine.SetupPhaseComplete)
            {
                _application.Logger?.WriteLine(
                    LogMessageCategory.Information,
                    $"Turn goes to Player {_application.Engine.CurrentPlayerIndex + 1}");

                ActiveTerritoryHighlighted = false;
                AssignExtraArmiesForControlledContinents();
            }
            else
            {
                if (!_application.Engine.CurrentPlayerIsAutomatic && 
                    _activeTerritoryDuringSetupPhase[_application.Engine.CurrentPlayerIndex].HasValue)
                {
                    await Delay(_delay, "D");
                    _indexOfActiveTerritory = _activeTerritoryDuringSetupPhase[_application.Engine.CurrentPlayerIndex].Value;
                    SelectedVertexCanvasPosition = MapViewModel.PointViewModels[_indexOfActiveTerritory.Value].Point - new PointD(20, 20);
                    ActiveTerritoryHighlighted = true;
                }
                else
                {
                    ActiveTerritoryHighlighted = false;
                }

                PlayerViewModels[_application.Engine.CurrentPlayerIndex].ArmiesToDeploy =
                    _application.Engine.ArmiesLeftInPool(_application.Engine.CurrentPlayerIndex);

                AssignAnArmyFromInitialPool();
            }
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

        private void AssignAnArmyFromInitialPool()
        {
            _application.Engine.AssignAnArmyFromInitialPool();
        }

        private void AssignExtraArmiesForControlledContinents()
        {
            var continents = _application.Engine.AssignExtraArmiesForControlledContinents();

            if (!continents.Any())
            {
                return;
            }

            if (!_application.Engine.CurrentPlayerIsAutomatic)
            {
                PlayerViewModels[_application.Engine.CurrentPlayerIndex].ArmiesToDeploy =
                    _application.Engine.ExtraArmiesForCurrentPlayer;
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

        private void UpdateCardViewModels(
            int playerIndex)
        {
            var playerViewModel = PlayerViewModels[playerIndex];
            var cardViewModels = playerViewModel.CardViewModels;

            playerViewModel.Height = 0;
            cardViewModels.Clear();

            _application.Engine.GetHand(playerIndex).ForEach(_ =>
            {
                playerViewModel.AddCardViewModel(
                    _territoryNameMap[_.TerritoryIndex],
                    _);
            });

            playerViewModel.WatchCardsButtonVisible = cardViewModels.Any() && !_application.Engine.CurrentPlayerIsAutomatic;
        }

        private void LoadSettingsFromConfigFile()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            if (!int.TryParse(settings["PlayerCount"]?.Value, out var playerCount) ||
                !int.TryParse(settings["HumanPlayerCount"]?.Value, out var humanPlayerCount) ||
                !int.TryParse(settings["Delay"]?.Value, out var delay)
                )
            {
                throw new InvalidDataException("Invalid Config file");
            }

            _playerCount = playerCount;
            _humanPlayerCount = humanPlayerCount;
            _delay = delay;
        }
    }
}