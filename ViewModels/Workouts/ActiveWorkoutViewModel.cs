namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    [QueryProperty(nameof(WorkoutId), "WorkoutId")]
    public partial class ActiveWorkoutViewModel : BaseViewModel
    {
        private readonly WorkoutLogService _workoutLogService;
        private readonly WorkoutService _workoutService;

        private readonly System.Diagnostics.Stopwatch _stopwatch = new();
        private IDispatcherTimer? _timer;

        public ActiveWorkoutViewModel(UserService userService, WorkoutLogService workoutLogService, WorkoutService workoutService) :base(userService)
        {
            _workoutLogService = workoutLogService;
            _workoutService = workoutService;
        }

        public ObservableCollection<ActiveExercise> Exercises { get; } = new();

        [ObservableProperty]
        public partial int WorkoutId { get; set; }

        [ObservableProperty]
        public partial Workout? Workout { get; set; }

        [ObservableProperty]
        public partial ActiveExercise? SelectedExercise { get; set; }

        [ObservableProperty]
        public partial string ElapsedText { get; set; } = "00:00";

        [ObservableProperty]
        public partial string CompletedText { get; set; } = string.Empty;

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
            ErrorMessage = string.Empty;

            var user = _userService.CurrentUser;

            // AppShell sends signed-out users to the landing page
            if (user is null)
            {
                return;
            }

            IsBusy = true;

            try
            {
                Workout = await _workoutService.GetWorkoutByIdAsync(WorkoutId, user.Id);

                if (Workout is null)
                {
                    Workout = await _workoutService.GetPublicWorkoutByIdAsync(WorkoutId);
                }

                if (Workout is not null)
                {
                    var exercises = await _workoutService.GetExercisesForWorkoutAsync(WorkoutId);

                    Exercises.Clear();

                    int number = 1;

                    foreach (var exercise in exercises)
                    {
                        exercise.DisplayNumber = number;
                        Exercises.Add(new ActiveExercise(exercise));
                        number++;
                    }

                    UpdateCompletedText();
                    StartTimer();
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

            if (Workout is null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Workout Not Found",
                    "This PUSH workout could not be loaded.",
                    "OK");

                await NavigateAsync("..");
            }
        }

        // Timer
        private void StartTimer()
        {
            if (_timer is null)
            {
                _timer = Application.Current!.Dispatcher.CreateTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += OnTimerTick;
            }

            _stopwatch.Start();
            _timer.Start();
        }

        private void StopTimer()
        {
            _stopwatch.Stop();

            if (_timer is not null)
            {
                _timer.Stop();
            }
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            TimeSpan elapsed = _stopwatch.Elapsed;

            if (elapsed.TotalHours >= 1)
            {
                ElapsedText = elapsed.ToString(@"h\:mm\:ss");
            }
            else
            {
                ElapsedText = elapsed.ToString(@"mm\:ss");
            }
        }

        // Checklist
        partial void OnSelectedExerciseChanged(ActiveExercise? value)
        {
            if (value is null)
            {
                return;
            }

            value.IsCompleted = !value.IsCompleted;
            UpdateCompletedText();
            SelectedExercise = null;
        }

        private void UpdateCompletedText()
        {
            int completed = Exercises.Count(e => e.IsCompleted);
            CompletedText = $"{completed} of {Exercises.Count} completed";
        }

        // Finish and discard
        [RelayCommand]
        private async Task FinishWorkoutAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;

            var user = _userService.CurrentUser;

            if (user is null || Workout is null)
            {
                return;
            }

            int completed = Exercises.Count(e => e.IsCompleted);
            string message;

            if (completed == Exercises.Count)
            {
                message = "Finish this workout?";
            }
            else
            {
                message = $"You've completed {completed} of {Exercises.Count} exercises. Finish anyway?";
            }

            bool confirmed = await Shell.Current.DisplayAlertAsync("Finish Workout", message, "Finish", "Keep Going");

            if (!confirmed)
            {
                return;
            }

            StopTimer();

            // Guest runs aren't saved
            if (user.IsGuest)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Workout Finished",
                    $"Nice work! Guest workouts aren't saved. Log in or register to track your progress.",
                    "OK");

                await NavigateAsync("..");
                return;
            }

            var workoutLog = new WorkoutLog
            {
                WorkoutId = Workout.Id,
                UserId = user.Id,
                DurationSeconds = (int)_stopwatch.Elapsed.TotalSeconds,
                DateTimeCompleted = DateTime.Now
            };

            IsBusy = true;

            try
            {
                await _workoutLogService.AddWorkoutLogAsync(workoutLog);

                await Shell.Current.DisplayAlertAsync(
                    "Workout Saved",
                    $"{Workout.Title} has been saved to your stats.",
                    "OK");

                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't save the workout. Please try again.";

                // Resume timing so the session isn't lost
                StartTimer();
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DiscardWorkoutAsync()
        {
            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Discard Workout",
                "This workout won't be saved.",
                "Discard",
                "Keep Going");

            if (!confirmed)
            {
                return;
            }

            StopTimer();
            await NavigateAsync("..");
        }
    }
}