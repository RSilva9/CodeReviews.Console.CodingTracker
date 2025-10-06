using Dapper;
using RSilva9.CodingTracker.Models;
using System.Data.SQLite;
using System.Globalization;

namespace RSilva9.CodingTracker
{
    internal class Database
    {
        static string connectionString = System.Configuration.ConfigurationManager.AppSettings.Get("DBConnectionString");

        internal static void CreateTables()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var createRecordsTable = 
                    @"
                    CREATE TABLE IF NOT EXISTS CodingSessions(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT NOT NULL,
                    StartTime TEXT NOT NULL,
                    EndTime TEXT NOT NULL,
                    Duration TEXT NOT NULL
                    )";

                var createGoalsTable =
                    @"
                    CREATE TABLE IF NOT EXISTS Goals(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StartDate TEXT NOT NULL,
                    EndDate TEXT NOT NULL,
                    CurrentHours DOUBLE,
                    GoalHours DOUBLE NOT NULL,
                    Completed BOOL DEFAULT FALSE
                    )";

                connection.Execute(createRecordsTable);
                connection.Execute(createGoalsTable);
                connection.Close();
            }
        }

        internal static CodingSession[] GetCodingRecords()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var getRecords =
                    @"
                    SELECT *
                    FROM CodingSessions
                    ";
                CodingSession[] fetchedRecords = connection.Query<CodingSession>(getRecords).ToArray();

                connection.Close();

                return fetchedRecords;
            }
        }

        internal static CodingSession[] GetCodingRecordsBetweenDates(string date1, string date2)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string parsedDate1 = DateTime.ParseExact(date1, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                string parsedDate2 = DateTime.ParseExact(date2, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                var getRecords =
                    @"
                    SELECT *
                    FROM CodingSessions
                    WHERE date(substr(Date, 7, 4) || '-' || substr(Date, 4, 2) || '-' || substr(Date, 1, 2))
                    BETWEEN date(@Date1) AND date(@Date2)
                    ";

                CodingSession[] fetchedRecords = connection.Query<CodingSession>(getRecords, new { Date1 = parsedDate1, Date2 = parsedDate2 }).ToArray();

                connection.Close();

                return fetchedRecords;
            }
        }

        internal static void InsertCodingRecord(CodingSession newCodingSession)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var insertRecord =
                    @"
                    INSERT INTO CodingSessions (Date, StartTime, EndTime, Duration) 
                    VALUES (@Date, @StartTime, @EndTime, @Duration)";
                connection.Execute(insertRecord, newCodingSession);

                connection.Close();
            }
        }

        internal static void UpdateCodingRecord(int id, string field, string newValue)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var updateRecord =
                    $@"
                    UPDATE CodingSessions SET {field} = @NewValue
                    WHERE Id = @Id
                    ";
                connection.Execute(updateRecord, new { Id = id, NewValue = newValue });

                connection.Close();
            }
        }

        internal static void DeleteCodingRecord(int id)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var deleteRecord =
                    $@"
                    DELETE FROM CodingSessions
                    WHERE Id = @Id
                    ";
                connection.Execute(deleteRecord, new { Id = id });

                connection.Close();
            }
        }

        // GOALS

        internal static Goal[] GetGoals()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var getGoals =
                    @"
                    SELECT *
                    FROM Goals
                    ";

                Goal[] fetchedGoals = connection.Query<Goal>(getGoals).ToArray();

                connection.Close();

                return fetchedGoals;
            }
        }

        internal static Goal[] GetGoalsByDate(string recordDate)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var getGoals =
                @"
                    SELECT *
                    FROM Goals
                    WHERE date(@RecordDate) 
                    BETWEEN date(substr(StartDate, 7, 4) || '-' || substr(StartDate, 4, 2) || '-' || substr(StartDate, 1, 2))
                    AND date(substr(EndDate, 7, 4) || '-' || substr(EndDate, 4, 2) || '-' || substr(EndDate, 1, 2))
                    AND Completed = FALSE
                    ";

                Goal[] fetchedGoals = connection.Query<Goal>(getGoals, new { RecordDate = recordDate}).ToArray();

                connection.Close();

                return fetchedGoals;

            }
        }

        internal static void CreateGoal(Goal newGoal)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var createGoal =
                    @"
                    INSERT INTO Goals (StartDate, EndDate, GoalHours) 
                    VALUES (@StartDate, @EndDate, @GoalHours)";
                connection.Execute(createGoal, newGoal);

                connection.Close();
            }
        }

        internal static void UpdateGoal(int id, string field, object newValue)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var updateGoal =
                   $@"
                    UPDATE Goals SET {field} = @NewValue
                    WHERE Id = @Id
                    ";

                connection.Execute(updateGoal, new { Id = id, NewValue = newValue });

                connection.Close();
            }
        }

        internal static void DeleteGoal(int id)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var deleteGoal =
                    $@"
                    DELETE FROM Goals
                    WHERE Id = @Id
                    ";
                connection.Execute(deleteGoal, new { Id = id });

                connection.Close();
            }
        }
    }
}
