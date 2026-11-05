namespace CapstoneProjectPUSH.Services
{
    public class NoteService
    {
        private readonly DatabaseService _databaseService;

        public NoteService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<Note>> GetNotesForUserAsync(int userId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var notes = await db.Table<Note>()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.DateTimeAdded)
                .ToListAsync();

            var workouts = await db.Table<Workout>()
                .Where(w => w.UserId == userId)
                .ToListAsync();

            var workoutNames = workouts.ToDictionary(w => w.Id, w => w.Name);

            foreach (var note in notes)
            {
                note.WorkoutName = workoutNames.GetValueOrDefault(note.WorkoutId, "No workout");
            }

            return notes;
        }

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

            return note;
        }

        // Add other functions as needed, CRUD?
    }
}