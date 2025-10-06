using RSilva9.CodingTracker.Models;
using Spectre.Console;
using System.Diagnostics;

namespace RSilva9.CodingTracker.Controllers
{
    internal static class CodingController
    {
        internal static void GetCodingRecords()
        {
            CodingSession[] fetchedSessions = Database.GetCodingRecords();
            AnsiConsole.Write(new Rule("[orange3]Coding Sessions[/]").LeftJustified());
            AnsiConsole.WriteLine();

            var panels = new List<Panel>();

            foreach (var session in fetchedSessions)
            {
                var panelContent = new List<string>
                {
                    $"[bold]Date:[/] {session.Date}",
                    $"[bold]Start Time:[/] {session.StartTime}",
                    $"[bold]End Time:[/] {session.EndTime}",
                    $"[bold]Duration:[/] {session.Duration}"
                };

                var panel = new Panel(string.Join("\n", panelContent))
                {
                    Border = BoxBorder.Rounded,
                    Header = new PanelHeader($"Session #{session.Id}", Justify.Center)
                };

                panels.Add(panel);
            }
            AnsiConsole.Write(new Columns(panels));

            Console.ReadLine();
            Console.Clear();
        }

        internal static void InsertCodingRecord()
        {
            string sessionDate = Validations.AskValidatedInput(
                "Date",
                "Please enter the date of the coding session you want to record (dd-MM-yyyy):",
                Validations.isDateValid
            );

            string startTime = Validations.AskValidatedInput(
                "Start Time",
                "Please enter session Start Sime (hh:mm):",
                Validations.isTimeValid
            );

            string endTime = Validations.AskValidatedInput(
                "End Time",
                "Please enter session End Time. Make sure it is not equal to Start Time (hh:mm):",
                Validations.isTimeValid
            );

            if(endTime == startTime)
            {
                do
                {
                    endTime = Validations.AskValidatedInput(
                        "End Time",
                        "Session End Time can not be equal to Start Time. Please, enter again (hh:mm):",
                        Validations.isTimeValid
                    );
                } while (endTime == startTime);
            }

            string sessionDuration = Utils.CalculateDuration(startTime, endTime);

            AnsiConsole.Write(new Rule("[orange3]Total Duration[/]").LeftJustified());
            AnsiConsole.WriteLine($"The total duration of the session is: {sessionDuration}");
            Console.ReadKey();
            CodingSession newCodingSession = new CodingSession
            {
                Date = sessionDate,
                StartTime = startTime,
                EndTime = endTime,
                Duration = sessionDuration
            };
            Database.InsertCodingRecord(newCodingSession);

            double sessionHours = TimeSpan.Parse(sessionDuration).TotalHours;
            GoalController.UpdateGoalProgress(sessionDate, sessionHours);

            Console.Clear();
        }

        internal static void UpdateCodingRecord()
        {
            CodingSession[] fetchedSessions = Database.GetCodingRecords();

            AnsiConsole.Write(new Rule("[orange3]Update Session[/]").LeftJustified());
            var sessionChoices = fetchedSessions.Select(s =>
            new {
                Session = s,
                Display = $"#{s.Id} | {s.Date} | {s.StartTime}-{s.EndTime} | {s.Duration}"
            }).ToList();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices("Back")
                    .AddChoices(sessionChoices.Select(x => x.Display))
            );

            if(selection == "Back")
            {
                Console.Clear();
                return;
            }

            var chosen = sessionChoices.First(x => x.Display == selection).Session;
            var fieldOptions = new Dictionary<string, string>
            {
                { "Date", $"Date: {chosen.Date}" },
                { "Start Time", $"Start Time: {chosen.StartTime}" },
                { "End Time", $"End Time: {chosen.EndTime}" },
                { "Duration", $"Duration: {chosen.Duration}" }
            };

            var selectedLabel = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Session #{chosen.Id}")
                    .AddChoices("Back")
                    .AddChoices(fieldOptions.Values)
            );

            if (selectedLabel == "Back")
            {
                Console.Clear();
                return;
            }

            string selectedField = fieldOptions.First(x => x.Value == selectedLabel).Key;

            var fieldValidations = new Dictionary<string, Func<string, bool>>
            {
                { "Date", Validations.isDateValid },
                { "Start Time", Validations.isTimeValid },
                { "End Time", Validations.isTimeValid },
                { "Duration", Validations.isTimeValid }
            };

            string newValue = Validations.AskValidatedInput(
                selectedField,
                $"Enter new {selectedField}:",
                fieldValidations[selectedField]
            );

