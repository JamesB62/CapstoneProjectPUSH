namespace CapstoneProjectPUSH.Services
{
    public class WeightLogService
    {

        private readonly DatabaseService _databaseService;
        private readonly StatsService _statsService;


        public WeightLogService(DatabaseService databaseService, StatsService statsService)
        {
            _databaseService = databaseService;
            _statsService = statsService;
        }   

        public async Task<List<WeightLog>> WeightLogsAsync(int userId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<WeightLog>()
                .Where(wl => wl.UserId == userId)
                .OrderByDescending(wl => wl.DateTimeLogged)
                .ToListAsync();
        }

        public async Task<WeightLog?> GetWeightLogByIdAsync(int weightLogId, int userId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<WeightLog>()
                .Where(wl => wl.Id == weightLogId && wl.UserId == userId)
                .FirstOrDefaultAsync();
        }




    }
}
