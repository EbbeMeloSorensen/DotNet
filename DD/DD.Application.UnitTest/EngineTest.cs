using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Craft.Utils;
using Xunit;
using FluentAssertions;

namespace DD.Application.UnitTest
{
    public class EngineTest
    {
        [Fact]
        public async Task TestMethod1()
        {
            // Arrange
            var squareIndexForCurrentCreature = new ObservableObject<int?>();
            var squareIndexesCurrentCreatureCanMoveTo = new ObservableObject<Dictionary<int, double>>();
            var squareIndexesCurrentCreatureCanAttackWithMeleeWeapon = new ObservableObject<HashSet<int>>();
            var squareIndexesCurrentCreatureCanAttackWithRangedWeapon = new ObservableObject<HashSet<int>>();

            var engine = new Engine(
                squareIndexForCurrentCreature,
                squareIndexesCurrentCreatureCanMoveTo,
                squareIndexesCurrentCreatureCanAttackWithMeleeWeapon,
                squareIndexesCurrentCreatureCanAttackWithRangedWeapon,
                null)
            {
                Scene = SceneGenerator.GenerateScene(2)
            };

            // Act
            engine.StartBattle();
            engine.StartBattleRound();
            engine.SwitchToNextCreature();

            while (true)
            {
                if (engine.BattleDecided)
                {
                    break;
                }

                if (engine.BattleroundCompleted)
                {
                    engine.StartBattleRound();
                    engine.SwitchToNextCreature();
                }

                var creatureAction = await engine.ExecuteNextAction();

                if (creatureAction == CreatureAction.Pass)
                {
                    engine.SwitchToNextCreature();
                }
            }

            // Assert
            engine.Creatures.Count.Should().Be(5);
            engine.Creatures.First().CreatureType.Name.Should().Be("Knight");
        }

        [Fact]
        public async Task TestMethod2()
        {
            // Arrange
            var squareIndexForCurrentCreature = new ObservableObject<int?>();
            var squareIndexesCurrentCreatureCanMoveTo = new ObservableObject<Dictionary<int, double>>();
            var squareIndexesCurrentCreatureCanAttackWithMeleeWeapon = new ObservableObject<HashSet<int>>();
            var squareIndexesCurrentCreatureCanAttackWithRangedWeapon = new ObservableObject<HashSet<int>>();

            var engine = new Engine(
                squareIndexForCurrentCreature,
                squareIndexesCurrentCreatureCanMoveTo,
                squareIndexesCurrentCreatureCanAttackWithMeleeWeapon,
                squareIndexesCurrentCreatureCanAttackWithRangedWeapon,
                null)
            {
                Scene = SceneGenerator.GenerateScene(16)
            };

            // Act
            engine.StartBattle();
            engine.StartBattleRound();
            engine.SwitchToNextCreature();

            while (true)
            {
                if (engine.BattleDecided)
                {
                    break;
                }

                if (engine.BattleroundCompleted)
                {
                    engine.StartBattleRound();
                    engine.SwitchToNextCreature();
                    continue;
                }

                var creatureAction = await engine.ExecuteNextAction();

                if (creatureAction == CreatureAction.Pass)
                {
                    engine.SwitchToNextCreature();
                }
            }

            // Assert
            engine.Creatures.Count.Should().Be(4);
            engine.Creatures.Count(c => c.CreatureType.Name == "Knight").Should().Be(1);
            engine.Creatures.Count(c => c.CreatureType.Name == "Archer").Should().Be(3);
        }

        [Fact]
        public async Task TestMethod3()
        {
            // Arrange
            var squareIndexForCurrentCreature = new ObservableObject<int?>();
            var squareIndexesCurrentCreatureCanMoveTo = new ObservableObject<Dictionary<int, double>>();
            var squareIndexesCurrentCreatureCanAttackWithMeleeWeapon = new ObservableObject<HashSet<int>>();
            var squareIndexesCurrentCreatureCanAttackWithRangedWeapon = new ObservableObject<HashSet<int>>();

            var engine = new Engine(
                squareIndexForCurrentCreature,
                squareIndexesCurrentCreatureCanMoveTo,
                squareIndexesCurrentCreatureCanAttackWithMeleeWeapon,
                squareIndexesCurrentCreatureCanAttackWithRangedWeapon,
                null)
            {
                Scene = SceneGenerator.GenerateScene(13)
            };

            // Act
            engine.StartBattle();
            engine.StartBattleRound();
            engine.SwitchToNextCreature();

            while (true)
            {
                if (engine.BattleDecided)
                {
                    break;
                }

                if (engine.BattleroundCompleted)
                {
                    engine.StartBattleRound();
                    engine.SwitchToNextCreature();
                    continue;
                }

                var creatureAction = await engine.ExecuteNextAction();

                if (creatureAction == CreatureAction.Pass)
                {
                    engine.SwitchToNextCreature();
                }
            }

            // Assert
            engine.Creatures.Count.Should().Be(12);
        }
    }
}