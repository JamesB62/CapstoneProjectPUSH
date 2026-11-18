namespace CapstoneProjectPUSH.Services
{
    public class WorkoutLogService
    {
        private readonly DatabaseService _databaseService;
        private readonly UserService _userService;

        public WorkoutLogService(DatabaseService databaseService, UserService userService)
        {
            _databaseService = databaseService;
            _userService = userService;
        }

        // Get all workout logs for user Id
        public async Task<List<WorkoutLog>> GetWorkoutLogsAsync(int userId)
        {
            if (!_userService.IsCurrentUser(userId))
            {
                return new List<WorkoutLog>();
            }

            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<WorkoutLog>()
                .Where(wl => wl.UserId == userId)
                .OrderByDescending(wl => wl.DateTimeCompleted)
                .ToListAsync();
        }

        // Add workout log
        public async Task AddWorkoutLogAsync(WorkoutLog workoutLog)
        {
            // Guest runs aren't saved
            if (!_userService.IsCurrentMember(workoutLog.UserId))
            {
                return;
            }

            var db = await _databaseService.GetConnectionAsync();

            await db.InsertAsync(workoutLog);
        }
    }
}