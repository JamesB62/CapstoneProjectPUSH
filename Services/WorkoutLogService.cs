using System;
using System.Collections.Generic;
using System.Text;
using CapstoneProjectPUSH.Models;

namespace CapstoneProjectPUSH.Services
{
    public class WorkoutLogService
    {
        private readonly DatabaseService _databaseService;
        private readonly Stats _statsService;


        public WorkoutLogService(DatabaseService databaseService, Stats statsService)
        {
            _databaseService = databaseService;
            _statsService = statsService;
        }

        public async Task<List<WorkoutLog>> GetWorkoutLogsAsync(int userId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<WorkoutLog>()
                .Where(wl => wl.UserId == userId)
                .OrderByDescending(wl => wl.Date)
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
