namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class MyNotesViewModel : BaseViewModel
    {

        private readonly NoteService _noteService;

        public MyNotesViewModel(UserService userService, NoteService noteService) : base(userService)
        {
            _noteService = noteService;
        }

        public ObservableCollection<Note> Notes { get; } = new();

        private List<Note> _allNotes = new();

        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial Note? SelectedNote { get; set; }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        [RelayCommand]
        private async Task LoadMyNotesAsync()
        {
            ErrorMessage = string.Empty;

            var user = _userService.CurrentUser;

            if (user is null)
            {
                return;
            }

            IsBusy = true;

            try
            {
                _allNotes = await _noteService.GetNotesForUserAsync(user.Id);

                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load your notes. Pull down to try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ApplyFilter()
        {
            string term = string.Empty;

            if (SearchText is not null)
            {
                term = SearchText.Trim();
            }

            List<Note> filtered;

            if (term.Length == 0)
            {
                filtered = _allNotes;
            }
            else
            {
                filtered = _allNotes
                    .Where(n => n.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
                             || n.Content.Contains(term, StringComparison.OrdinalIgnoreCase)
                             || n.WorkoutTitle.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            Notes.Clear();

            foreach (var note in filtered)
            {
                Notes.Add(note);
            }

            if (filtered.Count == 1)
            {
                StatusMessage = "Showing 1 note.";
            }
            else
            {
                StatusMessage = $"Showing {filtered.Count} notes.";
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

            if (note.WorkoutId <= 0)
            {
                return;
            }

            await NavigateAsync($"{nameof(WorkoutDetailsPage)}?WorkoutId={note.WorkoutId}");
        }
    }
}