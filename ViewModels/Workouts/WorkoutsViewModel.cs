namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class WorkoutsViewModel : BaseViewModel
    {
        private readonly WorkoutService _workoutService;

        public WorkoutsViewModel(UserService userService, WorkoutService workoutService) : base (userService)
        {
            _workoutService = workoutService;
        }

        public ObservableCollection<Workout> Workouts { get; } = new();

        private List<Workout> _allWorkouts = new();

        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial Workout? SelectedWorkout { get; set; }

        [RelayCommand]
        private async Task LoadWorkoutsAsync()
        {
            RefreshUserState();

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

                StatusMessage = "Loading workouts...";

                var workouts = await _workoutService.GetWorkoutsForUserAsync(user.Id);

                foreach (var workout in workouts)
                {
                    var exercises = await _workoutService.GetExercisesForWorkoutAsync(workout.Id);

                    workout.ExerciseCount = exercises.Count;
                    workout.SetCount = exercises.Sum(e => e.Sets);
                }

                _allWorkouts = workouts;

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

            List<Workout> filtered;

            if (searchTerm.Length == 0)
            {
                filtered = _allWorkouts;
            }
            else
            {
                filtered = _allWorkouts
                    .Where(w => w.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            Workouts.Clear();

            foreach (var workout in filtered)
            {
                Workouts.Add(workout);
            }

            if (Workouts.Count == 1)
            {
                StatusMessage = "Showing 1 workout.";
            }
            else
            {
                StatusMessage = $"Showing {Workouts.Count} workouts.";
            }
        }

        partial void OnSelectedWorkoutChanged(Workout? value)
        {
            
            if (value is null)
            {
                return;
            }

            ViewWorkoutCommand.Execute(value);
            SelectedWorkout = null;
        }

        [RelayCommand]
        private async Task ViewWorkoutAsync(Workout? workout)
        {
            if (!await RequireLoginAsync())
            return;

            if (workout is null)
            {
                return;
            }

            await NavigateAsync($"{nameof(WorkoutDetailsPage)}?WorkoutId={workout.Id}");
        }

        [RelayCommand]
        private async Task AddWorkoutAsync()
        {
            if (!await RequireLoginAsync())
            return;

            await NavigateAsync(nameof(SelectExercisesPage));
        }

        [RelayCommand]
        private async Task DeleteWorkoutAsync(Workout? workout)
        {
            if (!await RequireLoginAsync())
            return;

            if (workout is null)
            {
                return;
            }

            var user = _userService.CurrentUser;

            if (user is null)
            {
                return;
            }

            bool confirm = await Shell.Current.DisplayAlertAsync("Delete workout", $"Delete \"{workout.Title}\"?", "Delete", "Cancel");

            if (!confirm)
            {
                return;
            }

            try
            {
                await _workoutService.DeleteWorkoutAsync(workout.Id, user.Id);

                _allWorkouts.Remove(workout);
                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Something went wrong. Please try again.";
            }
        }
    }
}