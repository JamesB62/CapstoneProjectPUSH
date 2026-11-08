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

        public DateTime DateTimeCreated { get; set; } = DateTime.Now;

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