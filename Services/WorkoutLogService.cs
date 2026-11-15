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

        // Add workout log
        public async Task AddWorkoutLogAsync(WorkoutLog workoutLog)
        {
            var db = await _databaseService.GetConnectionAsync();

            await db.InsertAsync(workoutLog);
        }
    }
}