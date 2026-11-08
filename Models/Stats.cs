namespace CapstoneProjectPUSH.Models
{
    // Make filename singlular? change to UserStat?
    [Table("Stats")]
    public class Stats
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int TotalCalories { get; set; } = 0;
        // Total duration in minutes
        public int TotalDuration { get; set; } = 0;
        public int TotalWorkouts { get; set; } = 0;
        
        public DateTime DateTimeLastWorkout { get; set; } = DateTime.Now;

        [Indexed]
        public int UserId { get; set; }

        [Ignore]
        public string UserDisplayName { get; set; } = string.Empty;
    }
}