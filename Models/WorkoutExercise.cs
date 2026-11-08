namespace CapstoneProjectPUSH.Models
{
    [Table("WorkoutExercises")]
    public class WorkoutExercise
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int Reps { get; set; }

        public int Sets { get; set; }

        public int SortOrder { get; set; }

        // Add extra variables as needed, eg. Duration, WeightKg

        [Indexed]
        public int WorkoutId { get; set; }

        [Ignore]
        public string WorkoutTitle { get; set; } = string.Empty;

        [Indexed]
        public int ExerciseId { get; set; }

        [Ignore]
        public string ExerciseTitle { get; set; } = string.Empty;
    }
}