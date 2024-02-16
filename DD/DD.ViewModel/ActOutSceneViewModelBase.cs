using Craft.Logging;
using Craft.Utils;
using Craft.ViewModel.Utils;
using DD.Application;
using DD.Domain;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;

namespace DD.ViewModel;

public abstract class ActOutSceneViewModelBase
{
    protected ILogger _logger;
    protected IEngine _engine;
    protected BoardViewModel _boardViewModel;
    protected RelayCommand _resetCreaturesCommand;
    protected AsyncCommand _startBattleCommand;
    protected AsyncCommand _passCurrentCreatureCommand;
    protected AsyncCommand _automateCurrentCreatureCommand;

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

    public ActOutSceneViewModelBase(
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

    protected abstract Task Proceed();

    private void ResetCreatures()
    {
        var message = $"Resetting scene \"{_engine.Scene.Name}\"";
        _logger?.WriteLine(LogMessageCategory.Information, message);

        _engine.InitializeCreatures();

        _boardViewModel.UpdateCreatureViewModels(
            _engine.Creatures,
            _engine.CurrentCreature);
    }

    private bool CanResetCreatures()
    {
        return
            _engine.BattleHasStarted.Object;
    }

    private async Task StartBattle()
    {
        _engine.StartBattle();

        _boardViewModel.UpdateCreatureViewModels(
            _engine.Creatures,
            _engine.CurrentCreature);

        await Proceed();
    }

    private bool CanStartBattle()
    {
        return _engine.CanStartBattle();
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

    private bool CanPassCurrentCreature()
    {
        return
            _engine.BattleHasStarted.Object &&
            !_engine.BattleHasEnded.Object &&
            !_engine.AutoRunning.Object;
    }

    private async Task AutomateCurrentCreature()
    {
        _engine.CurrentCreature.IsAutomatic = true;
        _engine.SquareIndexesCurrentCreatureCanMoveTo.Object = null;
        _engine.SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = null;
        _engine.SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object = null;

        await Proceed();
    }

    private bool CanAutomateCurrentCreature()
    {
        return
            _engine.BattleHasStarted.Object &&
            !_engine.BattleHasEnded.Object &&
            !_engine.AutoRunning.Object;
    }

    private void UpdateCommandStates()
    {
        StartBattleCommand.RaiseCanExecuteChanged();
        PassCurrentCreatureCommand.RaiseCanExecuteChanged();
        AutomateCurrentCreatureCommand.RaiseCanExecuteChanged();
        ResetCreaturesCommand.RaiseCanExecuteChanged();
    }
}