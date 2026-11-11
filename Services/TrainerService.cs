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

        // Add trainer
        public async Task<(bool Success, string Message)> AddTrainerAsync(Trainer trainer)
        {
            trainer.FirstName = (trainer.FirstName ?? string.Empty).Trim();
            trainer.LastName = (trainer.LastName ?? string.Empty).Trim();
            trainer.Email = (trainer.Email ?? string.Empty).Trim().ToLowerInvariant();
            trainer.PhoneNumber = (trainer.PhoneNumber ?? string.Empty).Trim();
            trainer.Description = (trainer.Description ?? string.Empty).Trim();

            // Validation
            if (trainer.FirstName.Length == 0 || trainer.FirstName.Length > 60)
            {
                return (false, "First name is required (60 characters max).");
            }

            if (trainer.LastName.Length == 0 || trainer.LastName.Length > 60)
            {
                return (false, "Last name is required (60 characters max).");
            }

            // Email is optional, but must be valid when given
            if (trainer.Email.Length > 0 && !IsValidEmail(trainer.Email))
            {
                return (false, "Please enter a valid email address.");
            }

            if (trainer.PhoneNumber.Length > 20)
            {
                return (false, "Phone number must be 20 characters or fewer.");
            }

            if (trainer.Description.Length > 1000)
            {
                return (false, "Description must be 1000 characters or fewer.");
            }

            // Database checks
            var db = await _databaseService.GetConnectionAsync();

            var category = await _categoryService.GetCategoryByIdAsync(trainer.CategoryId);

            if (category is null)
            {
                return (false, "Category could not be found.");
            }

            var gym = await db.Table<Gym>().Where(g => g.Id == trainer.GymId).FirstOrDefaultAsync();

            if (gym is null)
            {
                return (false, "Gym could not be found.");
            }

            if (trainer.Email.Length > 0)
            {
                var emailTaken = await db.Table<Trainer>().Where(t => t.Email == trainer.Email).FirstOrDefaultAsync();

                if (emailTaken is not null)
                {
                    return (false, "A trainer with that email already exists.");
                }
            }

            await db.InsertAsync(trainer);

            return (true, $"{trainer.FullName} was added at {gym.Name} ({gym.City}).");
        }

        // Helpers
        // Validate email format
        private static bool IsValidEmail(string email)
        {
            if (email.Length > 250)
            {
                return false;
            }

            try
            {
                var address = new MailAddress(email);
                return address.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}