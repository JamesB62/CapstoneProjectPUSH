namespace CapstoneProjectPUSH.Services
{
    public class TrainerService
    {
        private readonly CategoryService _categoryService;
        private readonly DatabaseService _databaseService;
        private readonly UserService _userService;

        public TrainerService(CategoryService categoryService, DatabaseService databaseService, UserService userService)
        {
            _categoryService = categoryService;
            _databaseService = databaseService;
            _userService = userService;
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

            await FillGymAndCategoryAsync(trainers);

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

            await FillGymAndCategoryAsync(new List<Trainer> { trainer });

            return trainer;
        }

        // Get all trainers, with trainers at the favourite gym first
        public async Task<List<Trainer>> GetTrainersFavouriteGymFirstAsync(int? favouriteGymId)
        {
            var trainers = await GetTrainersAsync();

            foreach (var trainer in trainers)
            {
                if (favouriteGymId is not null && trainer.GymId == favouriteGymId.Value)
                {
                    trainer.IsAtFavouriteGym = true;
                }
                else
                {
                    trainer.IsAtFavouriteGym = false;
                }
            }

            return trainers.OrderByDescending(t => t.IsAtFavouriteGym).ToList();
        }

        // Add trainer
        public async Task<(bool Success, string Message)> AddTrainerAsync(Trainer trainer)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can add trainers.");
            }

            var error = await NormaliseAndValidateAsync(trainer, null);

            if (error is not null)
            {
                return (false, error);
            }

            if (trainer.ProfileImage is null)
            {
                trainer.ProfileImage = string.Empty;
            }

            trainer.ProfileImage = trainer.ProfileImage.Trim();

            if (trainer.ProfileImage.Length > 0 && !ImageService.IsAppImage(trainer.ProfileImage))
            {
                return (false, "That photo couldn't be used. Please choose another.");
            }

            var db = await _databaseService.GetConnectionAsync();

            await db.InsertAsync(trainer);

            return (true, $"{trainer.FullName} was added.");
        }

        // Update trainer details
        public async Task<(bool Success, string Message)> UpdateTrainerAsync(Trainer trainer)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can edit trainers.");
            }

            var db = await _databaseService.GetConnectionAsync();

            var existing = await db.Table<Trainer>().Where(t => t.Id == trainer.Id).FirstOrDefaultAsync();

            if (existing is null)
            {
                return (false, "Trainer could not be found.");
            }

            var error = await NormaliseAndValidateAsync(trainer, trainer.Id);

            if (error is not null)
            {
                return (false, error);
            }

            if (trainer.ProfileImage is null)
            {
                trainer.ProfileImage = string.Empty;
            }

            trainer.ProfileImage = trainer.ProfileImage.Trim();

            if (trainer.ProfileImage.Length > 0 && trainer.ProfileImage != existing.ProfileImage && !ImageService.IsAppImage(trainer.ProfileImage))
            {
                return (false, "That photo couldn't be used. Please choose another.");
            }

            existing.FirstName = trainer.FirstName;
            existing.LastName = trainer.LastName;
            existing.Email = trainer.Email;
            existing.PhoneNumber = trainer.PhoneNumber;
            existing.Description = trainer.Description;
            existing.Available = trainer.Available;
            existing.CategoryId = trainer.CategoryId;
            existing.GymId = trainer.GymId;

            string oldProfileImage = existing.ProfileImage;
            existing.ProfileImage = trainer.ProfileImage;

            await db.UpdateAsync(existing);

            if (oldProfileImage != existing.ProfileImage)
            {
                ImageService.DeleteImage(oldProfileImage);
            }

            return (true, $"{existing.FullName} was updated.");
        }

        // Delete trainer
        public async Task<(bool Success, string Message)> DeleteTrainerAsync(int trainerId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can delete trainers.");
            }

            var db = await _databaseService.GetConnectionAsync();

            var trainer = await db.Table<Trainer>().Where(t => t.Id == trainerId).FirstOrDefaultAsync();

            if (trainer is null)
            {
                return (false, "Trainer could not be found.");
            }

            await db.DeleteAsync(trainer);

            ImageService.DeleteImage(trainer.ProfileImage);

            return (true, $"{trainer.FullName} was deleted.");
        }

        // Helpers
        // Fill in each trainer's gym and category details
        private async Task FillGymAndCategoryAsync(List<Trainer> trainers)
        {
            var db = await _databaseService.GetConnectionAsync();

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
        }

        // Trim and validate trainer details
        private async Task<string?> NormaliseAndValidateAsync(Trainer trainer, int? existingTrainerId)
        {
            trainer.FirstName = (trainer.FirstName ?? string.Empty).Trim();
            trainer.LastName = (trainer.LastName ?? string.Empty).Trim();
            trainer.Email = (trainer.Email ?? string.Empty).Trim().ToLowerInvariant();
            trainer.PhoneNumber = (trainer.PhoneNumber ?? string.Empty).Trim();
            trainer.Description = (trainer.Description ?? string.Empty).Trim();

            // Validation
            if (trainer.FirstName.Length == 0 || trainer.FirstName.Length > 60)
            {
                return "First name is required (60 characters max).";
            }

            if (trainer.LastName.Length == 0 || trainer.LastName.Length > 60)
            {
                return "Last name is required (60 characters max).";
            }

            // Email is optional, but must be valid when given
            if (trainer.Email.Length > 0 && !IsValidEmail(trainer.Email))
            {
                return "Please enter a valid email address.";
            }

            if (trainer.PhoneNumber.Length > 20)
            {
                return "Phone number must be 20 characters or fewer.";
            }

            if (trainer.Email.Length == 0 && trainer.PhoneNumber.Length == 0)
            {
                return "Please enter an email address or phone number (or both).";
            }

            if (trainer.Description.Length > 1000)
            {
                return "Description must be 1000 characters or fewer.";
            }

            // Database checks
            var db = await _databaseService.GetConnectionAsync();

            var category = await _categoryService.GetCategoryByIdAsync(trainer.CategoryId);

            if (category is null)
            {
                return "Category could not be found.";
            }

            var gym = await db.Table<Gym>().Where(g => g.Id == trainer.GymId).FirstOrDefaultAsync();

            if (gym is null)
            {
                return "Gym could not be found.";
            }

            if (trainer.Email.Length > 0)
            {
                // Ids start at 1, so 0 means "no trainer to ignore"
                var ignoreId = existingTrainerId ?? 0;

                var emailTaken = await db.Table<Trainer>()
                    .Where(t => t.Email == trainer.Email && t.Id != ignoreId)
                    .FirstOrDefaultAsync();

                if (emailTaken is not null)
                {
                    return "A trainer with that email already exists.";
                }
            }

            return null;
        }

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