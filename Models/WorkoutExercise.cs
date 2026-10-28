namespace CapstoneProjectPUSH.Models
{
    [Table("WorkoutExercises")]
    public class WorkoutExercise
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int WorkoutId { get; set; }

        [Indexed]
        public int ExerciseId { get; set; }

        [Ignore]
        public string ExerciseTitle { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public int Sets { get; set; }

        public int Reps { get; set; }

        // Add extra variables as needed, eg. WeightKg
    }
}