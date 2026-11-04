namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class SelectExercisesViewModel : BaseViewModel
    {
        private readonly ExerciseService _exerciseService;
        private readonly CategoryService _categoryService;
        private readonly UserService _userService;
        private List<SelectableExercise> _all = new();

        public ObservableCollection<SelectableExercise> Exercises { get; } = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private int selectedCount;

        public SelectExercisesViewModel(ExerciseService exerciseService, CategoryService categoryService, UserService userService)
        {
            _exerciseService = exerciseService;
            _categoryService = categoryService;
            _userService = userService;
        }

        partial void OnSearchTextChanged(string value) => ApplyFilter();

        [RelayCommand]
        private async Task LoadExercisesAsync()
        {
            if (_all.Count > 0) return; 
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
            var q = SearchText?.Trim() ?? string.Empty;
            var filtered = string.IsNullOrEmpty(q)
                ? _all
                : _all.Where(x => x.Exercise.Title.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

            Exercises.Clear();
            foreach (var x in filtered) Exercises.Add(x);
        }

        [RelayCommand]
        private async Task DoneAsync()
        {
            var userId = _userService.CurrentUser?.Id;
            if(userId is null)
            {
                await Shell.Current.DisplayAlertAsync("Not signed in", "Please sign in first.", "Ok");
                return;
            }
            
            var selected = _all.Where(x => x.IsSelected).Select(x => x.Exercise).ToList();

            if (selected.Count == 0)
            {
                await Shell.Current.DisplayAlertAsync("Nothing selected", "Pick at least one exercise.", "OK");
                return;
            }

            await Shell.Current.DisplayAlertAsync("Selected", $"{selected.Count} exercise(s) selected.", "OK");
        }
    }
}