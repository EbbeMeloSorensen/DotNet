using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.Utils;
using Craft.ViewModel.Utils;
using DD.Application;
using DD.Application.BattleEvents;
using DD.Domain;

namespace DD.ViewModel;

public class ActOutSceneViewModelSimpleEngine : ActOutSceneViewModelBase
{
    private readonly ILogger _logger;
    private readonly IEngine _engine;
    private readonly BoardViewModel _boardViewModel;
    private RelayCommand _resetCreaturesCommand;
    private AsyncCommand _startBattleCommand;
    private AsyncCommand _passCurrentCreatureCommand;
    private AsyncCommand _automateCurrentCreatureCommand;

    public RelayCommand ResetCreaturesCommand
    {
        get
        {
            return _resetCreaturesCommand ?? (_resetCreaturesCommand = new RelayCommand(
                ResetCreatures,
                CanResetCreatures));
        }
    }

    public AsyncCommand StartBattleCommand
    {
        get
        {
            return _startBattleCommand ?? (_startBattleCommand = new AsyncCommand(
                StartBattle,
                CanStartBattle));
        }
    }

    public AsyncCommand PassCurrentCreatureCommand
    {
        get
        {
            return _passCurrentCreatureCommand ?? (_passCurrentCreatureCommand = new AsyncCommand(
                PassCurrentCreature,
                CanPassCurrentCreature));
        }
    }

    public AsyncCommand AutomateCurrentCreatureCommand
    {
        get
        {
            return _automateCurrentCreatureCommand ?? (_automateCurrentCreatureCommand = new AsyncCommand(
                AutomateCurrentCreature,
                CanAutomateCurrentCreature));
        }
    }

    public ActOutSceneViewModelSimpleEngine(
        IEngine engine,
        BoardViewModel boardViewModel,
        ObservableObject<Scene> selectedScene,
        ILogger logger)
    {
        _engine = engine;
        _boardViewModel = boardViewModel;
        _logger = logger;

        _engine.BattleHasStarted.PropertyChanged += (s, e) => UpdateCommandStates();
        _engine.BattleHasEnded.PropertyChanged += (s, e) => UpdateCommandStates();
        _engine.AutoRunning.PropertyChanged += (s, e) => UpdateCommandStates();

        boardViewModel.PlayerClickedSquare += async (s, e) =>
        {
            var creatureAction = _engine.PlayerSelectSquare(e.SquareIndex);

            if (creatureAction == null)
            {
                // Occurs e.g. when the user clicks a non-highlighted square that doesn't trigger anything
                return;
            }

            switch (creatureAction)
            {
                case CreatureMove:
                    _boardViewModel.CurrentCreatureIsHighlighted = false;
                    _boardViewModel.MoveCurrentCreature(
                        _engine.CurrentCreature,
                        _engine.CurrentCreaturePath);
                    break;
                case CreatureAttack:
                    _boardViewModel.AnimateAttack(
                        _engine.CurrentCreature,
                        _engine.TargetCreature,
                        false);
                    break;
            }
        };

        boardViewModel.MoveCreatureAnimationCompleted += async (s, e) => await Proceed();

        boardViewModel.AttackAnimationCompleted += async (s, e) =>
        {
            _boardViewModel.UpdateCreatureViewModels(
                _engine.Creatures,
                _engine.CurrentCreature);

            await Proceed();
        };

        selectedScene.PropertyChanged += (s, e) =>
        {
            _boardViewModel.CurrentCreatureIsHighlighted = false;

            _engine.Scene = (s as ObservableObject<Scene>)?.Object;

            _engine.InitializeCreatures();

            _boardViewModel.UpdateCreatureViewModels(
                _engine.Creatures,
                _engine.CurrentCreature);
        };
    }

    private async Task StartBattle()
    {
        _engine.StartBattle();

        _boardViewModel.UpdateCreatureViewModels(
            _engine.Creatures,
            _engine.CurrentCreature);

        await Proceed();
    }

