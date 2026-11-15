namespace CapstoneProjectPUSH.ViewModels.Exercises
{
    [QueryProperty(nameof(ExerciseId), "ExerciseId")]
    public partial class CreateExerciseViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private readonly ExerciseService _exerciseService;

        public CreateExerciseViewModel(CategoryService categoryService, ExerciseService exerciseService, UserService userService) : base(userService)
        {
            _categoryService = categoryService;
            _exerciseService = exerciseService;
        }

        private bool _hasLoadedExercise;

        [ObservableProperty]
        public partial int ExerciseId { get; set; }

        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Add PUSH Exercise";

        [ObservableProperty]
        public partial string SaveButtonText { get; set; } = "Save Exercise";

        // Picker items
        public ObservableCollection<Category> Categories { get; } = new();

        public List<string> DifficultyOptions { get; } = new()
        {
            "Beginner",
            "Easy",
            "Intermediate",
            "Hard",
            "Advanced"
        };

        [ObservableProperty]
        public partial string Title { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Description { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool IsTimed { get; set; }

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        [ObservableProperty]
        public partial int SelectedDifficultyIndex { get; set; } = -1;

        partial void OnExerciseIdChanged(int value)
        {
            if (value > 0)
            {
                PageTitle = "Edit PUSH Exercise";
                SaveButtonText = "Save Changes";
            }
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            ErrorMessage = string.Empty;

            if (!await RequireAdminAsync())
            {
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

                // Add mode, or already filled in
                if (ExerciseId <= 0 || _hasLoadedExercise)
                {
                    return;
                }

                var exercise = await _exerciseService.GetExerciseByIdAsync(ExerciseId);

                if (exercise is null)
                {
                    await Shell.Current.DisplayAlertAsync(
                        "Exercise Not Found",
                        "That exercise no longer exists.",
                        "OK");

                    await NavigateAsync("..");
                    return;
                }

                Title = exercise.Title;
                Description = exercise.Description;
                IsTimed = exercise.IsTimed;
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == exercise.CategoryId);
                SelectedDifficultyIndex = exercise.Difficulty - 1;

                _hasLoadedExercise = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load the form. Please try again.";
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;

            if (SelectedCategory is null)
            {
                ErrorMessage = "Please choose a category for this exercise.";
                return;
            }

            if (SelectedDifficultyIndex < 0)
            {
                ErrorMessage = "Please choose a difficulty for this exercise.";
                return;
            }

            IsBusy = true;

            try
            {
                bool isEditing = ExerciseId > 0;

                var exercise = new Exercise
                {
                    Id = ExerciseId,
                    Title = Title,
                    Description = Description,
                    IsTimed = IsTimed,
                    Difficulty = SelectedDifficultyIndex + 1,
                    CategoryId = SelectedCategory.Id
                };

                (bool Success, string Message) result;

                if (isEditing)
                {
                    result = await _exerciseService.UpdateExerciseAsync(exercise);
                }
                else
                {
                    result = await _exerciseService.AddExerciseAsync(exercise);
                }

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                string exerciseTitle = Title.Trim();

                if (isEditing)
                {
                    await Shell.Current.DisplayAlertAsync("Exercise Updated", $"{exerciseTitle} has been updated.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Exercise Created", $"{exerciseTitle} has been created.", "OK");
                }

                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't save the exercise. Please try again.";
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