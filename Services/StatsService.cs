namespace CapstoneProjectPUSH.Services
{
    public class StatsService
    {
        private readonly DatabaseService _databaseService;

        public StatsService(DatabaseService databaseService, CategoryService categoryService)
        {
            _databaseService = databaseService;
        }

        public async Task<Stats?> GetStatsByUserIdAsync(int userId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Stats>()
                .Where(s => s.UserId == userId)
                .FirstOrDefaultAsync();
        }







    }
}
