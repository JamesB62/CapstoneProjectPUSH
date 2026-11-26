namespace CapstoneProjectPUSH.Models
{
    public class WorkoutExercise
    {
        public int Id { get; set; }

        public int Reps { get; set; }

        public int Sets { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsTimed => DurationMinutes > 0;

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

        public int DisplayNumber { get; set; }

        public int WorkoutId { get; set; }

        public string WorkoutTitle { get; set; } = string.Empty;

        public int ExerciseId { get; set; }

        public string ExerciseTitle { get; set; } = string.Empty;
    }
}