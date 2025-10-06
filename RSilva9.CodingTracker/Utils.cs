using RSilva9.CodingTracker.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RSilva9.CodingTracker
{
    internal class Utils
    {
        internal static int GetPercentageFromArray(CodingSession[] sessionArray, Func<CodingSession, bool> condition)
        {
            int total = sessionArray.Length;
            int count = 0;
            foreach (CodingSession session in sessionArray) 
            {
                if (condition(session))
                {
                    count++;
                }
            }

            return (int)((double)count / total * 100);
        }

        internal static string ReplaceAllWhiteSpaces(string str)
        {
            return Regex.Replace(str, @"\s+", String.Empty);
        }

        internal static string CalculateDuration(string startTime, string endTime)
        {
            TimeSpan parsedStartTime = TimeSpan.Parse(startTime);
            TimeSpan parsedEndTime = TimeSpan.Parse(endTime);

            if (parsedStartTime < parsedEndTime)
            {
                return (parsedEndTime - parsedStartTime).ToString(@"hh\:mm");
            }
            else
            {
                return ((parsedEndTime + TimeSpan.FromDays(1)) - parsedStartTime).ToString(@"hh\:mm");
            }
        }

        internal static double GetCompletionPercentage(double goal, double completed)
        {
            if (goal == 0) return 0;
            return (completed * 100f) / goal;
        }

        internal static string RenderProgressBar(float goal, double completed, int barSize = 20)
        {
            double percentage = GetCompletionPercentage(goal, completed);
            percentage = Math.Clamp(percentage, 0, 100);
            int filled = (int)(percentage / 100 * barSize);
            string bar = new string('█', filled) + new string('░', barSize - filled);
            return $"{bar} {percentage:0.##}%";
        }

        internal static double CalculateHoursPerDay(string endDate, double goalHours, double currentHours)
        {
            DateTime parsedDate = DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime today = DateTime.Today;
            int remainingDays = (parsedDate - today).Days;
            double remainingHours = goalHours - currentHours;

            if(remainingDays == 0)
            {
                return 0;
            }

            return Math.Round(remainingHours / remainingDays, 2);
        }
    }
}