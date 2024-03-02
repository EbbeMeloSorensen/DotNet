using FluentAssertions;
using Games.Race.Application.GameEvents;

namespace Games.Race.Application.UnitTest
{
    public class EngineTest
    {
        [Fact]
        public async Task Test1()
        {
            // Arrange
            var engine = new Engine(4);
            var log = new List<string>();

            // Act
            engine.StartGame();

            while (!engine.GameDecided)
            {
                var nextEvent = await engine.ExecuteNextEvent();

                switch (nextEvent)
                {
                    case PlayerAdvances playerAdvances:
                    {
                        var logMessage =
                            $"Player {playerAdvances.Player} advances {playerAdvances.Squares} squares to a total of {playerAdvances.Total}";

                        log.Add(logMessage);
                        break;
                    }
                    default:
                    {
                        throw new InvalidOperationException("Unknown game event");
                    }
                }
            }

            log.Add($"Player {engine.CurrentPlayerIndex + 1} wins");

            // Assert
            log.Last().Should().Be("Player 1 wins");
        }
    }
}