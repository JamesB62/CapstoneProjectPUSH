namespace CapstoneProjectPUSH.ViewModels.Exercises
{
    public partial class ExercisesViewModel : BaseViewModel
    {
        private readonly ExerciseService _exerciseService;

        public ExercisesViewModel(ExerciseService exerciseService, UserService userService) : base(userService)
        {
            _exerciseService = exerciseService;
        }

        public ObservableCollection<Exercise> Exercises { get; } = new();

        private List<Exercise> _allExercises = new();

        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial Exercise? SelectedExercise { get; set; }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        [RelayCommand]
        private async Task LoadExercisesAsync()
        {
            ErrorMessage = string.Empty;
            RefreshUserState();

            var user = _userService.CurrentUser;

            if (user is null)
            {
                return;
            }

            IsBusy = true;

            try
            {
                _allExercises = await _exerciseService.GetExercisesAsync();

                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load exercises. Pull down to try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ApplyFilter()
        {
            string term = string.Empty;

            if (SearchText is not null)
            {
                term = SearchText.Trim();
            }

            List<Exercise> filtered;

            if (term.Length == 0)
            {
                filtered = _allExercises;
            }
            else
            {
                filtered = _allExercises
                    .Where(e => e.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
                             || e.CategoryTitle.Contains(term, StringComparison.OrdinalIgnoreCase))
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

        partial void OnSelectedExerciseChanged(Exercise? value)
        {
            if (value is null)
            {
                return;
            }

            ViewExerciseCommand.Execute(value);
            SelectedExercise = null;
        }

        [RelayCommand]
        private async Task ViewExerciseAsync(Exercise? exercise)
        {
            if (exercise is null)
            {
                return;
            }

            await NavigateAsync($"{nameof(ExerciseDetailsPage)}?ExerciseId={exercise.Id}");
        }

        [RelayCommand]
        private async Task AddExerciseAsync()
        {
            await NavigateAsync(nameof(CreateExercisePage));
        }

        // Opens the create exercise page in edit mode
        [RelayCommand]
        private async Task EditExerciseAsync(Exercise? exercise)
        {
            if (exercise is null || !IsCurrentUserAdmin)
            {
                return;
            }

            await NavigateAsync($"{nameof(CreateExercisePage)}?ExerciseId={exercise.Id}");
        }

        [RelayCommand]
        private async Task DeleteExerciseAsync(Exercise? exercise)
        {
            if (exercise is null || !IsCurrentUserAdmin)
            {
                return;
            }

            ErrorMessage = string.Empty;

            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete Exercise",
                $"Delete {exercise.Title}? This can't be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            try
            {
                var result = await _exerciseService.DeleteExerciseAsync(exercise.Id);

                if (result.Success)
                {
                    await LoadExercisesCommand.ExecuteAsync(null);
                }
                else
                {
                    ErrorMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't delete the exercise. Please try again.";
            }
        }
    }
}