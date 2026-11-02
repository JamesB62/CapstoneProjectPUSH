namespace CapstoneProjectPUSH.ViewModels.Exercises
{
    public partial class ExercisesViewModel : BaseViewModel
    {
        private readonly ExerciseService _exerciseService;
        private readonly CategoryService _categoryService;

        public ExercisesViewModel(ExerciseService exerciseService, CategoryService categoryService)
        {
            _exerciseService = exerciseService;
            _categoryService = categoryService;
        }

        public ObservableCollection<Exercise> Exercises { get; } = new();

        private List<Exercise> AllExercises = new();

        private static readonly Category AllCategories = new() { Id = 0, Name = "All" };

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        private Category? selectedCategory;

        [ObservableProperty]
        private Exercise? selectedExercise;

        [ObservableProperty]
        private string statusMessage = "Loading exercises...";

        private bool isNavigating;

        [RelayCommand]
        private async Task LoadExercisesAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            StatusMessage = "Loading exercises...";

            try
            {
                if (Categories.Count == 0)
                {
                    Categories.Add(AllCategories);

                    foreach (var category in await _categoryService.GetCategoriesAsync())
                    {
                        Categories.Add(category);
                    }

                    SelectedCategory = AllCategories;
                }

                var exercises = await _exerciseService.GetExercisesAsync();
                AllExercises = exercises.ToList();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Could not load exercises, please try again";
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
                filtered = AllExercises;
            }
            else
            {
                filtered = AllExercises.Where(t => t.CategoryId == categoryId).ToList();
            }

            Exercises.Clear();
            foreach (var exercise in filtered)
            {
                Exercises.Add(exercise);
            }

            if (Exercises.Count == 1)
            {
                StatusMessage = "Showing 1 exercise";
            }
            else
            {
                StatusMessage = $"Showing {Exercises.Count} exercises";
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
            if (exercise is null || isNavigating)
            {
                return;
            }

            isNavigating = true;
            try
            {
                await Shell.Current.GoToAsync($"{nameof(ExerciseDetailsPage)}?ExerciseId={exercise.Id}");
            }
            finally
            {
                isNavigating = false;
            }
        }

        // Add more functions when needed
    }
}