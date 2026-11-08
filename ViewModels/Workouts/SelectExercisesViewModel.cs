namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class SelectExercisesViewModel : BaseViewModel
    {
        private readonly ExerciseService _exerciseService;
        private readonly UserService _userService;

        public SelectExercisesViewModel(ExerciseService exerciseService, UserService userService)
        {
            _exerciseService = exerciseService;
            _userService = userService;
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
            IsBusy = true;

            try
            {
                var user = _userService.CurrentUser;

                if (user is null)
                {
                    // Comment out to disable redirect
                    await NavigateAsync("//LandingPage");
                    return;
                }

                // Keeps selections when returning from Create Workout
                if (_allExercises.Count > 0)
                {
                    return;
                }

                StatusMessage = "Loading exercises...";

                var exercises = await _exerciseService.GetExercisesAsync();

                _allExercises.Clear();

                foreach (var exercise in exercises)
                {
                    _allExercises.Add(new SelectableExercise(exercise));
                }

                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Something went wrong. Please try again.";
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
            string searchTerm;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                searchTerm = string.Empty;
            }
            else
            {
                searchTerm = SearchText.Trim();
            }

            List<SelectableExercise> filtered;

            if (searchTerm.Length == 0)
            {
                filtered = _allExercises;
            }
            else
            {
                filtered = _allExercises
                    .Where(e => e.Exercise.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            Exercises.Clear();

            foreach (var exercise in filtered)
            {
                Exercises.Add(exercise);
            }

            if (Exercises.Count == 1)
            {
                StatusMessage = "Showing 1 exercise.";
            }
            else
            {
                StatusMessage = $"Showing {Exercises.Count} exercises.";
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
            List<Exercise> selected = _allExercises
                .Where(e => e.IsSelected)
                .Select(e => e.Exercise)
                .ToList();

            if (selected.Count == 0)
            {
                await Shell.Current.DisplayAlertAsync("Nothing selected", "Pick at least one exercise.", "OK");
                return;
            }

            var parameters = new ShellNavigationQueryParameters
            {
                { "Exercises", selected }
            };

            await NavigateAsync(nameof(CreateWorkoutPage), parameters);
        }
    }
}