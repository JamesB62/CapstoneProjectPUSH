namespace CapstoneProjectPUSH.ViewModels.Exercises
{
    public partial class ExercisesViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private readonly ExerciseService _exerciseService;
        private readonly UserService _userService;

        public ExercisesViewModel(CategoryService categoryService, ExerciseService exerciseService, UserService userService)
        {
            _categoryService = categoryService;
            _exerciseService = exerciseService;
            _userService = userService;
        }

        public ObservableCollection<Exercise> Exercises { get; } = new();

        private List<Exercise> _allExercises = new();

        private static readonly Category AllCategory = new() { Id = 0, Title = "All" };

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        [ObservableProperty]
        public partial Exercise? SelectedExercise { get; set; }

        [RelayCommand]
        private async Task LoadExercisesAsync()
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

                StatusMessage = "Loading exercises...";

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

                var exercises = await _exerciseService.GetExercisesAsync();
                _allExercises = exercises.ToList();

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

            List<Exercise> filtered;

            if (categoryId == 0)
            {
                filtered = _allExercises;
            }
            else
            {
                filtered = _allExercises.Where(e => e.CategoryId == categoryId).ToList();
            }

            Exercises.Clear();
            foreach (var exercise in filtered)
            {
                Exercises.Add(exercise);
            }

            if (Exercises.Count == 1)
            {
                StatusMessage = "Showing 1 exercise.";
            }
            else
            {
                StatusMessage = $"Showing {Exercises.Count} exercises.";
            }
        }

        partial void OnSelectedExerciseChanged(Exercise? value)
        {
            if (value is null)
            {
                return;
            }

            ViewExerciseCommand.Execute(value);
            SelectedExercise = null;
        }

        [RelayCommand]
        private async Task ViewExerciseAsync(Exercise? exercise)
        {
            if (exercise is null)
            {
                return;
            }

            await NavigateAsync($"{nameof(ExerciseDetailsPage)}?ExerciseId={exercise.Id}");
        }
    }
}