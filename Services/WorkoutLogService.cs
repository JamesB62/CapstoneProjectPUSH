using System;
using System.Collections.Generic;
using System.Text;
using CapstoneProjectPUSH.Models;

namespace CapstoneProjectPUSH.Services
{
    public class WorkoutLogService
    {
        private readonly DatabaseService _databaseService;
        private readonly StatsService _statsService;


        public WorkoutLogService(DatabaseService databaseService, StatsService statsService)
        {
            _databaseService = databaseService;
            _statsService = statsService;
        }

        public async Task<List<WorkoutLog>> GetWorkoutLogsAsync(int userId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<WorkoutLog>()
                .Where(wl => wl.UserId == userId)
                .OrderByDescending(wl => wl.DateTimeCompleted)
                .ToListAsync();
        }

        public async Task<WorkoutLog?> GetWorkoutLogByIdAsync(int workoutLogId, int userId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<WorkoutLog>()
                .Where(wl => wl.Id == workoutLogId && wl.UserId == userId)
                .FirstOrDefaultAsync();
        }
    }
}
