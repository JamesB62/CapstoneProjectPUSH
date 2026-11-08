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
    }
}