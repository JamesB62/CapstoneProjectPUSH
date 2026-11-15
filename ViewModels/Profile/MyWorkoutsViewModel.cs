namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class MyWorkoutsViewModel : BaseViewModel
    {
        private readonly WorkoutService _workoutService;

        public MyWorkoutsViewModel(UserService userService, WorkoutService workoutService) : base(userService)
        {
            _workoutService = workoutService;
        }

        public ObservableCollection<Workout> Workouts { get; } = new();

        private List<Workout> _allWorkouts = new();

        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial Workout? SelectedWorkout { get; set; }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        [RelayCommand]
        private async Task LoadMyWorkoutsAsync()
        {
            ErrorMessage = string.Empty;

            var user = _userService.CurrentUser;

            if (user is null)
            {
                return;
            }

            IsBusy = true;

            try
            {
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
                ErrorMessage = "Couldn't load your workouts. Pull down to try again.";
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
    }
}