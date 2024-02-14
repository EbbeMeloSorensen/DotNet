using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using DD.Engine.Complex;

namespace DD.Application.UnitTest
{
    public class EngineTest
    {
        [Fact]
        public async Task TestMethod1()
        {
            // Arrange
            var engine = new ComplexEngine(
                null)
            {
                Scene = SceneGenerator.GenerateScene(2)
            };

            // Act
            engine.StartBattle();

            while (!engine.BattleDecided)
            {
                if (engine.BattleroundCompleted)
                {
                    engine.StartBattleRound();
                }

                var nextEvent = await engine.ExecuteNextEvent();

                if (nextEvent == CreatureAction.Pass)
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
            var engine = new ComplexEngine(
                null)
            {
                Scene = SceneGenerator.GenerateScene(16)
            };

            // Act
            engine.StartBattle();

            while (!engine.BattleDecided)
            {
                if (engine.BattleroundCompleted)
                {
                    engine.StartBattleRound();
                }

                var nextEvent = await engine.ExecuteNextEvent();

                if (nextEvent == CreatureAction.Pass)
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
            var engine = new ComplexEngine(
                null)
            {
                Scene = SceneGenerator.GenerateScene(13)
            };

            // Act
            engine.StartBattle();

            while (!engine.BattleDecided)
            {
                if (engine.BattleroundCompleted)
                {
                    engine.StartBattleRound();
                    continue;
                }

                var nextEvent = await engine.ExecuteNextEvent();

                if (nextEvent == CreatureAction.Pass)
                {
                    engine.SwitchToNextCreature();
                }
            }

            // Assert
            engine.Creatures.Count.Should().Be(12);
        }
    }
}