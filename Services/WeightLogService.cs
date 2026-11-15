namespace CapstoneProjectPUSH.Services
{
    public class WeightLogService
    {
        private readonly DatabaseService _databaseService;

        public WeightLogService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Get all weight logs for user Id
        public async Task<List<WeightLog>> GetWeightLogsAsync(int userId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<WeightLog>()
                .Where(wl => wl.UserId == userId)
                .OrderByDescending(wl => wl.DateTimeLogged)
                .ToListAsync();
        }

        // Save weight for today for user Id
        public async Task SaveWeightForTodayAsync(int userId, double weightKg)
        {
            var db = await _databaseService.GetConnectionAsync();

            // Used to create a date range for today
            DateTime todayStart = DateTime.Today;
            DateTime tomorrowStart = todayStart.AddDays(1);

            // Check if a weight log already exists for today for user Id
            var existingLog = await db.Table<WeightLog>()
                .Where(wl => wl.UserId == userId && wl.DateTimeLogged >= todayStart && wl.DateTimeLogged < tomorrowStart)
                .FirstOrDefaultAsync();

            // Updates the existing weight log or...
            if (existingLog is not null)
            {
                existingLog.WeightKg = weightKg;
                existingLog.DateTimeLogged = DateTime.Now;
                await db.UpdateAsync(existingLog);
            }
            // Creates a new weight log
            else
            {
                var weightLog = new WeightLog
                {
                    UserId = userId,
                    WeightKg = weightKg,
                    DateTimeLogged = DateTime.Now
                };

                await db.InsertAsync(weightLog);
            }
        }
    }
}