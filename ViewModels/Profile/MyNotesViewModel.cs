namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class MyNotesViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly NoteService _noteService;

        public MyNotesViewModel(UserService userService, NoteService noteService)
        {
            _userService = userService;
            _noteService = noteService;
        }

        public ObservableCollection<Note> Notes { get; } = new();

        [ObservableProperty]
        private Note? selectedNote;

        [ObservableProperty]
        private string statusMessage = "Loading notes...";

        private bool isNavigating;

        [RelayCommand]
        private async Task LoadMyNotesAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            StatusMessage = "Loading notes...";

            try
            {
                var user = _userService.CurrentUser;

                if (user is null)
                {
                    return;
                }

                var notes = await _noteService.GetNotesForUserAsync(user.Id);
                Notes.Clear();
                foreach (var note in notes)
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Could not load notes, please try again";
            }
            finally
            {
                IsBusy = false;
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
            if (note is null || isNavigating)
            {
                return;
            }

            isNavigating = true;
            try
            {
                // Link to details page when built
                //await Shell.Current.GoToAsync($"{nameof(WorkoutDetailsPage)}?WorkoutId={workout.Id}");
            }
            finally
            {
                isNavigating = false;
            }
        }

        // Add more functions when needed
    }
}
