namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class SelectableExercise : ObservableObject
    {
        public SelectableExercise(Exercise exercise)
        {
            Exercise = exercise;
        }

        public Exercise Exercise { get; }

        [ObservableProperty]
        public partial bool IsSelected { get; set; }
    }
}