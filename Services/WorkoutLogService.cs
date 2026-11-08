namespace CapstoneProjectPUSH.Services
{
    public class WorkoutLogService
    {
        private readonly DatabaseService _databaseService;

        public WorkoutLogService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Get all workout logs for user Id
        public async Task<List<WorkoutLog>> GetWorkoutLogsAsync(int userId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<WorkoutLog>()
                .Where(wl => wl.UserId == userId)
                .OrderByDescending(wl => wl.DateTimeCompleted)
                .ToListAsync();
        }

        // Get workout log by Id for user Id
        public async Task<WorkoutLog?> GetWorkoutLogByIdAsync(int workoutLogId, int userId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<WorkoutLog>()
                .Where(wl => wl.Id == workoutLogId && wl.UserId == userId)
                .FirstOrDefaultAsync();
        }
    }
}