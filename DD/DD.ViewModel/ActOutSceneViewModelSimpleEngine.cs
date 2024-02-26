using System;
using System.Threading.Tasks;
using Craft.Logging;
using Craft.Utils;
using DD.Application;
using DD.Application.BattleEvents;
using DD.Domain;

namespace DD.ViewModel;

public class ActOutSceneViewModelSimpleEngine : ActOutSceneViewModelBase
{
    public ActOutSceneViewModelSimpleEngine(
        IEngine engine,
        BoardViewModelBase boardViewModel,
        ObservableObject<Scene> selectedScene,
        ILogger logger) : base(
            engine,
            boardViewModel,
            selectedScene,
            logger)
    {
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
    }

    protected override async Task Proceed()
    {
        if (_paused)
        {
            return;
        }

        while (!_engine.BattleDecided)
        {
            if (_engine.BattleroundCompleted)
            {
                _engine.StartBattleRound();

                if (UpdateBoard)
                {
                    _boardViewModel.UpdateCreatureViewModels(
                        _engine.Creatures,
                        _engine.CurrentCreature);
                }
            }

            if (_engine.NextEventOccursAutomatically)
            {
                _engine.AutoRunning.Object = true;
                var nextEvent = await _engine.ExecuteNextEvent();

                switch (nextEvent)
                {
                    case CreaturePass:
                        _engine.SwitchToNextCreature();
                        if (UpdateBoard)
                        {
                            _boardViewModel.UpdateCreatureViewModels(
                                _engine.Creatures,
                                _engine.CurrentCreature);
                        }
                        continue;
                    // Bemærk, at vi for de næste 3 ikke kalder continue men derimod break, dvs vi træder ud af løkken og dermed hele Proceed
                    // metoden. Den kaldes igen, når vi håndterer disse evenst: MoveCreatureAnimationCompleted, AttackAnimationCompleted
                    case CreatureMove:
                        if (AnimateMoves)
                        {
                            _boardViewModel.MoveCurrentCreature(
                                _engine.CurrentCreature,
                                _engine.CurrentCreaturePath);
                            break;
                        }
                        else
                        {
                            if (UpdateBoard)
                            {
                                _boardViewModel.UpdateCreatureViewModels(
                                    _engine.Creatures,
                                    _engine.CurrentCreature);
                            }
                            continue;
                        }
                    case CreatureAttack:
                        if (AnimateAttacks)
                        {
                            _boardViewModel.AnimateAttack(
                                _engine.CurrentCreature,
                                _engine.TargetCreature,
                                false);
                            break;
                        }
                        else
                        {
                            if (UpdateBoard)
                            {
                                _boardViewModel.UpdateCreatureViewModels(
                                    _engine.Creatures,
                                    _engine.CurrentCreature);
                            }
                            continue;
                        }
                    default:
                        throw new ArgumentException("unexpected battle event");
                }
            }
            else
            {
                _engine.AutoRunning.Object = false;

                // Diagnostics
                //_logger.WriteLine(LogMessageCategory.Information, "(Proceed method about to exit - Initiative will go to the player)");
            }

            break;
        }

        if (_engine.BattleDecided)
        {
            if (!UpdateBoard)
            {
                _boardViewModel.UpdateCreatureViewModels(
                    _engine.Creatures,
                    _engine.CurrentCreature);
            }

            if (!_engine.CurrentCreature.IsAutomatic)
            {
                _boardViewModel.ClearPlayerOptions();
            }

            _logger?.WriteLine(LogMessageCategory.Information, "Battle was decided");
        }
        else
        {
            // Diagnostics
            //_logger.WriteLine(LogMessageCategory.Information, "(Proceed method exiting)");
        }
    }
}