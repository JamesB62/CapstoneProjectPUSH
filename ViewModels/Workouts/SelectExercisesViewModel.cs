namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class SelectExercisesViewModel : BaseViewModel
    {
        private readonly ExerciseService _exerciseService;

        public SelectExercisesViewModel(ExerciseService exerciseService, UserService userService) : base(userService)
        {
            _exerciseService = exerciseService;
        }

        public ObservableCollection<SelectableExercise> Exercises { get; } = new();

        private List<SelectableExercise> _allExercises = new();

        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial SelectableExercise? SelectedExercise { get; set; }

        [ObservableProperty]
        public partial int SelectedCount { get; set; }

        [RelayCommand]
        private async Task LoadExercisesAsync()
        {
            ErrorMessage = string.Empty;

            if (_allExercises.Count > 0)
            {
                return;
            }

            IsBusy = true;

            try
            {
                var exercises = await _exerciseService.GetExercisesAsync();

                foreach (var exercise in exercises)
                {
                    _allExercises.Add(new SelectableExercise(exercise));
                }

                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load exercises. Please go back and try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string term = string.Empty;

            if (SearchText is not null)
            {
                term = SearchText.Trim();
            }

            List<SelectableExercise> filtered;

            if (term.Length == 0)
            {
                filtered = _allExercises;
            }
            else
            {
                filtered = _allExercises
                    .Where(e => e.Exercise.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
                             || e.Exercise.CategoryTitle.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            Exercises.Clear();

            foreach (var exercise in filtered)
            {
                Exercises.Add(exercise);
            }

            if (filtered.Count == 1)
            {
                StatusMessage = "Showing 1 exercise.";
            }
            else
            {
                StatusMessage = $"Showing {filtered.Count} exercises.";
            }
        }

        partial void OnSelectedExerciseChanged(SelectableExercise? value)
        {
            if (value is null)
            {
                return;
            }

            value.IsSelected = !value.IsSelected;
            SelectedCount = _allExercises.Count(e => e.IsSelected);
            SelectedExercise = null;
        }

        [RelayCommand]
        private async Task DoneAsync()
        {
            ErrorMessage = string.Empty;

            List<Exercise> selected = _allExercises
                .Where(e => e.IsSelected)
                .Select(e => e.Exercise)
                .ToList();

            if (selected.Count == 0)
            {
                ErrorMessage = "Please pick at least one exercise.";
                return;
            }

            var parameters = new ShellNavigationQueryParameters
            {
                { "Exercises", selected }
            };

            await NavigateAsync("..", parameters);
        }
    }
}