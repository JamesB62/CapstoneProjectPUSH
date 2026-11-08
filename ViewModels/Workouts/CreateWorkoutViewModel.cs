namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class CreateWorkoutViewModel : BaseViewModel, IQueryAttributable
    {
        private const int MaxSets = 10;
        private const int MaxReps = 100;

        private readonly CategoryService _categoryService;
        private readonly UserService _userService;
        private readonly WorkoutService _workoutService;

        public CreateWorkoutViewModel(CategoryService categoryService, UserService userService, WorkoutService workoutService)
        {
            _categoryService = categoryService;
            _userService = userService;
            _workoutService = workoutService;
        }

        public ObservableCollection<WorkoutExerciseDraft> Items { get; } = new();

        public ObservableCollection<Category> Categories { get; } = new();

        private int _suggestedCategoryId;

        [ObservableProperty]
        public partial string WorkoutTitle { get; set; } = string.Empty;

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.TryGetValue("Exercises", out var value))
            {
                return;
            }

            if (value is not List<Exercise> exercises || exercises.Count == 0)
            {
                return;
            }

            Items.Clear();

            foreach (var exercise in exercises)
            {
                Items.Add(new WorkoutExerciseDraft(exercise));
            }

            _suggestedCategoryId = exercises
                .GroupBy(e => e.CategoryId)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;
        }

        [RelayCommand]
        private async Task LoadCategoriesAsync()
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

                if (Categories.Count == 0)
                {
                    var categories = await _categoryService.GetCategoriesAsync();

                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }
                }

                if (SelectedCategory is null)
                {
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == _suggestedCategoryId);
                }
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

        [RelayCommand]
        private void MoveUp(WorkoutExerciseDraft? item)
        {
            if (item is null)
            {
                return;
            }

            int index = Items.IndexOf(item);

            if (index > 0)
            {
                Items.Move(index, index - 1);
            }
        }

        [RelayCommand]
        private void MoveDown(WorkoutExerciseDraft? item)
        {
            if (item is null)
            {
                return;
            }

            int index = Items.IndexOf(item);

            if (index >= 0 && index < Items.Count - 1)
            {
                Items.Move(index, index + 1);
            }
        }

        [RelayCommand]
        private void RemoveItem(WorkoutExerciseDraft? item)
        {
            if (item is null)
            {
                return;
            }

            Items.Remove(item);
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            StatusMessage = string.Empty;

            var user = _userService.CurrentUser;

            if (user is null)
            {
                // Comment out to disable redirect
                await NavigateAsync("//LandingPage");
                return;
            }

            string title;

            if (string.IsNullOrWhiteSpace(WorkoutTitle))
            {
                title = string.Empty;
            }
            else
            {
                title = WorkoutTitle.Trim();
            }

            if (title.Length == 0)
            {
                StatusMessage = "Give your workout a title.";
                return;
            }

            if (SelectedCategory is null)
            {
                StatusMessage = "Pick a category.";
                return;
            }

            if (Items.Count == 0)
            {
                StatusMessage = "Add at least one exercise.";
                return;
            }

            List<WorkoutExercise> workoutExercises = new();
            int sortOrder = 1;

            foreach (var item in Items)
            {
                if (!int.TryParse(item.SetsText, out int sets) || sets < 1 || sets > MaxSets)
                {
                    StatusMessage = $"Sets for {item.Exercise.Title} must be between 1 and {MaxSets}.";
                    return;
                }

                if (!int.TryParse(item.RepsText, out int reps) || reps < 1 || reps > MaxReps)
                {
                    StatusMessage = $"Reps for {item.Exercise.Title} must be between 1 and {MaxReps}.";
                    return;
                }

                workoutExercises.Add(new WorkoutExercise
                {
                    ExerciseId = item.Exercise.Id,
                    SortOrder = sortOrder,
                    Sets = sets,
                    Reps = reps
                });

                sortOrder++;
            }

            var workout = new Workout
            {
                Title = title,
                UserId = user.Id,
                CategoryId = SelectedCategory.Id
            };

            IsBusy = true;

            try
            {
                await _workoutService.AddWorkoutWithExercisesAsync(workout, workoutExercises);
                await NavigateAsync("../..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Could not save the workout. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}