namespace CapstoneProjectPUSH.Services
{
    public class NoteService
    {
        private readonly ApiClient _apiClient;
        private readonly UserService _userService;

        public NoteService(ApiClient apiClient, UserService userService)
        {
            _apiClient = apiClient;
            _userService = userService;
        }

        // Get all notes for a user
        public async Task<List<Note>> GetNotesForUserAsync(int userId)
        {
            if (!_userService.IsCurrentUser(userId))
            {
                return new List<Note>();
            }

            ApiResult<List<Note>> result = await _apiClient.GetAsync<List<Note>>("api/notes/mine");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<Note>();
        }

        // Get note by workout id for a user
        public async Task<List<Note>> GetNoteForWorkoutAsync(int workoutId, int userId)
        {
            if (!_userService.IsCurrentUser(userId))
            {
                return new List<Note>();
            }

            ApiResult<List<Note>> result = await _apiClient.GetAsync<List<Note>>($"api/notes/workout/{workoutId}");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<Note>();
        }

        // Get note by Id for a user
        public async Task<Note?> GetNoteByIdAsync(int noteId, int userId)
        {
            if (!_userService.IsCurrentUser(userId))
            {
                return null;
            }

            ApiResult<Note> result = await _apiClient.GetAsync<Note>($"api/notes/{noteId}");

            if (result.Success)
            {
                return result.Data;
            }

            return null;
        }

        // Add
        public async Task<int> AddNoteAsync(Note note)
        {
            // Notes are view-only for guests
            if (!_userService.IsCurrentMember(note.UserId))
            {
                return 0;
            }

            // App-side validation
            if (!IsValidNote(note))
            {
                return 0;
            }

            NoteRequest request = new NoteRequest
            {
                Title = note.Title.Trim(),
                Content = note.Content.Trim(),
                WorkoutId = note.WorkoutId
            };

            ApiResult<NoteCreatedResponse> result = await _apiClient.PostAsync<NoteCreatedResponse>("api/notes", request);

            if (result.Success && result.Data != null)
            {
                note.Id = result.Data.NoteId;
                return result.Data.NoteId;
            }

            System.Diagnostics.Debug.WriteLine("Add note failed: " + result.Message);
            return 0;
        }

        // Delete
        public async Task<int> DeleteNoteAsync(int noteId, int userId)
        {
            if (!_userService.IsCurrentMember(userId))
            {
                return 0;
            }

            ApiResult<MessageResponse> result = await _apiClient.DeleteAsync<MessageResponse>($"api/notes/{noteId}");

            if (result.Success)
            {
                return 1;
            }

            System.Diagnostics.Debug.WriteLine("Delete note failed: " + result.Message);
            return 0;
        }

        // Update
        public async Task<int> UpdateNoteAsync(Note note, int userId)
        {
            if (!_userService.IsCurrentMember(userId))
            {
                return 0;
            }

            // App-side validation
            if (!IsValidNote(note))
            {
                return 0;
            }

            NoteRequest request = new NoteRequest
            {
                Title = note.Title.Trim(),
                Content = note.Content.Trim(),
                WorkoutId = note.WorkoutId
            };

            ApiResult<MessageResponse> result = await _apiClient.PutAsync<MessageResponse>($"api/notes/{note.Id}", request);

            if (result.Success)
            {
                return 1;
            }

            System.Diagnostics.Debug.WriteLine("Update note failed: " + result.Message);
            return 0;
        }

        // Helpers
        // Input validation
        private static bool IsValidNote(Note note)
        {
            string title = string.Empty;
            string content = string.Empty;

            if (note.Title is not null)
            {
                title = note.Title.Trim();
            }

            if (note.Content is not null)
            {
                content = note.Content.Trim();
            }

            if (title.Length == 0 || title.Length > 100)
            {
                return false;
            }

            if (content.Length == 0 || content.Length > 5000)
            {
                return false;
            }

            return true;
        }
    }
}