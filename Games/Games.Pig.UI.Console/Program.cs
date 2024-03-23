using Games.Pig.Application;
using Games.Pig.Application.GameEvents;

namespace Games.Pig.UI.Console;

internal class Program
{
    static async Task Main(string[] args)
    {
        var players = new [] {true, false};
        var engine = new Engine(players);

        while (!engine.GameDecided)
        {
            if (engine.NextEventOccursAutomatically)
            {
                var nextEvent = await engine.ExecuteNextEvent();

                switch (nextEvent)
                {
                    case PlayerRollsDie playerRollsDie:
                    {
                        var logMessage = $"Player {playerRollsDie.Player} rolls die and gets {playerRollsDie.DieRoll}";
                        System.Console.WriteLine(logMessage);

                        if (playerRollsDie.DieRoll == 1)
                        {
                            logMessage += " => Player looses turn and pot is reset";
                        }
                        else
                        {
                            logMessage += $" => Pot is now at {engine.Pot}";
                        }

                        System.Console.WriteLine(logMessage);

                        break;
                    }
                    case PlayerTakesPot playerTakesPot:
                    {
                        var logMessage =
                            $"Player {playerTakesPot.Player} takes pot and now has a score of {playerTakesPot.NewScore}";

                        System.Console.WriteLine(logMessage);

                        break;
                    }
                    default:
                    {
                        throw new InvalidOperationException("Unknown game event");
                    }
                }
            }
            else
            {
                // Player has the initiative

                var choice = "";

                System.Console.Clear();
                System.Console.WriteLine("Scores:");
                System.Console.WriteLine($"  Computer: {engine.PlayerScores[0],3}");
                System.Console.WriteLine($"  Player:   {engine.PlayerScores[1],3}");
                System.Console.WriteLine();
                System.Console.WriteLine($"Pot: {engine.Pot}");
                System.Console.WriteLine();
                System.Console.Write("Please select an option:\n  1: Roll die\n  2: Take pot\n\nYour choice: ");

                do
                {
                    choice = System.Console.ReadLine();

                    if (choice != "1" && choice != "2")
                    {
                        System.Console.Write("Invalid input, please try again: ");
                    }

                } while (choice != "1" && choice != "2");

                System.Console.WriteLine($"You chose {choice}");
                // Skal du køre noget proceed, lige som med DD?

                break;
            }
        }
    }
}