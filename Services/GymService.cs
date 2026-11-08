namespace CapstoneProjectPUSH.Services
{
    public class GymService
    {
        private readonly DatabaseService _databaseService;

        public GymService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Get gym count
        public async Task<int> GetGymCountAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Gym>().CountAsync();
        }

        // Get all gyms
        public async Task<List<Gym>> GetGymsAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Gym>()
                .OrderBy(g => g.Name)
                .ThenBy(g => g.City)
                .ToListAsync();
        }

        // Get gym by Id
        public async Task<Gym?> GetGymByIdAsync(int gymId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Gym>()
                .Where(g => g.Id == gymId)
                .FirstOrDefaultAsync();
        }
    }
}