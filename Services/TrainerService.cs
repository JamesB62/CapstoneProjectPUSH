namespace CapstoneProjectPUSH.Services
{
    public class TrainerService
    {
        private readonly ApiClient _apiClient;
        private readonly UserService _userService;

        public TrainerService(ApiClient apiClient, UserService userService)
        {
            _apiClient = apiClient;
            _userService = userService;
        }

        // Get trainer count
        public async Task<int> GetTrainerCountAsync()
        {
            ApiResult<int> result = await _apiClient.GetAsync<int>("api/trainers/count");

            if (result.Success)
            {
                return result.Data;
            }

            return 0;
        }

        // Get available trainer count
        public async Task<int> GetAvailableTrainerCountAsync()
        {
            ApiResult<int> result = await _apiClient.GetAsync<int>("api/trainers/available-count");

            if (result.Success)
            {
                return result.Data;
            }

            return 0;
        }

        // Get all trainers
        public async Task<List<Trainer>> GetTrainersAsync()
        {
            ApiResult<List<Trainer>> result = await _apiClient.GetAsync<List<Trainer>>("api/trainers");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<Trainer>();
        }

        // Get trainer by Id
        public async Task<Trainer?> GetTrainerByIdAsync(int trainerId)
        {
            ApiResult<Trainer> result = await _apiClient.GetAsync<Trainer>($"api/trainers/{trainerId}");

            if (result.Success)
            {
                return result.Data;
            }

            return null;
        }

        // Get all trainers -- at favourite gym first
        public async Task<List<Trainer>> GetTrainersFavouriteGymFirstAsync(int? favouriteGymId)
        {
            ApiResult<List<Trainer>> result = await _apiClient.GetAsync<List<Trainer>>("api/trainers/favourite-first");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<Trainer>();
        }

        // Add trainer
        public async Task<(bool Success, string Message)> AddTrainerAsync(Trainer trainer)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can add trainers.");
            }

            // App-side validation
            string? error = NormaliseAndValidate(trainer);

            if (error is not null)
            {
                return (false, error);
            }

            ApiResult<CreatedResponse> result = await _apiClient.PostAsync<CreatedResponse>("api/trainers", ToRequest(trainer));

            if (!result.Success)
            {
                return (false, result.Message);
            }

            if (result.Data is null)
            {
                return (false, "Something went wrong. Please try again.");
            }

            trainer.Id = result.Data.Id;

            // Upload the photo once the trainer exists
            if (ImageService.IsAppImage(trainer.ProfileImage))
            {
                string? imageError = await ImageUpload.ApplyAsync(_apiClient, $"api/images/trainers/{trainer.Id}", trainer.ProfileImage);

                if (imageError is not null)
                {
                    return (true, result.Data.Message + " The photo couldn't be saved: " + imageError);
                }
            }

            return (true, result.Data.Message);
        }

        // Update trainer details
        public async Task<(bool Success, string Message)> UpdateTrainerAsync(Trainer trainer)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can edit trainers.");
            }

            // App-side validation
            string? error = NormaliseAndValidate(trainer);

            if (error is not null)
            {
                return (false, error);
            }

            ApiResult<MessageResponse> result = await _apiClient.PutAsync<MessageResponse>($"api/trainers/{trainer.Id}", ToRequest(trainer));

            if (!result.Success)
            {
                return (false, result.Message);
            }

            string message = string.Empty;

            if (result.Data is not null)
            {
                message = result.Data.Message;
            }

            // New photo, removed photo, or unchanged
            string? imageError = await ImageUpload.ApplyAsync(_apiClient, $"api/images/trainers/{trainer.Id}", trainer.ProfileImage);

            if (imageError is not null)
            {
                return (true, message + " The photo couldn't be saved: " + imageError);
            }

            return (true, message);
        }

        // Delete trainer
        public async Task<(bool Success, string Message)> DeleteTrainerAsync(int trainerId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can delete trainers.");
            }

            ApiResult<MessageResponse> result = await _apiClient.DeleteAsync<MessageResponse>($"api/trainers/{trainerId}");

            return ToTuple(result);
        }

        // Helpers
        // Trim and validate trainer details
        private static string? NormaliseAndValidate(Trainer trainer)
        {
            trainer.FirstName = Clean(trainer.FirstName);
            trainer.LastName = Clean(trainer.LastName);
            trainer.Email = Clean(trainer.Email).ToLowerInvariant();
            trainer.PhoneNumber = Clean(trainer.PhoneNumber);
            trainer.Description = Clean(trainer.Description);

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

            if (trainer.CategoryId <= 0)
            {
                return "Category could not be found.";
            }

            if (trainer.GymId <= 0)
            {
                return "Gym could not be found.";
            }

            return null;
        }

        // Creates trainer object for requests
        private static TrainerRequest ToRequest(Trainer trainer)
        {
            return new TrainerRequest
            {
                FirstName = trainer.FirstName,
                LastName = trainer.LastName,
                Email = trainer.Email,
                PhoneNumber = trainer.PhoneNumber,
                Description = trainer.Description,
                Available = trainer.Available,
                CategoryId = trainer.CategoryId,
                GymId = trainer.GymId
            };
        }

        // Validate email input
        private static bool IsValidEmail(string email)
        {
            if (email.Length > 250)
            {
                return false;
            }

            try
            {
                MailAddress address = new MailAddress(email);
                return address.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        // Clean input
        private static string Clean(string? value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            return value.Trim();
        }

        // Turns an api reply into the response the view models expect
        private static (bool Success, string Message) ToTuple(ApiResult<MessageResponse> result)
        {
            if (!result.Success)
            {
                return (false, result.Message);
            }

            if (result.Data is null)
            {
                return (true, string.Empty);
            }

            return (true, result.Data.Message);
        }
    }
}