    private async Task Proceed()
    {
        while (!_engine.BattleDecided)
        {
            if (_engine.BattleroundCompleted)
            {
                _engine.StartBattleRound();

                _boardViewModel.UpdateCreatureViewModels(
                    _engine.Creatures,
                    _engine.CurrentCreature);
            }

            if (_engine.NextEventOccursAutomatically)
            {
                _engine.AutoRunning.Object = true;
                _boardViewModel.CurrentCreatureIsHighlighted = false;
                var nextEvent = await _engine.ExecuteNextEvent();

                switch (nextEvent)
                {
                    case CreaturePass:
                        _engine.SwitchToNextCreature();
                        _boardViewModel.UpdateCreatureViewModels(
                            _engine.Creatures,
                            _engine.CurrentCreature);
                        continue;
                    // Bemærk, at vi for de næste 3 ikke kalder continue men derimod break, dvs vi træder ud af løkken og dermed hele Proceed
                    // metoden. Den kaldes igen, når vi håndterer disse evenst: MoveCreatureAnimationCompleted, AttackAnimationCompleted
                    case CreatureMove:
                        _boardViewModel.MoveCurrentCreature(
                            _engine.CurrentCreature,
                            _engine.CurrentCreaturePath);
                        break;
                    case CreatureAttack:
                        _boardViewModel.AnimateAttack(
                            _engine.CurrentCreature,
                            _engine.TargetCreature,
                            false);
                        break;
                    default:
                        throw new ArgumentException("unexpected battle event");
                }
            }
            else
            {
                _engine.AutoRunning.Object = false;
                _boardViewModel.CurrentCreatureIsHighlighted = true;

                // Diagnostics
                //_logger.WriteLine(LogMessageCategory.Information, "(Proceed method about to exit - Initiative will go to the player)");
            }

            break;
        }

        if (_engine.BattleDecided)
        {
            _logger.WriteLine(LogMessageCategory.Information, "Battle was decided");
        }
        else
        {
            // Diagnostics
            //_logger.WriteLine(LogMessageCategory.Information, "(Proceed method exiting)");
        }
    }

    private async Task PassCurrentCreature()
    {
        var message =
            $"        {_engine.CurrentCreature.CreatureType.Name}{_engine.Tag(_engine.CurrentCreature)} passes";

        _logger?.WriteLine(LogMessageCategory.Information, message);

        _engine.SquareIndexesCurrentCreatureCanMoveTo.Object = null;
        _engine.SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = null;
        _engine.SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object = null;

        _engine.SwitchToNextCreature();

        _boardViewModel.UpdateCreatureViewModels(
            _engine.Creatures,
            _engine.CurrentCreature);

        await Proceed();
    }

    private async Task AutomateCurrentCreature()
    {
        _engine.CurrentCreature.IsAutomatic = true;
        _engine.SquareIndexesCurrentCreatureCanMoveTo.Object = null;
        _engine.SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = null;
        _engine.SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object = null;

        await Proceed();
    }

    private void ResetCreatures()
    {
        var message = $"Resetting scene \"{_engine.Scene.Name}\"";
        _logger?.WriteLine(LogMessageCategory.Information, message);

        _engine.InitializeCreatures();

        _boardViewModel.UpdateCreatureViewModels(
            _engine.Creatures,
            _engine.CurrentCreature);
    }

    private bool CanStartBattle()
    {
        return _engine.CanStartBattle();
    }

    private bool CanPassCurrentCreature()
    {
        return
            _engine.BattleHasStarted.Object &&
            !_engine.BattleHasEnded.Object &&
            !_engine.AutoRunning.Object;
    }

    private bool CanAutomateCurrentCreature()
    {
        return
            _engine.BattleHasStarted.Object &&
            !_engine.BattleHasEnded.Object &&
            !_engine.AutoRunning.Object;
    }

    private bool CanResetCreatures()
    {
        return
            _engine.BattleHasStarted.Object;
    }

    private void UpdateCommandStates()
    {
        StartBattleCommand.RaiseCanExecuteChanged();
        PassCurrentCreatureCommand.RaiseCanExecuteChanged();
        AutomateCurrentCreatureCommand.RaiseCanExecuteChanged();
        ResetCreaturesCommand.RaiseCanExecuteChanged();
    }
}