            string newDuration = "";
            if(selectedField == "Start Time")
            {
                newDuration = Utils.CalculateDuration(newValue, chosen.EndTime);
                Database.UpdateCodingRecord(chosen.Id, "Duration", newDuration);

            }
            else if (selectedField == "End Time")
            {
                newDuration = Utils.CalculateDuration(chosen.StartTime, newValue);
                Database.UpdateCodingRecord(chosen.Id, "Duration", newDuration);
            }

            Database.UpdateCodingRecord(chosen.Id, Utils.ReplaceAllWhiteSpaces(selectedField), newValue);

            if (selectedField == "Start Time" || selectedField == "End Time")
            {
                double sessionHours = TimeSpan.Parse(newDuration).TotalHours;
                GoalController.UpdateGoalProgress(chosen.Date, sessionHours);
            }

            Console.Clear();
        }

        internal static void DeleteCodingRecord()
        {
            CodingSession[] fetchedSessions = Database.GetCodingRecords();

            AnsiConsole.Write(new Rule("[orange3]Delete Session[/]").LeftJustified());
            var sessionChoices = fetchedSessions.Select(s =>
            new {
                Session = s,
                Display = $"#{s.Id} | {s.Date} | {s.StartTime}-{s.EndTime} | {s.Duration}"
            }).ToList();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices("Back")
                    .AddChoices(sessionChoices.Select(x => x.Display))
            );

            if (selection == "Back")
            {
                Console.Clear();
                return;
            }

            var chosen = sessionChoices.First(x => x.Display == selection).Session;
            var confirmation = AnsiConsole.Prompt(
                new TextPrompt<bool>($"Are you sure you want to delete record #{chosen.Id}?")
                    .AddChoice(true)
                    .AddChoice(false)
                    .WithConverter(choice => choice ? "y" : "n"));

            if (confirmation)
            {
                Database.DeleteCodingRecord(chosen.Id);
                double sessionHours = TimeSpan.Parse(chosen.Duration).TotalHours;
                GoalController.UpdateGoalProgress(chosen.Date, -sessionHours);
                Console.Clear();
            }
            else
            {
                Console.Clear();
                return;
            }
        }

        internal static void CodingStopwatch()
        {
            Stopwatch stopwatch = new Stopwatch();

            var confirmation = AnsiConsole.Prompt(
                new TextPrompt<bool>("Start coding session?")
                    .AddChoice(true)
                    .AddChoice(false)
                    .WithConverter(choice => choice ? "y" : "n"));

            string today = DateTime.Now.ToString("dd-MM-yyyy");
            string startTime = DateTime.Now.ToString("HH:mm");

            if (confirmation)
            {
                stopwatch.Start();
            }

            AnsiConsole.WriteLine("Press ENTER to stop the timer.");
            while (stopwatch.IsRunning)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        // The stopwatch serves only as display, since its values are not used in the calculation of duration.
                        stopwatch.Stop();
                        Console.Clear();
                        string endTime = DateTime.Now.ToString("HH:mm");

                        if (endTime.Equals(startTime))
                        {
                            AnsiConsole.WriteLine("Session too short. Aborting...");
                            Console.ReadLine();
                            Console.Clear();
                            return;
                        }

                        string sessionDuration = Utils.CalculateDuration(startTime, endTime);
                        AnsiConsole.WriteLine($"Session completed!\n" +
                            $"Date: {today}\n" +
                            $"Start Time: {startTime}\n" +
                            $"End Time: {endTime}\n" +
                            $"Total session duration: {Utils.CalculateDuration(startTime, endTime)}\n");
                        var saveSession = AnsiConsole.Prompt(
                            new TextPrompt<bool>("Do you wish to save the session?")
                                .AddChoice(true)
                                .AddChoice(false)
                                .WithConverter(choice => choice ? "y" : "n"));

                        if (saveSession)
                        {
                            CodingSession newCodingSession = new CodingSession
                            {
                                Date = today,
                                StartTime = startTime,
                                EndTime = endTime,
                                Duration = sessionDuration
                            };
                            Database.InsertCodingRecord(newCodingSession);
                        }
                        break;
                    }
                }

                Console.SetCursorPosition(0, 0);
                AnsiConsole.WriteLine(
                    $"Elapsed Time: " +
                    $"{stopwatch.Elapsed.Hours} {(stopwatch.Elapsed.Hours == 1 ? "hour" : "hours")} " +
                    $"{stopwatch.Elapsed.Minutes} {(stopwatch.Elapsed.Minutes == 1 ? "minute" : "minutes")} " +
                    $"and {stopwatch.Elapsed.Seconds} seconds");
                Thread.Sleep(1000);
            }
        }
    }
}
