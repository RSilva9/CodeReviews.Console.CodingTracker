using RSilva9.CodingTracker.Models;
using Spectre.Console;
using System.Globalization;

namespace RSilva9.CodingTracker.Controllers
{
    internal static class GoalController
    {
        internal static void ViewGoals()
        {
            Goal[] fetchedGoals = Database.GetGoals();

            AnsiConsole.Write(new Rule("[orange3]Goals[/]").LeftJustified());
            var goalChoices = fetchedGoals.Select(s =>
            new {
                Session = s,
                Display = $"#{s.Id} | {s.StartDate} | {s.EndDate} | " +
                $"{Utils.RenderProgressBar(s.GoalHours, s.CurrentHours)}"
            }).ToList();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices("Back")
                    .AddChoices(goalChoices.Select(x => x.Display))
            );

            if (selection == "Back")
            {
                Console.Clear();
                return;
            }

            var chosen = goalChoices.First(x => x.Display == selection).Session;
            Console.Clear();

            AnsiConsole.Write(new Rule($"[orange3]Goal #{chosen.Id}[/]").LeftJustified());
            AnsiConsole.WriteLine(Utils.RenderProgressBar(chosen.GoalHours, chosen.CurrentHours));
            AnsiConsole.WriteLine($"You have coded for {chosen.CurrentHours}/{chosen.GoalHours} hours");
            if (chosen.Completed)
            {
                AnsiConsole.WriteLine($"Congrats! You have completed this goal!");
            }
            else
            {
                AnsiConsole.WriteLine($"In order to reach the goal before {chosen.EndDate}, " +
                    $"you should code {Utils.CalculateHoursPerDay(chosen.EndDate, chosen.GoalHours, chosen.CurrentHours)} hours per day");
            }

            Console.ReadLine();
        }

        internal static void CreateGoal()
        {
            AnsiConsole.Write(new Rule("[orange3]Create Goal[/]").LeftJustified());

            string today = DateTime.Now.ToString("dd-MM-yyyy");

            string goalDate = Validations.AskValidatedInput(
                "Date",
                "Please enter the final date of the goal (dd-MM-yyyy):",
                Validations.isDateValid
            );

            while(DateTime.ParseExact(goalDate, "dd-MM-yyyy", CultureInfo.InvariantCulture) <= DateTime.Now)
            {
                goalDate = Validations.AskValidatedInput(
                    "Date",
                    "Final date can't be equal or earlier than start date. Please, enter final date again (dd-MM-yyyy):",
                    Validations.isDateValid
                );
            }

            string goalHours = Validations.AskValidatedInput(
                "Date",
                "Please enter the goal objective in hours:",
                Validations.isGoalTimeValid
            );

            Goal newGoal = new Goal
            {
                StartDate = today,
                EndDate = goalDate,
                CurrentHours = 0,
                GoalHours = float.Parse(goalHours)
            };

            Database.CreateGoal(newGoal);

            AnsiConsole.WriteLine("New goal created!");
            Console.ReadKey();
            Console.Clear();
        }

        internal static void UpdateGoal()
        {
            Goal[] fetchedGoals = Database.GetGoals();

            AnsiConsole.Write(new Rule("[orange3]Update Session[/]").LeftJustified());
            var goalChoices = fetchedGoals.Select(s =>
            new {
                Session = s,
                Display = $"#{s.Id} | {s.StartDate} | {s.EndDate} | " +
                $"{Utils.RenderProgressBar(s.GoalHours, s.CurrentHours)}"
            }).ToList();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices("Back")
                    .AddChoices(goalChoices.Select(x => x.Display))
            );

            if (selection == "Back")
            {
                Console.Clear();
                return;
            }

            var chosen = goalChoices.First(x => x.Display == selection).Session;
            var fieldOptions = new Dictionary<string, string>
            {
                { "Start Date", $"Start Date: {chosen.StartDate}" },
                { "End Date", $"End Date: {chosen.EndDate}" },
                { "Goal Hours", $"Goal Hours: {chosen.GoalHours} hours" },
            };

            var selectedLabel = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Goal #{chosen.Id}")
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
                { "Start Date", Validations.isDateValid },
                { "End Date", Validations.isDateValid },
                { "Goal Hours", Validations.isGoalTimeValid }
            };

            string newValue = Validations.AskValidatedInput(
                selectedField,
                $"Enter new {selectedField}:",
                fieldValidations[selectedField]
            );

            if(selectedField == "End Date")
            {
                while (DateTime.ParseExact(newValue, "dd-MM-yyyy", CultureInfo.InvariantCulture) <= DateTime.ParseExact(chosen.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                {
                    newValue = Validations.AskValidatedInput(
                        "Date",
                        "Final date can't be equal or earlier than start date. Please, enter final date again (dd-MM-yyyy):",
                        Validations.isDateValid
                    );
                }
            }

            if(selectedField == "Goal Hours")
            {
                Database.UpdateGoal(chosen.Id, "Completed", chosen.CurrentHours >= float.Parse(newValue));
            }

            Database.UpdateGoal(chosen.Id, Utils.ReplaceAllWhiteSpaces(selectedField), newValue);

            Console.Clear();
        }

        internal static void UpdateGoalProgress(string recordDate, double newProgress)
        {
            string parsedRecordDate = DateTime.ParseExact(recordDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            Goal[] fetchedGoals = Database.GetGoalsByDate(parsedRecordDate);

            foreach (var goal in fetchedGoals)
            {
                Database.UpdateGoal(goal.Id, "CurrentHours", goal.CurrentHours + newProgress);

                if(goal.CurrentHours + newProgress >= goal.GoalHours)
                {
                    Database.UpdateGoal(goal.Id, "Completed", true);
                }
            }
        }

        internal static void DeleteGoal()
        {
            Goal[] fetchedGoals = Database.GetGoals();

            AnsiConsole.Write(new Rule("[orange3]Delete Goal[/]").LeftJustified());
            var goalChoices = fetchedGoals.Select(s =>
            new {
                Session = s,
                Display = $"#{s.Id} | {s.StartDate} | {s.EndDate} | " +
                $"{Utils.RenderProgressBar(s.GoalHours, s.CurrentHours)}"
            }).ToList();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices("Back")
                    .AddChoices(goalChoices.Select(x => x.Display))
            );

            if (selection == "Back")
            {
                Console.Clear();
                return;
            }

            var chosen = goalChoices.First(x => x.Display == selection).Session;
            var confirmation = AnsiConsole.Prompt(
                new TextPrompt<bool>($"Are you sure you want to delete goal #{chosen.Id}?")
                    .AddChoice(true)
                    .AddChoice(false)
                    .WithConverter(choice => choice ? "y" : "n"));

            if (confirmation)
            {
                Database.DeleteGoal(chosen.Id);
                Console.Clear();
            }
            else
            {
                Console.Clear();
                return;
            }
        }
    }
}
