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
                                    logMessage += " => Player looses turn and pot is reset";
                                }
                                else
                                {
                                    logMessage += $" => Pot is now at {engine.Pot}";
                                }

                                log.Add(logMessage);
                                break;
                            }
                        case PlayerTakesPot playerTakesPot:
                            {
                                var logMessage =
                                    $"Player {playerTakesPot.Player} takes pot and now has a score of {playerTakesPot.NewScore}";

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

            log.Add($"Player {engine.CurrentPlayerIndex + 1} wins");

            await File.WriteAllLinesAsync("PigGameLog.txt", log);
        }
    }
}