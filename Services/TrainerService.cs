namespace CapstoneProjectPUSH.Services
{
    public class TrainerService
    {
        private readonly DatabaseService _databaseService;
        private readonly CategoryService _categoryService;

        public TrainerService(DatabaseService databaseService, CategoryService categoryService)
        {
            _databaseService = databaseService;
            _categoryService = categoryService;
        }

        public async Task<List<Trainer>> GetTrainersAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            var trainers = await db.Table<Trainer>()
                .OrderBy(t => t.LastName)
                .ThenBy(t => t.FirstName)
                .ToListAsync();

            var gyms = await db.Table<Gym>().ToListAsync();
            var gymLookup = gyms.ToDictionary(g => g.Id);

            var categoryNames = await _categoryService.GetCategoryNamesAsync();

            foreach (var trainer in trainers)
            {
                if (gymLookup.TryGetValue(trainer.GymId, out var gym))
                {
                    trainer.GymName = gym.Name;
                    trainer.GymCity = gym.City;
                }
                else
                {
                    trainer.GymName = "Unknown Gym";
                    trainer.GymCity = string.Empty;
                }

                trainer.CategoryName = categoryNames.GetValueOrDefault(trainer.CategoryId, "Uncategorised");
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
                trainer.GymCity = string.Empty;
            }
            else
            {
                trainer.GymName = gym.Name;
                trainer.GymCity = gym.City;
            }

            var categoryNames = await _categoryService.GetCategoryNamesAsync();
            trainer.CategoryName = categoryNames.GetValueOrDefault(trainer.CategoryId, "Uncategorised");

            return trainer;
        }

        // Add other functions as needed, CRUD?
    }
}