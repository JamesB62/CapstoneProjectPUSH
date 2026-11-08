namespace CapstoneProjectPUSH.Models
{
    [Table("WorkoutLogs")]
    public class WorkoutLog
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int DurationSeconds { get; set; }

        [Ignore]
        public int DurationMinutes => DurationSeconds / 60;

        public DateTime DateTimeCompleted { get; set; } = DateTime.Now;

        [Indexed]
        public int WorkoutId { get; set; }

        [Ignore]
        public string WorkoutTitle { get; set; } = string.Empty;

        [Indexed]
        public int UserId { get; set; }

        [Ignore]
        public string UserDisplayName { get; set; } = string.Empty;
    }
}