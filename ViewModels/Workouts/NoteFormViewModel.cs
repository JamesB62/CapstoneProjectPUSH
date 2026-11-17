namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    [QueryProperty(nameof(WorkoutId), "WorkoutId")]
    [QueryProperty(nameof(NoteId), "NoteId")]
    public partial class NoteFormViewModel : BaseViewModel
    {
        private readonly NoteService _noteService;

        public NoteFormViewModel(NoteService noteService, UserService userService) : base(userService)
        {
            _noteService = noteService;
        }

        private bool _hasLoadedNote;

        [ObservableProperty]
        public partial int WorkoutId { get; set; }

        [ObservableProperty]
        public partial int NoteId { get; set; }

        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Add PUSH Note";

        [ObservableProperty]
        public partial string SaveButtonText { get; set; } = "Save Note";

        [ObservableProperty]
        public partial string NoteTitle { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string NoteContent { get; set; } = string.Empty;

        partial void OnNoteIdChanged(int value)
        {
            if (value > 0)
            {
                PageTitle = "Edit PUSH Note";
                SaveButtonText = "Save Changes";

                LoadCommand.Execute(null);
            }
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            ErrorMessage = string.Empty;

            var user = _userService.CurrentUser;

            if (user is null || user.IsGuest)
            {
                await NavigateAsync("..");
                return;
            }

            // Add mode, or already filled in
            if (NoteId <= 0 || _hasLoadedNote)
            {
                return;
            }

            try
            {
                var note = await _noteService.GetNoteByIdAsync(NoteId, user.Id);

                if (note is null)
                {
                    await Shell.Current.DisplayAlertAsync(
                        "Note Not Found",
                        "This note could not be loaded.",
                        "OK");

                    await NavigateAsync("..");
                    return;
                }

                WorkoutId = note.WorkoutId;
                NoteTitle = note.Title;
                NoteContent = note.Content;

                _hasLoadedNote = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load the note. Please try again.";
            }
        }

        [RelayCommand]
        private async Task SaveNoteAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;

            var user = _userService.CurrentUser;

            if (user is null)
            {
                return;
            }

            string title = (NoteTitle ?? string.Empty).Trim();
            string content = (NoteContent ?? string.Empty).Trim();

            if (title.Length == 0)
            {
                ErrorMessage = "Please give your note a title.";
                return;
            }

            if (title.Length > 100)
            {
                ErrorMessage = "Please keep the title to 100 characters or less.";
                return;
            }

            if (content.Length == 0)
            {
                ErrorMessage = "Please write something in your note.";
                return;
            }

            if (content.Length > 5000)
            {
                ErrorMessage = "Please keep the note to 5000 characters or less.";
                return;
            }

            IsBusy = true;

            try
            {
                bool isEditing = NoteId > 0;

                if (isEditing)
                {
                    var updatedNote = new Note
                    {
                        Id = NoteId,
                        Title = title,
                        Content = content
                    };

                    int rowsUpdated = await _noteService.UpdateNoteAsync(updatedNote, user.Id);

                    if (rowsUpdated == 0)
                    {
                        ErrorMessage = "This note could not be found.";
                        return;
                    }

                    await Shell.Current.DisplayAlertAsync("Note Updated", "Your note has been updated.", "OK");
                }
                else
                {
                    if (WorkoutId <= 0)
                    {
                        ErrorMessage = "This note isn't linked to a workout. Please go back and try again.";
                        return;
                    }

                    var newNote = new Note
                    {
                        Title = title,
                        Content = content,
                        UserId = user.Id,
                        WorkoutId = WorkoutId,
                        DateTimeCreated = DateTime.Now
                    };

                    await _noteService.AddNoteAsync(newNote);

                    await Shell.Current.DisplayAlertAsync("Note Created", "Your note has been created.", "OK");
                }

                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't save the note. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await NavigateAsync("..");
        }
    }
}