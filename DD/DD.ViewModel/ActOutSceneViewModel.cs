using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.Utils;
using Craft.ViewModel.Utils;
using DD.Domain;
using DD.Application;

namespace DD.ViewModel
{
    public class ActOutSceneViewModel : ViewModelBase
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
            get { return _resetCreaturesCommand ?? (_resetCreaturesCommand = new RelayCommand(
                ResetCreatures,
                CanResetCreatures)); }
        }

        public AsyncCommand StartBattleCommand
        {
            get { return _startBattleCommand ?? (_startBattleCommand = new AsyncCommand(
                StartBattle, 
                CanStartBattle)); }
        }

        public AsyncCommand PassCurrentCreatureCommand
        {
            get { return _passCurrentCreatureCommand ?? (_passCurrentCreatureCommand = new AsyncCommand(
                PassCurrentCreature,
                CanPassCurrentCreature)); }
        }

        public AsyncCommand AutomateCurrentCreatureCommand
        {
            get { return _automateCurrentCreatureCommand ?? (_automateCurrentCreatureCommand = new AsyncCommand(
                AutomateCurrentCreature,
                CanAutomateCurrentCreature)); }
        }

        public ActOutSceneViewModel(
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

                switch (creatureAction)
                {
                    case CreatureAction.Move:
                        _boardViewModel.MoveCurrentCreature(
                            _engine.CurrentCreature,
                            _engine.CurrentCreaturePath);
                        break;
                    case CreatureAction.Evade:
                        await Proceed();
                        break;
                    case CreatureAction.MeleeAttack:
                        _boardViewModel.AnimateAttack(
                            _engine.CurrentCreature,
                            _engine.TargetCreature,
                            false);
                        break;
                    case CreatureAction.RangedAttack:
                        _boardViewModel.AnimateAttack(
                            _engine.CurrentCreature,
                            _engine.TargetCreature,
                            true);
                        break;
                    case CreatureAction.Pass:
                    case CreatureAction.NoAction:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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
                    var creatureAction = await _engine.ExecuteNextAction();

                    switch (creatureAction)
                    {
                        case CreatureAction.InitiativeSwitchDuringEvasion:
                            _boardViewModel.UpdateCreatureViewModels(
                                _engine.Creatures,
                                _engine.CurrentCreature);
                            continue;
                        case CreatureAction.Pass:
                            _engine.SwitchToNextCreature();
                            _boardViewModel.UpdateCreatureViewModels(
                                _engine.Creatures,
                                _engine.CurrentCreature);
                            continue;
                        // Bemærk, at vi for de næste 3 ikke kalder continue men derimod break, dvs vi træder ud af løkken og dermed hele Proceed
                        // metoden. Den kaldes igen, når vi håndterer disse evenst: MoveCreatureAnimationCompleted, AttackAnimationCompleted
                        case CreatureAction.Move:
                            _boardViewModel.MoveCurrentCreature(
                                _engine.CurrentCreature,
                                _engine.CurrentCreaturePath);
                            break;
                        case CreatureAction.MeleeAttack:
                            _boardViewModel.AnimateAttack(
                                _engine.CurrentCreature,
                                _engine.TargetCreature,
                                false);
                            break;
                        case CreatureAction.RangedAttack:
                            _boardViewModel.AnimateAttack(
                                _engine.CurrentCreature,
                                _engine.TargetCreature,
                                true);
                            break;
                        default:
                            continue;
                    }
                }
                else
                {
                    _engine.AutoRunning.Object = false;

                    if (!_engine.CurrentPlayerControlledCreatureHasAnyOptionsLeft())
                    {
                        _engine.SwitchToNextCreature();
                        _boardViewModel.UpdateCreatureViewModels(
                            _engine.Creatures,
                            _engine.CurrentCreature);
                        continue;
                    }
                }

                break;
            }

            _logger.WriteLine(LogMessageCategory.Information, "Battle was decided");
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
}
