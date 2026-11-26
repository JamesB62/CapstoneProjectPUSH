namespace CapstoneProjectPUSH.Models
{
    public class WorkoutLog
    {
        public int Id { get; set; }

        public int DurationSeconds { get; set; }

        public int DurationMinutes => DurationSeconds / 60;

        public DateTime DateTimeCompleted { get; set; } = DateTime.Now;

        public int WorkoutId { get; set; }

        public string WorkoutTitle { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string UserDisplayName { get; set; } = string.Empty;
    }
}