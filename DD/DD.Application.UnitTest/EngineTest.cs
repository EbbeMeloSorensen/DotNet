using System.Linq;
using System.Threading.Tasks;
using DD.Application.BattleEvents;
using Xunit;
using FluentAssertions;
using DD.Engine.Complex;

namespace DD.Application.UnitTest
{
    public class EngineTest
    {
        [Fact]
        public async Task SevenKnights_vs_FiftyGoblins_16x16_SquareTiles()
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

                if (nextEvent is CreaturePass)
                {
                    engine.SwitchToNextCreature();
                }
            }

            // Assert
            engine.Creatures.Count.Should().Be(5);
            engine.Creatures.First().CreatureType.Name.Should().Be("Knight");
        }

        [Fact]
        public async Task Humans_vs_Goblins_12x12_SquareTiles()
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

                if (nextEvent is CreaturePass)
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
        public async Task Archer_vs_TwelveGoblins_5x10_SquareTiles()
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

                if (nextEvent is CreaturePass)
                {
                    engine.SwitchToNextCreature();
                }
            }

            // Assert
            engine.Creatures.Count.Should().Be(12);
        }

        [Fact]
        public async Task Knight_vs_Skeleton_2x2_SquareTiles()
        {
            // Arrange
            var engine = new SimpleEngine(
                null)
            {
                Scene = SceneGenerator.GenerateScene(18)
            };

            engine.BoardTileMode = BoardTileMode.Square;

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

                if (nextEvent is CreaturePass)
                {
                    engine.SwitchToNextCreature();
                }
            }

            // Assert
            engine.Creatures.Count.Should().Be(1);
            engine.Creatures.Single().CreatureType.Name.Should().Be("Knight");
        }

        [Fact]
        public async Task Knight_vs_Skeleton_ExcludingMove_2x2_HexagonalTiles()
        {
            // Arrange
            var engine = new SimpleEngine(
                null)
            {
                Scene = SceneGenerator.GenerateScene(18)
            };

            engine.BoardTileMode = BoardTileMode.Hexagonal;

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

                if (nextEvent is CreaturePass)
                {
                    engine.SwitchToNextCreature();
                }
            }

            // Assert
            engine.Creatures.Count.Should().Be(1);
            engine.Creatures.Single().CreatureType.Name.Should().Be("Knight");
        }

        [Fact]
        public async Task Knight_vs_Skeleton_IncludingMove_2x2_HexagonalTiles()
        {
            // Arrange
            var engine = new SimpleEngine(
                null)
            {
                Scene = SceneGenerator.GenerateScene(24)
            };

            engine.BoardTileMode = BoardTileMode.Hexagonal;

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

                if (nextEvent is CreaturePass)
                {
                    engine.SwitchToNextCreature();
                }
            }

            // Assert
            engine.Creatures.Count.Should().Be(1);
            engine.Creatures.Single().CreatureType.Name.Should().Be("Knight");
        }
    }
}