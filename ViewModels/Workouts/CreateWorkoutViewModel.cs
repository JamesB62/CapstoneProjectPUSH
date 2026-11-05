namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class CreateWorkoutViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly WorkoutService _workoutService;
        private readonly CategoryService _categoryService;
        private readonly UserService _userService;
        private int _suggestedCategoryId;

        public ObservableCollection<WorkoutExerciseDraft> Items { get; } = new();
        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        private string workoutName = string.Empty;

        [ObservableProperty]
        private Category? selectedCategory;

        public CreateWorkoutViewModel(WorkoutService workoutService, CategoryService categoryService, UserService userService)
        {
            _workoutService = workoutService;
            _categoryService = categoryService;
            _userService = userService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("exercises", out var value) && value is List<Exercise> exercises && exercises.Count > 0)
            {
                Items.Clear();
                foreach (var e in exercises) Items.Add(new WorkoutExerciseDraft(e));

                _suggestedCategoryId = exercises
                    .GroupBy(e => e.CategoryId)
                    .OrderByDescending(g => g.Count())
                    .First().Key;
            }
        }
        [RelayCommand]
        private async Task LoadAsync()
        {
            if (Categories.Count > 0) return;

            foreach (var c in await _categoryService.GetCategoriesAsync())
            {
                Categories.Add(c);
            }
            
            SelectedCategory ??= Categories.FirstOrDefault(c => c.Id == _suggestedCategoryId);
        }

        [RelayCommand]
        private void MoveUp(WorkoutExerciseDraft item)
        {
            var i = Items.IndexOf(item);
            if (i > 0) Items.Move(i, i - 1);
        }
        [RelayCommand]
        private void MoveDown(WorkoutExerciseDraft item)
        {
            var i = Items.IndexOf(item);
            if (i >= 0 && i < Items.Count - 1) Items.Move(i, i + 1);
        }

        [RelayCommand]
        private void RemoveItem(WorkoutExerciseDraft item) => Items.Remove(item);

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy) return;

            var userId = _userService.CurrentUser?.Id;
            if (userId is null)
            {
                await Shell.Current.DisplayAlertAsync("Not signed in", "Please sign in first", "OK");
                return;
            }

            var name = WorkoutName?.Trim() ?? string.Empty;
            if (name.Length == 0)
            {
                await Shell.Current.DisplayAlertAsync("Name required", "Give your workout a name.", "OK");
                return;
            }

            if (SelectedCategory is null)
            {
                await Shell.Current.DisplayAlertAsync("Category required", "Pick a category.", "OK");
                return;
            }

            if (Items.Count == 0)
            {
                await Shell.Current.DisplayAlertAsync("No exercises", "Add at least one exercise.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var workout = new Workout
                {
                    Name = name,
                    UserId = userId.Value,
                    CategoryId = SelectedCategory.Id
                };
                
                var items = Items.Select((x, i) => new WorkoutExercise
                {
                    ExerciseId = x.Exercise.Id,
                    SortOrder = i + 1,
                    Sets = x.Sets,
                    Reps = x.Reps
                }).ToList();
                await _workoutService.AddWorkoutWithExercisesAsync(workout, items);
                await Shell.Current.GoToAsync("../..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                await Shell.Current.DisplayAlertAsync("Error", "Could not save the workout. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}