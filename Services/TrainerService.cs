namespace CapstoneProjectPUSH.Services
{
    public class TrainerService
    {
        private readonly DatabaseService _databaseService;

        public TrainerService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<Trainer>> GetTrainersAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            var trainers = await db.Table<Trainer>()
                .OrderBy(t => t.LastName)
                .ThenBy(t => t.FirstName)
                .ToListAsync();

            var gyms = await db.Table<Gym>().ToListAsync();
            var gymNames = gyms.ToDictionary(g => g.Id, g => g.Name);

            foreach (var trainer in trainers)
            {
                trainer.GymName = gymNames.GetValueOrDefault(trainer.GymId, "Unknown Gym");
            }

            return trainers;
        }

        public async Task<Trainer?> GetTrainerByIdAsync(int trainerId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var trainer = await db.Table<Trainer>()
                .Where(t => t.Id == trainerId)
                .FirstOrDefaultAsync();

            if (trainer is null)
            {
                return null;
            }

            var gym = await db.Table<Gym>()
                .Where(g => g.Id == trainer.GymId)
                .FirstOrDefaultAsync();

            if (gym is null)
            {
                trainer.GymName = "Unknown Gym";
            }
            else
            {
                trainer.GymName = gym.Name;
            }

            return trainer;
        }

        // Add other functions as needed, CRUD?
    }
}