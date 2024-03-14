using Games.Pig.Application.GameEvents;

namespace Games.Pig.Application.UnitTest
{
    public class EngineTest
    {
        [Fact]
        public async Task Test1()
        {
            // Arrange
            var players = new bool[] { true, true, true };
            var engine = new Engine(players);
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
                        case PlayerRollsDie playerRollsDie:
                            {
                                var logMessage =
                                    $"Player {playerRollsDie.Player} rolls die and gets {playerRollsDie.DieRoll}";

                                if (playerRollsDie.DieRoll == 1)
                                {
                                    logMessage += " => Bad luck: Player looses the turn and pot is reset";
                                }
                                else
                                {
                                    logMessage += $" => Raising the stakes: Pot is now at {engine.Pot}";
                                }

                                log.Add(logMessage);
                                break;
                            }
                        case PlayerTakesPot playerTakesPot:
                            {
                                var logMessage =
                                    $"Player {playerTakesPot.Player} takes pot";

                                log.Add(logMessage);
                                break;
                            }
                        default:
                            {
                                throw new InvalidOperationException("Unknown game event");
                            }
                    }
                }
            }
        }
    }
}