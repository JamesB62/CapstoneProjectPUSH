namespace CapstoneProjectPUSH.Services
{
    public class NoteService
    {
        private readonly DatabaseService _databaseService;

        public NoteService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Get all notes for a user
        public async Task<List<Note>> GetNotesForUserAsync(int userId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var notes = await db.Table<Note>()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.DateTimeCreated)
                .ToListAsync();

            var workouts = await db.Table<Workout>()
                .Where(w => w.UserId == userId)
                .ToListAsync();

            var workoutTitle = workouts.ToDictionary(w => w.Id, w => w.Title);

            foreach (var note in notes)
            {
                note.WorkoutTitle = workoutTitle.GetValueOrDefault(note.WorkoutId, "No workout");
            }

            return notes;
        }

        // Get note by Id for a user
        public async Task<Note?> GetNoteByIdAsync(int noteId, int userId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var note = await db.Table<Note>()
                .Where(n => n.Id == noteId && n.UserId == userId)
                .FirstOrDefaultAsync();

            if (note is null)
            {
                return null;
            }

            var workout = await db.Table<Workout>()
                .Where(w => w.Id == note.WorkoutId && w.UserId == userId)
                .FirstOrDefaultAsync();

            if (workout is null)
            {
                note.WorkoutTitle = "No workout";
            }
            else
            {
                note.WorkoutTitle = workout.Title;
            }

            return note;
        }

        // Get note by Workout Id for a user
        public async Task<List<Note>> GetNotesForWorkoutAsync(int workoutId, int userId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Note>()
                .Where(n => n.WorkoutId == workoutId && n.UserId == userId)
                .OrderByDescending(n => n.DateTimeCreated)
                .ToListAsync();
        }
    }
}