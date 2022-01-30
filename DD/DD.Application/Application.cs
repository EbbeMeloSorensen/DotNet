using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Craft.Logging;
using Craft.Utils;

namespace DD.Application
{
    public class Application
    {
        public IUIDataProvider UIDataProvider { get; }

        // It must be possible for an external component to set the Logger, e.g. in order to override with a decorator
        public ILogger Logger { get; set; }

        public Engine Engine { get; }

        public ObservableObject<int?> SquareIndexForCurrentCreature { get; }
        public ObservableObject<Dictionary<int, double>> SquareIndexesCurrentCreatureCanMoveTo { get; }
        public ObservableObject<HashSet<int>> SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon { get; }
        public ObservableObject<HashSet<int>> SquareIndexesCurrentCreatureCanAttackWithRangedWeapon { get; }

        public Application(
            IUIDataProvider uiDataProvider,
            ILogger logger)
        {
            UIDataProvider = uiDataProvider;
            Logger = logger;

            SquareIndexForCurrentCreature = new ObservableObject<int?>();
            SquareIndexesCurrentCreatureCanMoveTo = new ObservableObject<Dictionary<int, double>>();
            SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon = new ObservableObject<HashSet<int>>();
            SquareIndexesCurrentCreatureCanAttackWithRangedWeapon = new ObservableObject<HashSet<int>>();

            Engine = new Engine(
                SquareIndexForCurrentCreature,
                SquareIndexesCurrentCreatureCanMoveTo,
                SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon,
                SquareIndexesCurrentCreatureCanAttackWithRangedWeapon, 
                Logger);
        }

        public async Task ActOutBattle()
        {
            await Task.Run(async () =>
            {
                Engine.StartBattle();
                Engine.StartBattleRound();
                Engine.SwitchToNextCreature();

                while (true)
                {
                    if (Engine.BattleDecided)
                    {
                        break;
                    }

                    if (Engine.BattleroundCompleted)
                    {
                        Engine.StartBattleRound();
                        Engine.SwitchToNextCreature();
                    }

                    var creatureAction = await Engine.ExecuteNextAction();

                    if (creatureAction == CreatureAction.Pass)
                    {
                        Engine.SwitchToNextCreature();
                    }
                }
            });
        }
    }
}
