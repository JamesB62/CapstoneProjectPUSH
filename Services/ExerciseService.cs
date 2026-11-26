namespace CapstoneProjectPUSH.Services
{
    public class ExerciseService
    {
        private readonly ApiClient _apiClient;
        private readonly UserService _userService;

        public ExerciseService(ApiClient apiClient, UserService userService)
        {
            _apiClient = apiClient;
            _userService = userService;
        }

        // Get exercise count
        public async Task<int> GetExerciseCountAsync()
        {
            ApiResult<int> result = await _apiClient.GetAsync<int>("api/exercises/count");

            if (result.Success)
            {
                return result.Data;
            }

            return 0;
        }

        // Get all exercises
        public async Task<List<Exercise>> GetExercisesAsync()
        {
            ApiResult<List<Exercise>> result = await _apiClient.GetAsync<List<Exercise>>("api/exercises");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<Exercise>();
        }

        // Get exercise by Id
        public async Task<Exercise?> GetExerciseByIdAsync(int exerciseId)
        {
            ApiResult<Exercise> result = await _apiClient.GetAsync<Exercise>($"api/exercises/{exerciseId}");

            if (result.Success)
            {
                return result.Data;
            }

            return null;
        }

        // Add an exercise
        public async Task<(bool Success, string Message)> AddExerciseAsync(Exercise exercise)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can add exercises.");
            }

            // App-side validation
            string? error = NormaliseAndValidate(exercise);

            if (error is not null)
            {
                return (false, error);
            }

            ApiResult<CreatedResponse> result = await _apiClient.PostAsync<CreatedResponse>("api/exercises", ToRequest(exercise));

            if (!result.Success)
            {
                return (false, result.Message);
            }

            if (result.Data is null)
            {
                return (false, "Something went wrong. Please try again.");
            }

            exercise.Id = result.Data.Id;

            // Upload the photo once the exercise exists
            if (ImageService.IsAppImage(exercise.Image))
            {
                string? imageError = await ImageUpload.ApplyAsync(_apiClient, $"api/images/exercises/{exercise.Id}", exercise.Image);

                if (imageError is not null)
                {
                    return (true, result.Data.Message + " The photo couldn't be saved: " + imageError);
                }
            }

            return (true, result.Data.Message);
        }

        // Update an exercise
        public async Task<(bool Success, string Message)> UpdateExerciseAsync(Exercise exercise)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can edit exercises.");
            }

            // App-side validation
            string? error = NormaliseAndValidate(exercise);

            if (error is not null)
            {
                return (false, error);
            }

            ApiResult<MessageResponse> result = await _apiClient.PutAsync<MessageResponse>($"api/exercises/{exercise.Id}", ToRequest(exercise));

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
            string? imageError = await ImageUpload.ApplyAsync(_apiClient, $"api/images/exercises/{exercise.Id}", exercise.Image);

            if (imageError is not null)
            {
                return (true, message + " The photo couldn't be saved: " + imageError);
            }

            return (true, message);
        }

        // Delete an exercise
        public async Task<(bool Success, string Message)> DeleteExerciseAsync(int exerciseId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can delete exercises.");
            }

            ApiResult<MessageResponse> result = await _apiClient.DeleteAsync<MessageResponse>($"api/exercises/{exerciseId}");

            return ToTuple(result);
        }

        // Helpers
        // Trim and validate exercise details
        private static string? NormaliseAndValidate(Exercise exercise)
        {
            exercise.Title = Clean(exercise.Title);
            exercise.Description = Clean(exercise.Description);

            if (exercise.Title.Length < 2 || exercise.Title.Length > 100)
            {
                return "Title must be between 2 and 100 characters.";
            }

            if (exercise.Description.Length == 0 || exercise.Description.Length > 1000)
            {
                return "Description is required (1000 characters max).";
            }

            if (exercise.Difficulty < 1 || exercise.Difficulty > 5)
            {
                return "Please choose a difficulty.";
            }

            if (exercise.CategoryId <= 0)
            {
                return "Please choose a category for this exercise.";
            }

            return null;
        }

        // Create exercise object for request
        private static ExerciseRequest ToRequest(Exercise exercise)
        {
            return new ExerciseRequest
            {
                Title = exercise.Title,
                Description = exercise.Description,
                IsTimed = exercise.IsTimed,
                Difficulty = exercise.Difficulty,
                CategoryId = exercise.CategoryId
            };
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