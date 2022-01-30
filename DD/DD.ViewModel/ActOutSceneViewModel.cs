using System;
using System.Threading.Tasks;
using System.Collections.Generic;
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
        private readonly Engine _engine;
        private readonly BoardViewModel _boardViewModel;
        private RelayCommand _resetCreaturesCommand;
        private AsyncCommand _startBattleCommand;
        private AsyncCommand _passCurrentCreatureCommand;
        private AsyncCommand _automateCurrentCreatureCommand;
        private bool _rangedAttackSucceeded;

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
            Engine engine,
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
                    case CreatureAction.FailedMeleeAttack:
                        _rangedAttackSucceeded = false;
                        _boardViewModel.AnimateAttack(
                            _engine.CurrentCreature,
                            _engine.TargetCreature,
                            false);
                        break;
                    case CreatureAction.FailedRangedAttack:
                        _rangedAttackSucceeded = false;
                        _boardViewModel.AnimateAttack(
                            _engine.CurrentCreature,
                            _engine.TargetCreature,
                            true);
                        break;
                    case CreatureAction.SuccessfulMeleeAttack:
                        _rangedAttackSucceeded = true;
                        _boardViewModel.AnimateAttack(
                            _engine.CurrentCreature,
                            _engine.TargetCreature,
                            false);
                        break;
                    case CreatureAction.SuccessfulRangedAttack:
                        _rangedAttackSucceeded = true;
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

            boardViewModel.AnimationCompleted += async (s, e) => await Proceed();

            boardViewModel.FireProjectileAnimationCompleted += async (s, e) =>
            {
                if (_rangedAttackSucceeded)
                {
                    _boardViewModel.UpdateCreatureViewModels(
                        _engine.Creatures,
                        _engine.CurrentCreature);
                }

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
            _engine.StartBattleRound();

            _engine.SwitchToNextCreature();

            _boardViewModel.UpdateCreatureViewModels(
                _engine.Creatures,
                _engine.CurrentCreature);

            await Proceed();
        }

        private async Task Proceed()
        {
            while (true)
            {
                if (_engine.BattleDecided)
                {
                    _logger.WriteLine(LogMessageCategory.Information, "Battle was decided");
                    break;
                }

                if (_engine.BattleroundCompleted)
                {
                    _engine.StartBattleRound();
                    _engine.SwitchToNextCreature();
                    _boardViewModel.UpdateCreatureViewModels(
                        _engine.Creatures,
                        _engine.CurrentCreature);
                }

                if (_engine.CreatureIsEvading ||
                    _engine.CurrentCreature.IsAutomatic)
                {
                    _engine.AutoRunning.Object = true;
                    var creatureAction = await _engine.ExecuteNextAction();

                    switch (creatureAction)
                    {
                        case CreatureAction.Evade:
                            await Proceed();
                            break;
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
                        case CreatureAction.Move:
                            _boardViewModel.MoveCurrentCreature(
                                _engine.CurrentCreature,
                                _engine.CurrentCreaturePath);
                            break;
                        case CreatureAction.FailedMeleeAttack:
                            _rangedAttackSucceeded = false;
                            _boardViewModel.AnimateAttack(
                                _engine.CurrentCreature,
                                _engine.TargetCreature,
                                false);
                            break;
                        case CreatureAction.FailedRangedAttack:
                            _rangedAttackSucceeded = false;
                            _boardViewModel.AnimateAttack(
                                _engine.CurrentCreature,
                                _engine.TargetCreature,
                                true);
                            break;
                        case CreatureAction.SuccessfulMeleeAttack:
                            _rangedAttackSucceeded = true;
                            _boardViewModel.AnimateAttack(
                                _engine.CurrentCreature,
                                _engine.TargetCreature,
                                false);
                            break;
                        case CreatureAction.SuccessfulRangedAttack:
                            _rangedAttackSucceeded = true;
                            _boardViewModel.AnimateAttack(
                                _engine.CurrentCreature,
                                _engine.TargetCreature,
                                true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
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
