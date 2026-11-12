namespace CapstoneProjectPUSH.ViewModels.Workouts
{
    [QueryProperty(nameof(WorkoutId), "WorkoutId")]
    [QueryProperty(nameof(NoteId), "NoteId")]
    public partial class NoteFormViewModel : BaseViewModel
    {
        private readonly NoteService _noteService;


        public NoteFormViewModel(NoteService noteService, UserService userService) :base(userService)
        {
            _noteService = noteService;
        }

        [ObservableProperty]
        public partial int WorkoutId { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEditMode))]
        [NotifyPropertyChangedFor(nameof(PageTitle))]
        [NotifyPropertyChangedFor(nameof(SaveButtonText))]
        public partial int NoteId { get; set; }

        [ObservableProperty]
        public partial string NoteTitle { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string NoteContent { get; set; } = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        public partial string ErrorMessage { get; set; } = string.Empty;

        public bool IsEditMode
        {
            get
            {
                return NoteId > 0;
            }
        }

        public bool HasError
        {
            get
            {
                return !string.IsNullOrEmpty(ErrorMessage);
            }
        }

        public string PageTitle
        {
            get
            {
                if (IsEditMode)
                {
                    return "Edit Note";
                }
                else
                {
                    return "Add Note";
                }
            }
        }

        public string SaveButtonText
        {
            get
            {
                if (IsEditMode)
                {
                    return "Save Changes";
                }
                else
                {
                    return "Add Note";
                }
            }
        }

        partial void OnNoteIdChanged(int value)
        {
            if (value <= 0)
            {
                return;
            }

            LoadNoteCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadNoteAsync()
        {
            var user = _userService.CurrentUser;

            if (user is null)
            {
                await NavigateAsync("//LandingPage");
                return;
            }

            var note = await _noteService.GetNoteByIdAsync(NoteId, user.Id);

            if (note is null)
            {
                await Shell.Current.DisplayAlertAsync("Note not found", "This note could not be loaded.", "OK");
                await NavigateAsync("..");
                return;
            }

            WorkoutId = note.WorkoutId;
            NoteTitle = note.Title;
            NoteContent = note.Content;
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
                await NavigateAsync("//LandingPage");
                return;
            }

            string title = (NoteTitle ?? string.Empty).Trim();
            string content = (NoteContent ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(title))
            {
                ErrorMessage = "Title is required.";
                return;
            }

            if (title.Length > 100)
            {
                ErrorMessage = "Title must be 100 characters or less.";
                return;
            }

            if (string.IsNullOrEmpty(content))
            {
                ErrorMessage = "Content is required.";
                return;
            }

            if (content.Length > 5000)
            {
                ErrorMessage = "Content must be 5000 characters or less.";
                return;
            }

            IsBusy = true;

            try
            {
                if (IsEditMode)
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
                        ErrorMessage = "This note could not be updated.";
                        return;
                    }
                }
                else
                {
                    if (WorkoutId <= 0)
                    {
                        ErrorMessage = "No workout selected for this note.";
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
                }

                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Something went wrong saving this note.";
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