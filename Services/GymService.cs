namespace CapstoneProjectPUSH.Services
{
    public class GymService
    {
        private readonly ApiClient _apiClient;
        private readonly UserService _userService;

        public GymService(ApiClient apiClient, UserService userService)
        {
            _apiClient = apiClient;
            _userService = userService;
        }

        // Get gym count
        public async Task<int> GetGymCountAsync()
        {
            ApiResult<int> result = await _apiClient.GetAsync<int>("api/gyms/count");

            if (result.Success)
            {
                return result.Data;
            }

            return 0;
        }

        // Get all gyms
        public async Task<List<Gym>> GetGymsAsync()
        {
            ApiResult<List<Gym>> result = await _apiClient.GetAsync<List<Gym>>("api/gyms");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<Gym>();
        }

        // Get gym by Id
        public async Task<Gym?> GetGymByIdAsync(int gymId)
        {
            ApiResult<Gym> result = await _apiClient.GetAsync<Gym>($"api/gyms/{gymId}");

            if (result.Success)
            {
                return result.Data;
            }

            return null;
        }

        // Add gym
        public async Task<(bool Success, string Message)> AddGymAsync(string name, string address, string city)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can add gyms.");
            }

            name = Clean(name);
            address = Clean(address);
            city = Clean(city);

            // App-side validation
            string? error = ValidateGym(name, address, city);

            if (error is not null)
            {
                return (false, error);
            }

            GymRequest request = new GymRequest
            {
                Name = name,
                Address = address,
                City = city
            };

            ApiResult<MessageResponse> result = await _apiClient.PostAsync<MessageResponse>("api/gyms", request);

            return ToTuple(result);
        }

        // Update gym
        public async Task<(bool Success, string Message)> UpdateGymAsync(int gymId, string name, string address, string city)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can edit gyms.");
            }

            name = Clean(name);
            address = Clean(address);
            city = Clean(city);

            // App-side validation
            string? error = ValidateGym(name, address, city);

            if (error is not null)
            {
                return (false, error);
            }

            GymRequest request = new GymRequest
            {
                Name = name,
                Address = address,
                City = city
            };

            ApiResult<MessageResponse> result = await _apiClient.PutAsync<MessageResponse>($"api/gyms/{gymId}", request);

            return ToTuple(result);
        }

        // Delete gym
        public async Task<(bool Success, string Message)> DeleteGymAsync(int gymId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can delete gyms.");
            }

            ApiResult<MessageResponse> result = await _apiClient.DeleteAsync<MessageResponse>($"api/gyms/{gymId}");

            return ToTuple(result);
        }

        // Helpers
        // Validate gym details
        private static string? ValidateGym(string name, string address, string city)
        {
            if (name.Length == 0 || name.Length > 250)
            {
                return "Gym name is required (250 characters max).";
            }

            if (address.Length == 0 || address.Length > 250)
            {
                return "Address is required (250 characters max).";
            }

            if (city.Length == 0 || city.Length > 100)
            {
                return "City is required (100 characters max).";
            }

            return null;
        }

        // Clean inputs
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