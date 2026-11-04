namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    [QueryProperty(nameof(WorkoutId),"workoutId")]
    public partial class WorkoutDetailViewModel : BaseViewModel
    {
        private readonly WorkoutService _workoutService;
        public ObservableCollection<WorkoutExercise> Exercises { get; } = new();

        [ObservableProperty]
        private int workoutId;

        [ObservableProperty]
        private string workoutName = string.Empty;

        [ObservableProperty]
        private string categoryName = string.Empty;

        public WorkoutDetailViewModel(WorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            int userId = /* test user */ 1 ;

            var workout = await _workoutService.GetWorkoutByIdAsync(WorkoutId, userId);
            if(workout is null)
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
            await Shell.Current.DisplayAlertAsync("Coming soon", "workout tracking isn't built yet.","OK");
        }
    }
}