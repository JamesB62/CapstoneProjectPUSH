namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    [QueryProperty(nameof(WorkoutId), "WorkoutId")]
    public partial class ActiveWorkoutViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly WorkoutLogService _workoutLogService;
        private readonly WorkoutService _workoutService;

        private readonly System.Diagnostics.Stopwatch _stopwatch = new();
        private IDispatcherTimer? _timer;

        public ActiveWorkoutViewModel(UserService userService, WorkoutLogService workoutLogService, WorkoutService workoutService)
        {
            _userService = userService;
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

                if (Workout is null)
                {
                    Workout = await _workoutService.GetPublicWorkoutByIdAsync(WorkoutId);
                }

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

            if (Workout is null && _userService.CurrentUser is not null)
            {
                await Shell.Current.DisplayAlertAsync("Workout not found", "This PUSH workout could not be loaded.", "OK");
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
            StatusMessage = string.Empty;

            var user = _userService.CurrentUser;

            if (user is null)
            {
                // Comment out to disable redirect
                await NavigateAsync("//LandingPage");
                return;
            }

            if (Workout is null)
            {
                return;
            }

            int completed = Exercises.Count(e => e.IsCompleted);
            string message;

            if (completed == Exercises.Count)
            {
                message = "Save this workout?";
            }
            else
            {
                message = $"You've completed {completed} of {Exercises.Count} exercises. Finish and save anyway?";
            }

            bool confirm = await Shell.Current.DisplayAlertAsync("Finish Workout", message, "Finish", "Keep going");

            if (!confirm)
            {
                return;
            }

            StopTimer();

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
                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Could not save the workout. Please try again.";

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
            bool confirm = await Shell.Current.DisplayAlertAsync("Discard workout?", "This workout won't be saved.", "Discard", "Keep going");

            if (!confirm)
            {
                return;
            }

            StopTimer();
            await NavigateAsync("..");
        }
    }
}