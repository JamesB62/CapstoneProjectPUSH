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
        private List<SelectableExercise> _all = new();

        public ObservableCollection<SelectableExercise> Exercises { get; } = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private int selectedCount;

        public ExercisesViewModel(ExerciseService exerciseService, CategoryService categoryService)
        {
            _exerciseService = exerciseService;
            _categoryService = categoryService;
        }

        partial void OnSearchTextChanged(string value) => ApplyFilter();

        [RelayCommand]
        private async Task LoadExerciseAsync()
        {
            if(_all.Count > 0) return;

            var exercises = await _exerciseService.GetExercisesAsync();
            var categoryNames = await _categoryService.GetCategoryNamesAsync();

            _all = exercises.Select(e =>
            {
                var item = new SelectableExercise(e, categoryNames.GetValueOrDefault(e.CategoryId, "Uncategorised"));
                item.PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName == nameof(SelectableExercise.IsSelected))
                        SelectedCount = _all.Count(x => x.IsSelected);
                };
                return item;
            }).ToList();
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
//             var q = SearchText?.Trim() ?? string.Empty;
//             var filtered = string.IsNullOrEmpty(q)
//                 ? _all
//                 : _all.Where(x => x.Exercise.Title.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

//             Exercises.Clear();
//             foreach(var x in filtered) Exercises.Add(x);

//         }

        [RelayCommand]
        private async Task DoneAsync()
        {
            var selected = _all.Where(x => x.IsSelected).Select(x => x.Exercise).ToList();

            if(selected.Count == 0)
            {
                await Shell.Current.DisplayAlertAsync("Nothing selected", "Pick at least one exercise.", "OK");
                return;
            }

            await Shell.Current.DisplayAlertAsync("Selected", $"{selected.Count} exercise(s) selected", "OK");
        }
    }
}