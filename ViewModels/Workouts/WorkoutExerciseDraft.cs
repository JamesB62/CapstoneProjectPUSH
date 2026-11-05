namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class WorkoutExerciseDraft : ObservableObject
    {
        public Exercise Exercise { get; }
        public string Title => Exercise.Title;

        [ObservableProperty]
        private int sets = 3;

        [ObservableProperty]
        private int reps = 10;

        public WorkoutExerciseDraft(Exercise exercise) => Exercise = exercise;

        [RelayCommand] private void IncreaseSets() => Sets = Math.Min(Sets + 1, 10);
        [RelayCommand] private void DecreaseSets() => Sets = Math.Max(Sets - 1, 1);
        [RelayCommand] private void IncreaseReps() => Reps = Math.Min(Reps + 1, 100);
        [RelayCommand] private void DecreaseReps() => Reps = Math.Max(Reps - 1, 1);
    }
}