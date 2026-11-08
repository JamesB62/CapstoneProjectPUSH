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

        private static readonly Category AllCategory = new() { Id = 0, Title = "All" };

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        [ObservableProperty]
        public partial Workout? SelectedWorkout { get; set; }

        [RelayCommand]
        private async Task LoadMyWorkoutsAsync()
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

                if (Categories.Count == 0)
                {
                    var categories = await _categoryService.GetCategoriesAsync();

                    var sortedCategories = categories.OrderBy(c => c.Title, StringComparer.OrdinalIgnoreCase).ToList();

                    Categories.Clear();
                    Categories.Add(AllCategory);

                    foreach (var category in sortedCategories)
                    {
                        Categories.Add(category);
                    }

                    SelectedCategory = AllCategory;
                }

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
            if (workout is null)
            {
                return;
            }

            await NavigateAsync($"{nameof(WorkoutDetailsPage)}?WorkoutId={workout.Id}");
        }
    }
}