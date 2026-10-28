namespace CapstoneProjectPUSH.Models
{
    [Table("WorkoutLogs")]
    public class WorkoutLog
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int WorkoutId { get; set; }

        [Indexed]
        public int UserId { get; set; }

        public DateTime DateTimeCompleted { get; set; } = DateTime.Now;

        public int DurationSeconds { get; set; }

        public int DurationMinutes { get; set; }

        // Create additional model to log each exercise within a workout?
    }
}