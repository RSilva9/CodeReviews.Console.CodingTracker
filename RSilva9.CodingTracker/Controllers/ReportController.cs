using Spectre.Console;
using RSilva9.CodingTracker.Models;

namespace RSilva9.CodingTracker.Controllers
{
    internal class ReportController
    {
        internal static void GenerateReport()
        {
            bool exit = false;

            while (!exit)
            {
                string startDate = Validations.AskValidatedInput(
                "Date",
                "Please enter the initial date of the report period:",
                Validations.IsDateValid
                );

                string endDate = Validations.AskValidatedInput(
                "Date",
                "Please enter the ending date of the report period:",
                Validations.IsDateValid
                );

                CodingSession[] fetchedSessions = Database.GetCodingRecordsBetweenDates(startDate, endDate);

                int numberOfSessions = fetchedSessions.Length;
                if (numberOfSessions == 0)
                {
                    AnsiConsole.WriteLine($"No recorded sessions in this period ({startDate} - {endDate})");
                    Console.ReadLine();
                    exit = true;
                    break;
                }

                double avg = TimeSpan.FromMinutes(
                    fetchedSessions
                        .Select(s => TimeSpan.Parse(s.Duration).TotalMinutes)
                        .Average()
                ).TotalMinutes;

                double totalMinutes = fetchedSessions.Sum(s => TimeSpan.Parse(s.Duration).TotalMinutes);

                Console.Clear();

                AnsiConsole.Write(new Rule($"[orange3]Coding Report | {startDate} - {endDate}[/]").LeftJustified());
                AnsiConsole.WriteLine();

                AnsiConsole.Write(new BreakdownChart()
                    .Width(60)
                    .AddItem("Less than an hour", Utils.GetPercentageFromArray(fetchedSessions, s => TimeSpan.Parse(s.Duration).TotalHours < 1), Color.Green)
                    .AddItem("1-5 hours", Utils.GetPercentageFromArray(fetchedSessions, s => TimeSpan.Parse(s.Duration).TotalHours >= 1 && TimeSpan.Parse(s.Duration).TotalHours <= 5), Color.Blue)
                    .AddItem("5-10 hours", Utils.GetPercentageFromArray(fetchedSessions, s => TimeSpan.Parse(s.Duration).TotalHours > 5 && TimeSpan.Parse(s.Duration).TotalHours <= 10), Color.Yellow)
                    .AddItem("More than 10 hours", Utils.GetPercentageFromArray(fetchedSessions, s => TimeSpan.Parse(s.Duration).TotalHours > 10), Color.Red)
                );
                AnsiConsole.WriteLine();

                var sessionsTable = new Table();
                sessionsTable.AddColumn(new TableColumn(new Markup("[orange3]Date[/]")));
                sessionsTable.AddColumn(new TableColumn(new Markup("[orange3]Time Period[/]")));
                sessionsTable.AddColumn(new TableColumn(new Markup("[orange3]Total Duration[/]")));
                foreach (var s in fetchedSessions)
                {
                    sessionsTable.AddRow(s.Date, $"{s.StartTime} - {s.EndTime}", s.Duration);
                }

                var statsTable = new Table();
                statsTable.AddColumn(new TableColumn(new Markup("[blue]Number of Sessions[/]")));
                statsTable.AddColumn(new TableColumn(new Markup("[blue]Total Minutes[/]")));
                statsTable.AddColumn(new TableColumn(new Markup("[blue]Average Session Time (minutes)[/]")));
                statsTable.AddRow(numberOfSessions.ToString(), totalMinutes.ToString(), avg.ToString());

                AnsiConsole.Write(sessionsTable);
                AnsiConsole.Write(statsTable);

                Console.ReadLine();

                exit = true;
            }
        }
    }
}
