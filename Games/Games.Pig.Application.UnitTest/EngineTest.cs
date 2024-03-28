using Games.Pig.Application.GameEvents;

namespace Games.Pig.Application.UnitTest
{
    public class EngineTest
    {
        [Fact]
        public async Task Test1()
        {
            // Arrange
            var players = new [] { true, true, true };
            var engine = new Engine(players, true);
            var log = new List<string>();

            // Act
            engine.StartGame();

            while (!engine.GameDecided)
            {
                if (engine.NextEventOccursAutomatically)
                {
                    var nextEvent = await engine.ExecuteNextEvent();

                    switch (nextEvent)
                    {
                        case PlayerRollsDie gameEvent:
                            {
                                log.Add(gameEvent.Description);
                                break;
                            }
                        case PlayerTakesPot gameEvent:
                            {
                                log.Add(gameEvent.Description);
                                break;
                            }
                        default:
                            {
                                throw new InvalidOperationException("Unknown game event");
                            }
                    }
                }
            }

            log.Add($"Player {engine.CurrentPlayerIndex + 1} wins");

            await File.WriteAllLinesAsync("PigGameLog.txt", log);
        }
    }
}