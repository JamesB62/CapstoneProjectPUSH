namespace CapstoneProjectPUSH.Models
{
    [Table("Workouts")]
    public class Workout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Ignore]
        public int ExerciseCount { get; set; }

        [Ignore]
        public int SetCount { get; set; }

        [Ignore]
        public int TotalMinutes { get; set; }

        [Ignore]
        public bool HasSets => SetCount > 0;

        [Ignore]
        public bool HasMinutes => TotalMinutes > 0;

        public DateTime DateTimeCreated { get; set; } = DateTime.Now;

        public bool IsPrivate { get; set; } = true;

        [Ignore]
        public string VisibilityText => IsPrivate ? "Private" : "Public";

        [Indexed]
        public int UserId { get; set; }

        [Ignore]
        public string UserDisplayName { get; set; } = string.Empty;

        [Indexed]
        public int CategoryId { get; set; }

        [Ignore]
        public string CategoryTitle { get; set; } = string.Empty;
    }
}