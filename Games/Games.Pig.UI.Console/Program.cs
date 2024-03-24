using System.Text;
using Games.Pig.Application;
using Games.Pig.Application.GameEvents;
using Games.Pig.Application.PlayerOptions;
using Microsoft.VisualBasic;

namespace Games.Pig.UI.Console;

internal class Program
{
    static async Task Main(string[] args)
    {
        System.Console.WriteLine("Welcome to the Pig Game");
        WaitForPlayerToProceed();

        while(true)
        {
            System.Console.Clear();
            await PlayGame();

            System.Console.WriteLine("Do you want to play again? (yes/no): ");

            var answer = System.Console.ReadLine();

            if (answer == null || (answer.ToUpper() != "YES" && answer.ToUpper() != "Y"))
            {
                break;
            }
        }
    }

    private static async Task PlayGame()
    {
        var players = new[] { true, false };
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
                            var logMessage = $"Computer rolls die and gets {playerRollsDie.DieRoll}";
                            
                            if (playerRollsDie.DieRoll == 1)
                            {
                                logMessage += " => Computer looses turn and pot is reset";
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
                                $"Computer takes pot and now has a score of {playerTakesPot.NewScore}";

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
                if (engine.Pot == 0)
                {
                    WaitForPlayerToProceed();
                }

                var choice = "";

                System.Console.Clear();
                System.Console.WriteLine("Scores:");
                System.Console.WriteLine($"  Computer: {engine.PlayerScores[0],3}");
                System.Console.WriteLine($"  Player:   {engine.PlayerScores[1],3}");
                System.Console.WriteLine();
                System.Console.WriteLine($"Pot: {engine.Pot}");
                System.Console.WriteLine();

                var options = new Dictionary<int, string>
                {
                    {1, "Roll die"}
                };

                if (engine.Pot > 0)
                {
                    options[2] = "Take pot";
                }

                var messageBuilder = new StringBuilder("Please select an option:\n");

                foreach (var kvp in options)
                {
                    messageBuilder.Append($"  {kvp.Key}: {kvp.Value}\n");
                }

                messageBuilder.Append("\nYour choice: ");

                System.Console.Write(messageBuilder.ToString());

                do
                {
                    choice = System.Console.ReadLine();

                    if (ChoiceIsInvalid(options, choice))
                    {
                        System.Console.Write("Invalid input, please try again: ");
                    }

                } while (ChoiceIsInvalid(options, choice));

                switch (choice)
                {
                    case "1":
                        {
                            var gameEvent = await engine.PlayerSelectsOption(new RollDie()) as PlayerRollsDie;
                            var logMessage = $"You roll a {gameEvent.DieRoll}";

                            if (gameEvent.DieRoll == 1)
                            {
                                logMessage += $", so unfortunately you loose your turn and the pot is reset";
                            }
                            else
                            {
                                logMessage += $", and the pot increases to {engine.Pot}";
                            }

                            System.Console.WriteLine(logMessage);
                            WaitForPlayerToProceed();
                            break;
                        }
                    case "2":
                        {
                            var gameEvent = await engine.PlayerSelectsOption(new TakePot()) as PlayerTakesPot;
                            System.Console.WriteLine($"You take the pot and now have a score of {gameEvent.NewScore}");
                            WaitForPlayerToProceed();
                            break;
                        }
                }

                System.Console.Clear();
            }
        }

        System.Console.WriteLine(engine.CurrentPlayerIndex == 0
            ? "You lost the game"
            : "Congratulations. You won the game!");
    }

    private static void WaitForPlayerToProceed()
    {
        System.Console.WriteLine("\nPress any key to continue..");
        System.Console.ReadKey();
    }

    private static bool ChoiceIsInvalid(
        Dictionary<int, string> options,
        string input)
    {
        return !options.Keys.Select(_ => $"{_}").Contains(input);
    }
}