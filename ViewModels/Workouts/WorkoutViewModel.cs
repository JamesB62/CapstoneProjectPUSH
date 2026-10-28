namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class WorkoutViewModel : BaseViewModel
    {
        private readonly WorkoutService _workoutService;
        private List<Workout> _allWorkouts = new();
        public ObservableCollection<Workout> Workouts { get; } = new();

        [ObservableProperty]
        private string searchText = string.Empty;


        public WorkoutViewModel(WorkoutService workoutService)
        {
            _workoutService = workoutService;
        }
        partial void OnSearchTextChanged(string value) => ApplyFilter();

        [RelayCommand]
        private async Task LoadWorkoutsAsync()
        {
            int userId = /* Test user 1 */ 1;
            _allWorkouts = await _workoutService.GetWorkoutsForUserAsync(userId);
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var q = SearchText?.Trim() ?? string.Empty;
            var filtered = string.IsNullOrEmpty(q)
                ? _allWorkouts
                : _allWorkouts.Where(w => w.Name.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

            Workouts.Clear();
            foreach (var w in filtered) Workouts.Add(w);
        }

        // Generates AddWorkoutCommand
        [RelayCommand]
        private async Task AddWorkoutAsync()
        {
            await Shell.Current.DisplayAlertAsync("Coming soon", "Adding workouts isn't built yet.", "OK");
        }
    }
}