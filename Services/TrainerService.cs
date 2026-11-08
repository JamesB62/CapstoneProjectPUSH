namespace CapstoneProjectPUSH.Services
{
    public class TrainerService
    {
        private readonly CategoryService _categoryService;
        private readonly DatabaseService _databaseService;

        public TrainerService(CategoryService categoryService, DatabaseService databaseService)
        {
            _categoryService = categoryService;
            _databaseService = databaseService;
        }

        // Get trainer count
        public async Task<int> GetTrainerCountAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Trainer>().CountAsync();
        }

        // Get available trainer count
        public async Task<int> GetAvailableTrainerCountAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Trainer>().Where(t => t.Available).CountAsync();
        }

        // Get all trainers
        public async Task<List<Trainer>> GetTrainersAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            var trainers = await db.Table<Trainer>()
                .OrderBy(t => t.LastName)
                .ThenBy(t => t.FirstName)
                .ToListAsync();

            var gyms = await db.Table<Gym>().ToListAsync();
            var gymLookup = gyms.ToDictionary(g => g.Id);

            var categoryTitles = await _categoryService.GetCategoryTitlesAsync();

            foreach (var trainer in trainers)
            {
                if (gymLookup.TryGetValue(trainer.GymId, out var gym))
                {
                    trainer.GymName = gym.Name;
                    trainer.GymAddress = gym.Address;
                    trainer.GymCity = gym.City;
                }
                else
                {
                    trainer.GymName = "Unknown Gym";
                    trainer.GymAddress = string.Empty;
                    trainer.GymCity = string.Empty;
                }

                trainer.CategoryTitle = categoryTitles.GetValueOrDefault(trainer.CategoryId, "Uncategorised");
            }

            return trainers;
        }

        // Get trainer by Id
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
                trainer.GymAddress = string.Empty;
                trainer.GymCity = string.Empty;
            }
            else
            {
                trainer.GymName = gym.Name;
                trainer.GymAddress = gym.Address;
                trainer.GymCity = gym.City;
            }

            var categoryTitle = await _categoryService.GetCategoryTitlesAsync();

            trainer.CategoryTitle = categoryTitle.GetValueOrDefault(trainer.CategoryId, "Uncategorised");

            return trainer;
        }
    }
}