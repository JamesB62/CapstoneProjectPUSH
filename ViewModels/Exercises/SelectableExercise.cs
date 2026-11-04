namespace CapstoneProjectPUSH.ViewModels.Exercises
{
    public partial class SelectableExercise : ObservableObject
    {
        public Exercise Exercise { get; }
        public string CategoryName { get; }

        [ObservableProperty]
        private bool isSelected;

        public SelectableExercise(Exercise exercise, string categoryName)
        {
            Exercise = exercise;
            CategoryName = categoryName;
        }

        [RelayCommand]
        private void Toggle() => IsSelected = !IsSelected;

    }

}