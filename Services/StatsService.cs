namespace CapstoneProjectPUSH.Services
{
    public class StatsService
    {
        private readonly DatabaseService _databaseService;

        public StatsService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Get stats by user Id
        public async Task<Stats?> GetStatsByUserIdAsync(int userId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Stats>()
                .Where(s => s.UserId == userId)
                .FirstOrDefaultAsync();
        }
    }
}