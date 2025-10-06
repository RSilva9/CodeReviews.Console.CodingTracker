namespace RSilva9.CodingTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.CreateTables();
            UserInput.MainMenu();
        }
    }
}