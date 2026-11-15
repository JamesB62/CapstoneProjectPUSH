namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    [QueryProperty(nameof(WorkoutId), "WorkoutId")]
    public partial class WorkoutDetailsViewModel : BaseViewModel
    {
        private readonly WorkoutService _workoutService;
        private readonly NoteService _noteService;

        public WorkoutDetailsViewModel(NoteService noteService, UserService userService, WorkoutService workoutService) : base(userService)
        {
            _noteService = noteService;
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

        [ObservableProperty]
        public partial bool CanEdit { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNoExercises))]
        public partial bool HasExercises { get; set; }

        public bool HasNoExercises => !HasExercises;

        [ObservableProperty]
        public partial string PrivacyText { get; set; } = string.Empty;

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
            ErrorMessage = string.Empty;
            RefreshUserState();

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
                    CanEdit = IsOwner && IsLoggedIn;

                    if (Workout.IsPrivate)
                    {
                        PrivacyText = "Private";
                    }
                    else
                    {
                        PrivacyText = "Shared";
                    }

                    var exercises = await _workoutService.GetExercisesForWorkoutAsync(WorkoutId);

                    Exercises.Clear();

                    int number = 1;

                    foreach (var exercise in exercises)
                    {
                        exercise.DisplayNumber = number;
                        Exercises.Add(exercise);
                        number++;
                    }

                    HasExercises = Exercises.Count > 0;

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

            if (Workout is null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Workout Not Found",
                    "This PUSH workout could not be loaded.",
                    "OK");

                await NavigateAsync("..");
            }
        }

        [RelayCommand]
        private async Task EditWorkoutAsync()
        {
            if (Workout is null || !CanEdit)
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
            if (Workout is null || !HasExercises)
            {
                return;
            }

            await NavigateAsync($"{nameof(ActiveWorkoutPage)}?WorkoutId={Workout.Id}");
        }

        [RelayCommand]
        private async Task AddNoteAsync()
        {
            if (Workout is null)
            {
                return;
            }

            if (!await RequireLoginAsync())
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
            if (note is null || !IsLoggedIn)
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
            if (note is null || !IsLoggedIn)
            {
                return;
            }

            var user = _userService.CurrentUser;

            if (user is null)
            {
                return;
            }

            ErrorMessage = string.Empty;

            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete Note",
                $"Delete {note.Title}? This can't be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            try
            {
                int rowsDeleted = await _noteService.DeleteNoteAsync(note.Id, user.Id);

                if (rowsDeleted > 0)
                {
                    Notes.Remove(note);
                }
                else
                {
                    ErrorMessage = "That note could not be found.";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't delete the note. Please try again.";
            }
        }
    }
}