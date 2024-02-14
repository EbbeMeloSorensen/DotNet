using System.Collections.Generic;
using System.Threading.Tasks;
using Craft.Logging;
using Craft.Utils;
using DD.Domain;

namespace DD.Application
{
    public interface IBattleEvent
    {
    }

    // The NoEvent class is used by the engine to inform its host that even though the engine passes control to the host,
    // the host should pass back control to the engine immediately by calling the main loop that e.g. includes ExecuteNextAction
    public class NoEvent : IBattleEvent
    {
    }

    public class CreaturePass : IBattleEvent
    {
    }

    public class CreatureMove : IBattleEvent
    {
    }

    public class CreatureAttack : IBattleEvent
    {
    }

    public class CreatureAttackMelee : IBattleEvent
    {
    }

    public class CreatureAttackRanged : IBattleEvent
    {
    }

    public enum CreatureAction
    {
        NoAction,
        Move,
        MeleeAttack,
        RangedAttack,
        Pass
    }

    public class MoveCreatureResult
    {
        public int? IndexOfDestinationSquare { get; set; }
        public double? WalkingDistanceToDestinationSquare { get; set; }
        public Creature FinalClosestOpponent { get; set; }
        public double FinalDistanceToClosestOpponent { get; set; }
    }

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

        Task<CreatureAction> ExecuteNextEvent();

        CreatureAction? PlayerSelectSquare(
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
