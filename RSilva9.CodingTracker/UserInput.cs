using RSilva9.CodingTracker.Controllers;
using Spectre.Console;

namespace RSilva9.CodingTracker
{
    internal static class UserInput
    {
        internal static void MainMenu()
        {
            bool closeApp = false;

            while (!closeApp)
            {
                Console.Clear();

                AnsiConsole.Write(new Spectre.Console.Rule("[orange3]Main Menu[/]").LeftJustified());
                AnsiConsole.WriteLine();

                var selectedMenuOption = AnsiConsole.Prompt(
                    new SelectionPrompt<MainMenuOptions>()
                    .UseConverter(option =>
                    {
                        string name = option.ToString();
                        return name.Replace("_", " ");
                    })
                    .AddChoices(Enum.GetValues<MainMenuOptions>())
                );

                Console.Clear();

                switch (selectedMenuOption)
                {
                    case MainMenuOptions.Close_Application:
                        closeApp = true;
                        break;
                    case MainMenuOptions.Records_Menu:
                        RecordsMenu();
                        break;
                    case MainMenuOptions.Reports_Menu:
                        ReportsMenu();
                        break;
                    case MainMenuOptions.Goals_Menu:
                        GoalsMenu();
                        break;
                }
            }
        }

        internal static void RecordsMenu()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();

                AnsiConsole.Write(new Spectre.Console.Rule("[orange3]Records Menu[/]").LeftJustified());
                AnsiConsole.WriteLine();

                var selectedMenuOption = AnsiConsole.Prompt(
                    new SelectionPrompt<RecordsMenuOptions>()
                    .UseConverter(option =>
                    {
                        string name = option.ToString();
                        return name.Replace("_", " ");
                    })
                    .AddChoices(Enum.GetValues<RecordsMenuOptions>())
                );

                Console.Clear();

                switch (selectedMenuOption)
                {
                    case RecordsMenuOptions.Return:
                        exit = true;
                        break;
                    case RecordsMenuOptions.View_Records:
                        CodingController.GetCodingRecords();
                        break;
                    case RecordsMenuOptions.Insert_Record:
                        CodingController.InsertCodingRecord();
                        break;
                    case RecordsMenuOptions.Update_Record:
                        CodingController.UpdateCodingRecord();
                        break;
                    case RecordsMenuOptions.Delete_Record:
                        CodingController.DeleteCodingRecord();
                        break;
                    case RecordsMenuOptions.Coding_Stopwatch:
                        CodingController.CodingStopwatch();
                        break;
                }
            }

        }

        internal static void ReportsMenu()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();

                AnsiConsole.Write(new Spectre.Console.Rule("[orange3]Reports Menu[/]").LeftJustified());
                AnsiConsole.WriteLine();

                var selectedMenuOption = AnsiConsole.Prompt(
                    new SelectionPrompt<ReportsMenuOptions>()
                    .UseConverter(option =>
                    {
                        string name = option.ToString();
                        return name.Replace("_", " ");
                    })
                    .AddChoices(Enum.GetValues<ReportsMenuOptions>())
                );

                Console.Clear();

                switch (selectedMenuOption)
                {
                    case ReportsMenuOptions.Return:
                        exit = true;
                        break;
                    case ReportsMenuOptions.Generate_Report:
                        ReportController.GenerateReport();
                        break;
                }
            }
        }

        internal static void GoalsMenu()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();

                AnsiConsole.Write(new Spectre.Console.Rule("[orange3]Goals Menu[/]").LeftJustified());
                AnsiConsole.WriteLine();

                var selectedMenuOption = AnsiConsole.Prompt(
                    new SelectionPrompt<GoalsMenuOptions>()
                    .UseConverter(option =>
                    {
                        string name = option.ToString();
                        return name.Replace("_", " ");
                    })
                    .AddChoices(Enum.GetValues<GoalsMenuOptions>())
                );
                
                Console.Clear();

                switch (selectedMenuOption)
                {
                    case GoalsMenuOptions.Return:
                        exit = true;
                        break;
                    case GoalsMenuOptions.View_Goals:
                        GoalController.ViewGoals();
                        break;
                    case GoalsMenuOptions.Create_Goal:
                        GoalController.CreateGoal();
                        break;
                    case GoalsMenuOptions.Update_Goal:
                        GoalController.UpdateGoal();
                        break;
                    case GoalsMenuOptions.Delete_Goal:
                        GoalController.DeleteGoal();
                        break;
                }
            }
        }
    }
}
