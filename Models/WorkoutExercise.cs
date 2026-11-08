namespace CapstoneProjectPUSH.Models
{
    [Table("WorkoutExercises")]
    public class WorkoutExercise
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int Reps { get; set; }

        public int Sets { get; set; }

        public int DurationMinutes { get; set; }

        [Ignore]
        public bool IsTimed => DurationMinutes > 0;

        [Ignore]
        public string TargetText
        {
            get
            {
                if (IsTimed)
                {
                    return $"{DurationMinutes} min";
                }
                else
                {
                    return $"{Sets} sets × {Reps} reps";
                }
            }
        }

        public int SortOrder { get; set; }

        [Ignore]
        public int DisplayNumber { get; set; }

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