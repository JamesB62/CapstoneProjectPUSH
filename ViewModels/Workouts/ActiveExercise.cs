namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class ActiveExercise : ObservableObject
    {
        public ActiveExercise(WorkoutExercise workoutExercise)
        {
            WorkoutExercise = workoutExercise;
        }

        public WorkoutExercise WorkoutExercise { get; }

        [ObservableProperty]
        public partial bool IsCompleted { get; set; }
    }
}