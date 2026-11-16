namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class WorkoutsViewModel : BaseViewModel
    {
        private readonly WorkoutService _workoutService;

        public WorkoutsViewModel(UserService userService, WorkoutService workoutService) : base(userService)
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
            ErrorMessage = string.Empty;
            RefreshUserState();

            var user = _userService.CurrentUser;

            // AppShell sends signed-out users to the landing page
            if (user is null)
            {
                return;
            }

            IsBusy = true;

            try
            {
                _allWorkouts = await _workoutService.GetWorkoutsForUserAsync(user.Id);

                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load your workouts. Pull down to try again.";
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

            List<Workout> filtered;

            if (term.Length == 0)
            {
                filtered = _allWorkouts;
            }
            else
            {
                filtered = _allWorkouts
                    .Where(w => w.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
                             || w.CategoryTitle.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            Workouts.Clear();

            foreach (var workout in filtered)
            {
                Workouts.Add(workout);
            }

            if (filtered.Count == 1)
            {
                StatusMessage = "Showing 1 workout.";
            }
            else
            {
                StatusMessage = $"Showing {filtered.Count} workouts.";
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
            if (workout is null)
            {
                return;
            }

            await NavigateAsync($"{nameof(WorkoutDetailsPage)}?WorkoutId={workout.Id}");
        }

        // Guests get the log in or register prompt
        [RelayCommand]
        private async Task AddWorkoutAsync()
        {
            if (!await RequireLoginAsync())
            {
                return;
            }

            await NavigateAsync(nameof(CreateWorkoutPage));
        }

        // Opens the create workout page in edit mode
        [RelayCommand]
        private async Task EditWorkoutAsync(Workout? workout)
        {
            if (workout is null || !IsLoggedIn)
            {
                return;
            }

            await NavigateAsync($"{nameof(CreateWorkoutPage)}?WorkoutId={workout.Id}");
        }

        [RelayCommand]
        private async Task DeleteWorkoutAsync(Workout? workout)
        {
            if (workout is null || !IsLoggedIn)
            {
                return;
            }

            var user = _userService.CurrentUser;

            if (user is null)
            {
                return;
            }

            ErrorMessage = string.Empty;

            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete Workout",
                $"Delete {workout.Title}? This can't be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            try
            {
                await _workoutService.DeleteWorkoutAsync(workout.Id, user.Id);
                await LoadWorkoutsCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't delete the workout. Please try again.";
            }
        }
    }
}