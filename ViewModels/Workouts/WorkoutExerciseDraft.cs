namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class WorkoutExerciseDraft : ObservableObject
    {
        public WorkoutExerciseDraft(Exercise exercise)
        {
            Exercise = exercise;
        }

        public Exercise Exercise { get; }

        [ObservableProperty]
        public partial string SetsText { get; set; } = "3";

        [ObservableProperty]
        public partial string RepsText { get; set; } = "10";

        [ObservableProperty]
        public partial string DurationText { get; set; } = "20";

        public bool IsTimed => Exercise.IsTimed;

        public bool IsNotTimed => !Exercise.IsTimed;
    }
}