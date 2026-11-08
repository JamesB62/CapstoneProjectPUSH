namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class MyNotesViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly NoteService _noteService;
        private readonly WorkoutService _workoutService;

        public MyNotesViewModel(UserService userService, NoteService noteService, WorkoutService workoutService)
        {
            _userService = userService;
            _noteService = noteService;
            _workoutService = workoutService;
        }

        public ObservableCollection<Note> Notes { get; } = new();

        private List<Note> _allNotes = new();

        private static readonly Workout AllWorkout = new() { Id = 0, Title = "All" };

        public ObservableCollection<Workout> Workouts { get; } = new();

        [ObservableProperty]
        public partial Workout? SelectedWorkout { get; set; }

        [ObservableProperty]
        public partial Note? SelectedNote { get; set; }

        [RelayCommand]
        private async Task LoadMyNotesAsync()
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

                StatusMessage = "Loading notes...";

                var notes = await _noteService.GetNotesForUserAsync(user.Id);
                var workouts = await _workoutService.GetWorkoutsForUserAsync(user.Id);

                int selectedId = 0;
                if (SelectedWorkout is not null)
                {
                    selectedId = SelectedWorkout.Id;
                }

                _allNotes = notes;

                Workouts.Clear();
                Workouts.Add(AllWorkout);

                var sortedWorkouts = workouts.OrderBy(w => w.Title, StringComparer.OrdinalIgnoreCase).ToList();

                foreach (var workout in sortedWorkouts)
                {
                    Workouts.Add(workout);
                }

                Workout? match = null;

                foreach (var workout in Workouts)
                {
                    if (workout.Id == selectedId)
                    {
                        match = workout;
                        break;
                    }
                }

                if (match != null)
                {
                    SelectedWorkout = match;
                }
                else
                {
                    SelectedWorkout = AllWorkout;
                }

                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Something went wrong. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnSelectedWorkoutChanged(Workout? value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            int workoutId;

            if (SelectedWorkout is null)
            {
                workoutId = 0;
            }
            else
            {
                workoutId = SelectedWorkout.Id;
            }

            List<Note> filtered;

            if (workoutId == 0)
            {
                filtered = _allNotes;
            }
            else
            {
                filtered = _allNotes.Where(n => n.WorkoutId == workoutId).ToList();
            }

            Notes.Clear();
            foreach (var note in filtered)
            {
                Notes.Add(note);
            }

            if (Notes.Count == 1)
            {
                StatusMessage = "Showing 1 note.";
            }
            else
            {
                StatusMessage = $"Showing {Notes.Count} notes.";
            }
        }

        partial void OnSelectedNoteChanged(Note? value)
        {
            if (value is null)
            {
                return;
            }

            ViewNoteCommand.Execute(value);
            SelectedNote = null;
        }

        [RelayCommand]
        private async Task ViewNoteAsync(Note? note)
        {
            if (note is null)
            {
                return;
            }

            // Link to workout details page when built
            //await NavigateAsync($"{nameof(WorkoutDetailsPage)}?WorkoutId={workout.Id}");
        }
    }
}