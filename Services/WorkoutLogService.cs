namespace CapstoneProjectPUSH.Services
{
    public class WorkoutLogService
    {
        private readonly ApiClient _apiClient;
        private readonly UserService _userService;

        public WorkoutLogService(ApiClient apiClient, UserService userService)
        {
            _apiClient = apiClient;
            _userService = userService;
        }

        // Get all workout logs for user Id
        public async Task<List<WorkoutLog>> GetWorkoutLogsAsync(int userId)
        {
            if (!_userService.IsCurrentUser(userId))
            {
                return new List<WorkoutLog>();
            }

            ApiResult<List<WorkoutLog>> result = await _apiClient.GetAsync<List<WorkoutLog>>("api/workoutlogs/mine");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<WorkoutLog>();
        }

        // Add workout log
        public async Task AddWorkoutLogAsync(WorkoutLog workoutLog)
        {
            // Guest runs aren't saved
            if (!_userService.IsCurrentMember(workoutLog.UserId))
            {
                return;
            }

            // App-side validation
            if (workoutLog.DurationSeconds < 0)
            {
                return;
            }

            WorkoutLogRequest request = new WorkoutLogRequest
            {
                WorkoutId = workoutLog.WorkoutId,
                DurationSeconds = workoutLog.DurationSeconds
            };

            ApiResult<MessageResponse> result = await _apiClient.PostAsync<MessageResponse>("api/workoutlogs", request);

            if (!result.Success)
            {
                System.Diagnostics.Debug.WriteLine("Add workout log failed: " + result.Message);
            }
        }
    }
}