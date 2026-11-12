namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    [QueryProperty(nameof(WorkoutId), "WorkoutId")]
    public partial class WorkoutDetailsViewModel : BaseViewModel
    {
        private readonly NoteService _noteService;
        private readonly UserService _userService;
        private readonly WorkoutService _workoutService;

        public WorkoutDetailsViewModel(NoteService noteService, UserService userService, WorkoutService workoutService)
        {
            _noteService = noteService;
            _userService = userService;
            _workoutService = workoutService;
        }

        public ObservableCollection<WorkoutExercise> Exercises { get; } = new();

        public ObservableCollection<Note> Notes { get; } = new();

        [ObservableProperty]
        public partial int WorkoutId { get; set; }

        [ObservableProperty]
        public partial Workout? Workout { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotOwner))]
        public partial bool IsOwner { get; set; }

        public bool IsNotOwner
        {
            get
            {
                return !IsOwner;
            }
        }

        partial void OnWorkoutIdChanged(int value)
        {
            if (value <= 0)
            {
                return;
            }

            LoadWorkoutCommand.Execute(null);
        }

        public void Reload()
        {
            if (Workout is null)
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
                    IsOwner = true;
                }
                else
                {
                    IsOwner = false;
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
                        Exercises.Add(exercise);
                        number++;
                    }

                    var notes = await _noteService.GetNoteForWorkoutAsync(WorkoutId, user.Id);

                    Notes.Clear();

                    foreach (var note in notes)
                    {
                        Notes.Add(note);
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
        private async Task EditWorkoutAsync()
        {
            if (Workout is null || !IsOwner)
            {
                return;
            }

            var parameters = new ShellNavigationQueryParameters
            {
                { "WorkoutId", Workout.Id }
            };

            await NavigateAsync(nameof(CreateWorkoutPage), parameters);
        }

        [RelayCommand]
        private async Task StartWorkoutAsync()
        {
            if (Workout is null)
            {
                return;
            }

            await NavigateAsync($"{nameof(ActiveWorkoutPage)}?WorkoutId={Workout.Id}");
        }

        // =====================================

        [RelayCommand]
        private async Task AddNoteAsync()
        {
            if (Workout is null)
            {
                return;
            }

            var parameters = new ShellNavigationQueryParameters
            {
                { "WorkoutId", Workout.Id }
            };

            await NavigateAsync(nameof(NoteFormPage), parameters);
        }

        [RelayCommand]
        private async Task EditNoteAsync(Note? note)
        {
            if (note is null)
            {
                return;
            }

            var parameters = new ShellNavigationQueryParameters
            {
                { "NoteId", note.Id }
            };

            await NavigateAsync(nameof(NoteFormPage), parameters);
        }

        [RelayCommand]
        private async Task DeleteNoteAsync(Note? note)
        {
            if (note is null)
            {
                return;
            }

            var user = _userService.CurrentUser;

            if (user is null)
            {
                return;
            }

            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete note",
                $"Delete \"{note.Title}\"? This can't be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            int rowsDeleted = await _noteService.DeleteNoteAsync(note.Id, user.Id);

            if (rowsDeleted > 0)
            {
                Notes.Remove(note);
            }
        }
    }
}