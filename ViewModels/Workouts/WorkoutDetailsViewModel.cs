namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    [QueryProperty(nameof(WorkoutId), "WorkoutId")]
    public partial class WorkoutDetailsViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly WorkoutService _workoutService;

        public WorkoutDetailsViewModel(UserService userService, WorkoutService workoutService)
        {
            _userService = userService;
            _workoutService = workoutService;
        }

        public ObservableCollection<WorkoutExercise> Exercises { get; } = new();

        [ObservableProperty]
        public partial int WorkoutId { get; set; }

        [ObservableProperty]
        public partial Workout? Workout { get; set; }

        partial void OnWorkoutIdChanged(int value)
        {
            if (value <= 0)
            {
                return;
            }

            LoadWorkoutCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadWorkoutAsync()
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

                Workout = await _workoutService.GetWorkoutByIdAsync(WorkoutId, user.Id);

                if (Workout is not null)
                {
                    var exercises = await _workoutService.GetExercisesForWorkoutAsync(WorkoutId);

                    List<WorkoutExercise> orderedExercises = exercises
                        .OrderBy(e => e.SortOrder)
                        .ToList();

                    Exercises.Clear();

                    int number = 1;

                    foreach (var exercise in orderedExercises)
                    {
                        exercise.DisplayNumber = number;
                        Exercises.Add(exercise);
                        number++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }

            if (Workout is null && _userService.CurrentUser is not null)
            {
                await Shell.Current.DisplayAlertAsync("Workout not found", "This PUSH workout could not be loaded.", "OK");
                await NavigateAsync("..");
            }
        }

        [RelayCommand]
        private async Task StartWorkoutAsync()
        {
            await Shell.Current.DisplayAlertAsync("Coming soon", "Workout tracking isn't built yet.", "OK");
        }
    }
}