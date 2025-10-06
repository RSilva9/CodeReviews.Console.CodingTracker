namespace RSilva9.CodingTracker.Models
{
    internal class Goal
    {
        public int Id { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public float CurrentHours { get; set; }
        public float GoalHours { get; set; }
        public bool Completed { get; set; }
    }
}
