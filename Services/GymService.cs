namespace CapstoneProjectPUSH.Services
{
    public class GymService
    {
        private readonly DatabaseService _databaseService;

        public GymService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<Gym>> GetGymsAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Gym>()
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        public async Task<Gym?> GetGymByIdAsync(int gymId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Gym>()
                .Where(g => g.Id == gymId)
                .FirstOrDefaultAsync();
        }

        // Add other functions as needed, CRUD?
    }
}