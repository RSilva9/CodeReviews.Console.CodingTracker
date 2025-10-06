using Spectre.Console;
using System.Globalization;

namespace RSilva9.CodingTracker
{
    internal static class Validations
    {
        internal static string AskValidatedInput(
            string title,
            string prompt,
            Func<string, bool> validation
        )
        {
            AnsiConsole.Write(new Spectre.Console.Rule($"[orange3]{title}[/]").LeftJustified());
            string input = AnsiConsole.Ask<string>(prompt);
            while (!validation(input))
            {
                input = AnsiConsole.Ask<string>($"Invalid input. {prompt}");
            }

            return input;
        }

        internal static bool IsDateValid(string date)
        {
            return DateTime.TryParseExact(date, "dd-MM-yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out _);
        }
        
        internal static bool IsTimeValid(string time)
        {
            return TimeSpan.TryParseExact(time, "hh\\:mm", CultureInfo.InvariantCulture, out _);
        }

        internal static bool IsGoalTimeValid(string time)
        {
            return float.TryParse(time, out _);
        }
    }
}
