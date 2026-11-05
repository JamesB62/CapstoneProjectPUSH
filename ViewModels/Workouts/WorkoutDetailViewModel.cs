namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    [QueryProperty(nameof(WorkoutId), "workoutId")]
    public partial class WorkoutsDetailViewModel : BaseViewModel
    {
        private readonly WorkoutService _workoutService;
        private readonly UserService _userService;
        public ObservableCollection<WorkoutExercise> Exercises { get; } = new();

        [ObservableProperty]
        private int workoutId;

        [ObservableProperty]
        private string workoutName = string.Empty;

        [ObservableProperty]
        private string categoryName = string.Empty;

        public WorkoutsDetailViewModel(WorkoutService workoutService, UserService userService)
        {
            _workoutService = workoutService;
            _userService = userService;
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            var userId = _userService.CurrentUser?.Id;
            if(userId is null)
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            var workout = await _workoutService.GetWorkoutByIdAsync(WorkoutId, userId.Value);
            if (workout is null)
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            WorkoutName = workout.Name;
            CategoryName = workout.CategoryName;

            var items = await _workoutService.GetExercisesForWorkoutAsync(WorkoutId);
            Exercises.Clear();
            foreach (var item in items) Exercises.Add(item);
        }

        [RelayCommand]
        private async Task StartWorkoutAsync()
        {
            await Shell.Current.DisplayAlertAsync("Coming soon", "workout tracking isn't built yet.", "OK");
        }

    }
}