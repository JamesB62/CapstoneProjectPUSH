namespace CapstoneProjectPUSH.Services
{
    public class WeightLogService
    {
        private readonly ApiClient _apiClient;
        private readonly UserService _userService;

        public WeightLogService(ApiClient apiClient, UserService userService)
        {
            _apiClient = apiClient;
            _userService = userService;
        }

        // Get all weight logs for user Id
        public async Task<List<WeightLog>> GetWeightLogsAsync(int userId)
        {
            if (!_userService.IsCurrentUser(userId))
            {
                return new List<WeightLog>();
            }

            ApiResult<List<WeightLog>> result = await _apiClient.GetAsync<List<WeightLog>>("api/weightlogs/mine");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<WeightLog>();
        }

        // Save weight for today for user Id
        public async Task SaveWeightForTodayAsync(int userId, double weightKg)
        {
            if (!_userService.IsCurrentMember(userId))
            {
                return;
            }

            // App-side validation
            if (weightKg <= 0 || weightKg > 500)
            {
                return;
            }

            WeightLogRequest request = new WeightLogRequest
            {
                WeightKg = weightKg
            };

            ApiResult<MessageResponse> result = await _apiClient.PostAsync<MessageResponse>("api/weightlogs/today", request);

            if (!result.Success)
            {
                System.Diagnostics.Debug.WriteLine("Save weight failed: " + result.Message);
            }
        }
    }
}