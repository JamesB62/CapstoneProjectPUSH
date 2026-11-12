namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class CreateWorkoutViewModel : BaseViewModel, IQueryAttributable
    {
        private const int MaxSets = 10;
        private const int MaxReps = 100;
        private const int MaxDurationMinutes = 300;

        private readonly CategoryService _categoryService;
        private readonly ExerciseService _exerciseService;
        private readonly WorkoutService _workoutService;

        public CreateWorkoutViewModel(CategoryService categoryService, WorkoutService workoutService,ExerciseService exerciseService,UserService userService) : base(userService)
        {
            _categoryService = categoryService;
            _exerciseService = exerciseService;
            _workoutService = workoutService;
        }

        public ObservableCollection<WorkoutExerciseDraft> Items { get; } = new();

        public ObservableCollection<Category> Categories { get; } = new();

        private int _suggestedCategoryId;

        [ObservableProperty]
        public partial string WorkoutTitle { get; set; } = string.Empty;

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        [ObservableProperty]
        public partial bool IsPublic { get; set; }

        private int _workoutId;

        private bool _hasLoadedWorkout;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PageTitle))]
        [NotifyPropertyChangedFor(nameof(SaveButtonText))]
        public partial bool IsEditMode { get; set; }

        public string PageTitle
        {
            get
            {
                if (IsEditMode)
                {
                    return "Edit PUSH Workout";
                }
                else
                {
                    return "Create PUSH Workout";
                }
            }
        }

        public string SaveButtonText
        {
            get
            {
                if (IsEditMode)
                {
                    return "Save Changes";
                }
                else
                {
                    return "Save Workout";
                }
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("WorkoutId", out var idValue))
            {
                if (idValue is int workoutId && workoutId > 0)
                {
                    _workoutId = workoutId;
                    IsEditMode = true;
                }
            }

            if (query.TryGetValue("Exercises", out var exercisesValue))
            {
                if (exercisesValue is List<Exercise> exercises && exercises.Count > 0)
                {
                    foreach (var exercise in exercises)
                    {
                        Items.Add(new WorkoutExerciseDraft(exercise));
                    }

                    if (SelectedCategory is null)
                    {
                        _suggestedCategoryId = exercises
                            .GroupBy(e => e.CategoryId)
                            .OrderByDescending(g => g.Count())
                            .First()
                            .Key;
                    }
                }
            }
        }

        [RelayCommand]
        private async Task LoadFormAsync()
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

                if (IsEditMode && !_hasLoadedWorkout)
                {
                    bool loaded = await LoadWorkoutAsync(user.Id);

                    if (!loaded)
                    {
                        await Shell.Current.DisplayAlertAsync("Workout not found", "This PUSH workout could not be loaded.", "OK");
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
                StatusMessage = "Something went wrong. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<bool> LoadWorkoutAsync(int userId)
        {
            var workout = await _workoutService.GetWorkoutByIdAsync(_workoutId, userId);

            if (workout is null)
            {
                return false;
            }

            WorkoutTitle = workout.Title;
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == workout.CategoryId);

            if (workout.IsPrivate)
            {
                IsPublic = false;
            }
            else
            {
                IsPublic = true;
            }

            var workoutExercises = await _workoutService.GetExercisesForWorkoutAsync(_workoutId);
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
                var workoutExercise = new WorkoutExercise
                {
                    ExerciseId = item.Exercise.Id,
                    SortOrder = sortOrder
                };

                if (item.IsTimed)
                {
                    if (!int.TryParse(item.DurationText, out int minutes) || minutes < 1 || minutes > MaxDurationMinutes)
                    {
                        StatusMessage = $"Duration for {item.Exercise.Title} must be between 1 and {MaxDurationMinutes} minutes.";
                        return;
                    }

                    workoutExercise.DurationMinutes = minutes;
                }
                else
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
                CategoryId = SelectedCategory.Id
            };

            if (IsPublic)
            {
                workout.IsPrivate = false;
            }
            else
            {
                workout.IsPrivate = true;
            }

            IsBusy = true;

            try
            {
                if (IsEditMode)
                {
                    workout.Id = _workoutId;

                    bool updated = await _workoutService.UpdateWorkoutWithExercisesAsync(workout, user.Id, workoutExercises);

                    if (!updated)
                    {
                        StatusMessage = "This workout could not be found.";
                        return;
                    }

                    await NavigateAsync("..");
                }
                else
                {
                    await _workoutService.AddWorkoutWithExercisesAsync(workout, workoutExercises);
                    await NavigateAsync("..");
                }
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