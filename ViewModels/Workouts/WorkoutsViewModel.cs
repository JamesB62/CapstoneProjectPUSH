namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    public partial class WorkoutsViewModel : BaseViewModel
    {
        private readonly WorkoutService _workoutService;
        private readonly UserService _userService;
        private List<Workout> _allWorkouts = new();
        public ObservableCollection<Workout> Workouts { get; } = new();

        [ObservableProperty]
        private string searchText = string.Empty;


        public WorkoutsViewModel(WorkoutService workoutService, UserService userService)
        {
            _workoutService = workoutService;
            _userService = userService;
        }

        partial void OnSearchTextChanged(string value) => ApplyFilter();

        [RelayCommand]
        private async Task LoadWorkoutsAsync()
        {
            var userId = _userService.CurrentUser?.Id;
            if(userId is null) return;

            _allWorkouts = await _workoutService.GetWorkoutsForUserAsync(userId.Value);
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
            await Shell.Current.GoToAsync(nameof(SelectExercisesPage));
        }

        [RelayCommand]
        private async Task OpenWorkoutAsync(Workout workout)
        {
            if(workout is null) return;
            await Shell.Current.GoToAsync($"{nameof(WorkoutDetailPage)}?workoutId={workout.Id}");
        }
        
        [RelayCommand]
        private async Task DeleteWorkoutAsync(Workout workout)
        {
            var userId = _userService.CurrentUser?.Id;
            if(userId is null) return;

            if(workout is null) return;

            bool confirm = await Shell.Current.DisplayAlertAsync(
                "Delete workout", $"Delete \"{workout.Name}\"?", "Delete", "Cancel");
            if(!confirm) return;
            
            await _workoutService.DeleteWorkoutAsync(workout.Id, userId.Value);

            _allWorkouts.Remove(workout);
            ApplyFilter();
        }
    }
}