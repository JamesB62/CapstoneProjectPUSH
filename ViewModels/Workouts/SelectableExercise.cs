namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class SelectableExercise : ObservableObject
    {
        public Exercise Exercise { get; }
        public string CategoryTitle { get; }

        [ObservableProperty]
        private bool isSelected;

        public SelectableExercise(Exercise exercise, string categoryTitle)
        {
            Exercise = exercise;
            CategoryTitle = categoryTitle;
        }

        [RelayCommand]
        private void Toggle() => IsSelected = !IsSelected;

    }

}