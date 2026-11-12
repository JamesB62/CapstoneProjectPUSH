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

                _allWorkouts = await _workoutService.GetPublicWorkoutsAsync();

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
            if (workout is null)
            {
                return;
            }

            await NavigateAsync($"{nameof(WorkoutDetailsPage)}?WorkoutId={workout.Id}");
        }
    }
}