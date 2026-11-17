namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    [QueryProperty(nameof(WorkoutId), "WorkoutId")]
    [QueryProperty(nameof(PickedExercises), "Exercises")]
    public partial class CreateWorkoutViewModel : BaseViewModel
    {
        private const int MaxSets = 10;
        private const int MaxReps = 100;
        private const int MaxDurationMinutes = 300;

        private readonly CategoryService _categoryService;
        private readonly ExerciseService _exerciseService;
        private readonly WorkoutService _workoutService;

        public CreateWorkoutViewModel(CategoryService categoryService, WorkoutService workoutService, ExerciseService exerciseService, UserService userService) : base(userService)
        {
            _categoryService = categoryService;
            _exerciseService = exerciseService;
            _workoutService = workoutService;
        }

        private bool _hasLoadedWorkout;

        private int _suggestedCategoryId;

        public ObservableCollection<WorkoutExerciseDraft> Items { get; } = new();

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        public partial int WorkoutId { get; set; }

        [ObservableProperty]
        public partial List<Exercise>? PickedExercises { get; set; }

        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Add PUSH Workout";

        [ObservableProperty]
        public partial string SaveButtonText { get; set; } = "Save Workout";

        [ObservableProperty]
        public partial string WorkoutTitle { get; set; } = string.Empty;

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        [ObservableProperty]
        public partial bool IsPublic { get; set; }

        // Switches the page into edit mode
        partial void OnWorkoutIdChanged(int value)
        {
            if (value > 0)
            {
                PageTitle = "Edit PUSH Workout";
                SaveButtonText = "Save Changes";

                LoadFormCommand.Execute(null);
            }
        }

        // Add the exercises picked on the select exercises page
        partial void OnPickedExercisesChanged(List<Exercise>? value)
        {
            if (value is null || value.Count == 0)
            {
                return;
            }

            foreach (var exercise in value)
            {
                Items.Add(new WorkoutExerciseDraft(exercise));
            }

            if (SelectedCategory is null)
            {
                _suggestedCategoryId = value
                    .GroupBy(e => e.CategoryId)
                    .OrderByDescending(g => g.Count())
                    .First()
                    .Key;
            }

            PickedExercises = null;
        }

        [RelayCommand]
        private async Task LoadFormAsync()
        {
            ErrorMessage = string.Empty;

            var user = _userService.CurrentUser;

            // Only registered accounts can create or edit workouts (guests can't save)
            if (user is null || user.IsGuest)
            {
                await NavigateAsync("..");
                return;
            }

            try
            {
                if (Categories.Count == 0)
                {
                    var categories = await _categoryService.GetCategoriesAsync();

                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }
                }

                if (WorkoutId > 0 && !_hasLoadedWorkout)
                {
                    bool loaded = await LoadWorkoutAsync(user.Id);

                    if (!loaded)
                    {
                        await Shell.Current.DisplayAlertAsync(
                            "Workout Not Found",
                            "This PUSH workout could not be loaded.",
                            "OK");

                        await NavigateAsync("..");
                        return;
                    }

                    _hasLoadedWorkout = true;
                }

                if (SelectedCategory is null)
                {
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == _suggestedCategoryId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load the form. Please try again.";
            }
        }

        private async Task<bool> LoadWorkoutAsync(int userId)
        {
            var workout = await _workoutService.GetWorkoutByIdAsync(WorkoutId, userId);

            if (workout is null)
            {
                return false;
            }

            WorkoutTitle = workout.Title;
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == workout.CategoryId);
            IsPublic = !workout.IsPrivate;

            var workoutExercises = await _workoutService.GetExercisesForWorkoutAsync(WorkoutId);
            var exercises = await _exerciseService.GetExercisesAsync();
            var exercisesById = exercises.ToDictionary(e => e.Id);

            Items.Clear();

            foreach (var workoutExercise in workoutExercises)
            {
                if (!exercisesById.TryGetValue(workoutExercise.ExerciseId, out var exercise))
                {
                    continue;
                }

                var draft = new WorkoutExerciseDraft(exercise);

                if (exercise.IsTimed)
                {
                    draft.DurationText = workoutExercise.DurationMinutes.ToString();
                }
                else
                {
                    draft.SetsText = workoutExercise.Sets.ToString();
                    draft.RepsText = workoutExercise.Reps.ToString();
                }

                Items.Add(draft);
            }

            return true;
        }

        [RelayCommand]
        private async Task AddExercisesAsync()
        {
            await NavigateAsync(nameof(SelectExercisesPage));
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
            if (IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;

            var user = _userService.CurrentUser;

            if (user is null)
            {
                return;
            }

            string title = string.Empty;

            if (WorkoutTitle is not null)
            {
                title = WorkoutTitle.Trim();
            }

            if (title.Length == 0)
            {
                ErrorMessage = "Please give your workout a title.";
                return;
            }

            if (SelectedCategory is null)
            {
                ErrorMessage = "Please choose a category for this workout.";
                return;
            }

            if (Items.Count == 0)
            {
                ErrorMessage = "Please add at least one exercise.";
                return;
            }

            List<WorkoutExercise> workoutExercises = new();
            int sortOrder = 1;

            foreach (var item in Items)
            {
                var workoutExercise = new WorkoutExercise
                {
                    ExerciseId = item.Exercise.Id,
                    SortOrder = sortOrder
                };

                if (item.IsTimed)
                {
                    if (!int.TryParse(item.DurationText, out int minutes) || minutes < 1 || minutes > MaxDurationMinutes)
                    {
                        ErrorMessage = $"Please enter between 1 and {MaxDurationMinutes} minutes for {item.Exercise.Title}.";
                        return;
                    }

                    workoutExercise.DurationMinutes = minutes;
                }
                else
                {
                    if (!int.TryParse(item.SetsText, out int sets) || sets < 1 || sets > MaxSets)
                    {
                        ErrorMessage = $"Please enter between 1 and {MaxSets} sets for {item.Exercise.Title}.";
                        return;
                    }

                    if (!int.TryParse(item.RepsText, out int reps) || reps < 1 || reps > MaxReps)
                    {
                        ErrorMessage = $"Please enter between 1 and {MaxReps} reps for {item.Exercise.Title}.";
                        return;
                    }

                    workoutExercise.Sets = sets;
                    workoutExercise.Reps = reps;
                }

                workoutExercises.Add(workoutExercise);
                sortOrder++;
            }

            var workout = new Workout
            {
                Title = title,
                UserId = user.Id,
                CategoryId = SelectedCategory.Id,
                IsPrivate = !IsPublic
            };

            IsBusy = true;

            try
            {
                bool isEditing = WorkoutId > 0;

                if (isEditing)
                {
                    workout.Id = WorkoutId;

                    bool updated = await _workoutService.UpdateWorkoutWithExercisesAsync(workout, user.Id, workoutExercises);

                    if (!updated)
                    {
                        ErrorMessage = "This workout could not be found.";
                        return;
                    }

                    await Shell.Current.DisplayAlertAsync("Workout Updated", $"{title} has been updated.", "OK");
                }
                else
                {
                    await _workoutService.AddWorkoutWithExercisesAsync(workout, workoutExercises);

                    await Shell.Current.DisplayAlertAsync("Workout Created", $"{title} has been created.", "OK");
                }

                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't save the workout. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await NavigateAsync("..");
        }
    }
}