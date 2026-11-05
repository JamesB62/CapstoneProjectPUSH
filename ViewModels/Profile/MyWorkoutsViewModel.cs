namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class MyWorkoutsViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private readonly UserService _userService;
        private readonly WorkoutService _workoutService;

        public MyWorkoutsViewModel(CategoryService categoryService, UserService userService, WorkoutService workoutService)
        {
            _categoryService = categoryService;
            _userService = userService;
            _workoutService = workoutService;
        }

        public ObservableCollection<Workout> Workouts { get; } = new();

        private List<Workout> _allWorkouts = new();

        private static readonly Category AllCategory = new() { Id = 0, Name = "All" };

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        [ObservableProperty]
        public partial Workout? SelectedWorkout { get; set; }

        [ObservableProperty]
        public partial string StatusMessage { get; set; } = "Loading workouts...";

        private bool _isNavigating;

        [RelayCommand]
        private async Task LoadMyWorkoutsAsync()
        {
            var user = _userService.CurrentUser;

            if (user is null)
            {
                // Comment out to disable redirect
                await Shell.Current.GoToAsync("//LandingPage");
                return;
            }

            IsBusy = true;
            StatusMessage = "Loading workouts...";

            try
            {
                if (Categories.Count == 0)
                {
                    var categories = await _categoryService.GetCategoriesAsync();

                    Categories.Clear();
                    Categories.Add(AllCategory);

                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }

                    SelectedCategory = AllCategory;
                }

                var workouts = await _workoutService.GetWorkoutsForUserAsync(user.Id);

                foreach (var workout in workouts)
                {
                    var exercises = await _workoutService.GetExercisesForWorkoutAsync(workout.Id);
                    int totalSets = exercises.Sum(e => e.Sets);

                    workout.Summary = BuildSummary(exercises.Count, totalSets);
                }

                _allWorkouts.Clear();
                foreach (var workout in workouts)
                {
                    _allWorkouts.Add(workout);
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

        private static string BuildSummary(int exerciseCount, int totalSets)
        {
            if (exerciseCount == 0)
            {
                return "No exercises yet";
            }

            string exercises;
            if (exerciseCount == 1)
            {
                exercises = "1 exercise";
            }
            else
            {
                exercises = $"{exerciseCount} exercises";
            }

            string sets;
            if (totalSets == 1)
            {
                sets = "1 set";
            }
            else
            {
                sets = $"{totalSets} sets";
            }

            return $"{exercises} • {sets}";
        }

        partial void OnSelectedCategoryChanged(Category? value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            int categoryId;

            if (SelectedCategory is null)
            {
                categoryId = 0;
            }
            else
            {
                categoryId = SelectedCategory.Id;
            }

            List<Workout> filtered;

            if (categoryId == 0)
            {
                filtered = _allWorkouts;
            }
            else
            {
                filtered = _allWorkouts.Where(w => w.CategoryId == categoryId).ToList();
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
            if (workout is null || _isNavigating)
            {
                return;
            }

            _isNavigating = true;
            try
            {
                // Link to workout details page when built
                //await Shell.Current.GoToAsync($"{nameof(WorkoutDetailsPage)}?WorkoutId={workout.Id}");
            }
            finally
            {
                _isNavigating = false;
            }
        }
    }
}