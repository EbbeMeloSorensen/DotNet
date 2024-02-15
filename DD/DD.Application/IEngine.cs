using System.Collections.Generic;
using System.Threading.Tasks;
using Craft.Logging;
using Craft.Utils;
using DD.Domain;
using DD.Application.BattleEvents;

namespace DD.Application
{
    public interface IEngine
    {
        ILogger Logger { get; set; }

        int[] CurrentCreaturePath { get; set; }

        bool BattleroundCompleted { get; set; }

        bool BattleDecided { get; set; }

        Scene Scene { get; set; }

        List<Creature> Creatures { get; set; }

        Creature CurrentCreature { get; set; }

        Creature TargetCreature { get; set; }

        ObservableObject<int?> SquareIndexForCurrentCreature { get; }

        ObservableObject<Dictionary<int, double>> SquareIndexesCurrentCreatureCanMoveTo { get; }

        ObservableObject<HashSet<int>> SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon { get; }

        ObservableObject<HashSet<int>> SquareIndexesCurrentCreatureCanAttackWithRangedWeapon { get; }

        ObservableObject<bool> BattleHasStarted { get; }

        ObservableObject<bool> BattleHasEnded { get; }

        ObservableObject<bool> AutoRunning { get; }

        bool NextEventOccursAutomatically { get; }

        void Randomize();

        Task<IBattleEvent> ExecuteNextEvent();

        IBattleEvent? PlayerSelectSquare(
            int squareIndex);

        void StartBattle();

        bool CurrentPlayerControlledCreatureHasAnyOptionsLeft();

        bool CanStartBattle();

        void InitializeCreatures();

        void StartBattleRound();

        void SwitchToNextCreature();

        public string Tag(
            Creature creature);
    }
}
