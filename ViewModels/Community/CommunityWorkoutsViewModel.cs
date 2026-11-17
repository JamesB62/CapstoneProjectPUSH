namespace CapstoneProjectPUSH.ViewModels.Community
{
    public partial class CommunityWorkoutsViewModel : BaseViewModel
    {
        private readonly WorkoutService _workoutService;

        public CommunityWorkoutsViewModel(UserService userService, WorkoutService workoutService) : base(userService)
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
                _allWorkouts = await _workoutService.GetPublicWorkoutsAsync(user.Id);

                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load community workouts. Pull down to try again.";
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
                             || w.CategoryTitle.Contains(term, StringComparison.OrdinalIgnoreCase)
                             || w.UserDisplayName.Contains(term, StringComparison.OrdinalIgnoreCase))
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
    }
}