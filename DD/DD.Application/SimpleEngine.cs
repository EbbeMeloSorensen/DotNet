using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Craft.Logging;
using Craft.Utils;
using DD.Domain;

namespace DD.Application
{
    public class SimpleEngine : IEngine
    {
        public ILogger Logger { get; set; }
        public int[] CurrentCreaturePath { get; set; }
        public bool BattleroundCompleted { get; set; }
        public bool BattleDecided { get; set; }
        public Scene Scene { get; set; }
        public List<Creature> Creatures { get; set; }
        public Creature CurrentCreature { get; set; }
        public Creature TargetCreature { get; set; }
        public ObservableObject<int?> SquareIndexForCurrentCreature { get; }
        public ObservableObject<Dictionary<int, double>> SquareIndexesCurrentCreatureCanMoveTo { get; }
        public ObservableObject<HashSet<int>> SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon { get; }
        public ObservableObject<HashSet<int>> SquareIndexesCurrentCreatureCanAttackWithRangedWeapon { get; }
        public ObservableObject<bool> BattleHasStarted { get; }
        public ObservableObject<bool> BattleHasEnded { get; }
        public ObservableObject<bool> AutoRunning { get; }
        public bool NextEventOccursAutomatically { get; }
        public Task<CreatureAction> ExecuteNextAction()
        {
            throw new NotImplementedException();
        }

        public CreatureAction PlayerSelectSquare(int squareIndex)
        {
            throw new NotImplementedException();
        }

        public void StartBattle()
        {
            throw new NotImplementedException();
        }

        public bool CurrentPlayerControlledCreatureHasAnyOptionsLeft()
        {
            throw new NotImplementedException();
        }

        public bool CanStartBattle()
        {
            throw new NotImplementedException();
        }

        public void InitializeCreatures()
        {
            throw new NotImplementedException();
        }

        public void StartBattleRound()
        {
            throw new NotImplementedException();
        }

        public void SwitchToNextCreature()
        {
            throw new NotImplementedException();
        }

        public string Tag(Creature creature)
        {
            throw new NotImplementedException();
        }
    }
}
