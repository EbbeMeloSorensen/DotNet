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
        BoardViewModel boardViewModel,
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
    }

    protected override async Task Proceed()
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
}