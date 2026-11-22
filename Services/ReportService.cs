namespace CapstoneProjectPUSH.Services
{
    public class ReportService
    {
        private readonly ApiClient _apiClient;
        private readonly UserService _userService;

        public ReportService(ApiClient apiClient, UserService userService)
        {
            _apiClient = apiClient;
            _userService = userService;
        }

        // Get pending report count
        public async Task<int> GetPendingReportCountAsync()
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return 0;
            }

            ApiResult<int> result = await _apiClient.GetAsync<int>("api/reports/pending-count");

            if (result.Success)
            {
                return result.Data;
            }

            return 0;
        }

        // Get all reports
        public async Task<List<Report>> GetReportsAsync()
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return new List<Report>();
            }

            ApiResult<List<Report>> result = await _apiClient.GetAsync<List<Report>>("api/reports");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<Report>();
        }

        // Get report by Id
        public async Task<Report?> GetReportByIdAsync(int reportId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return null;
            }

            ApiResult<Report> result = await _apiClient.GetAsync<Report>($"api/reports/{reportId}");

            if (result.Success)
            {
                return result.Data;
            }

            return null;
        }

        // Add
        public async Task<(bool Success, string Message)> AddReportAsync(int workoutId, string reason)
        {
            User? user = _userService.CurrentUser;

            if (user is null || user.IsGuest)
            {
                return (false, "You need to be logged in to report a workout.");
            }

            if (reason is null)
            {
                reason = string.Empty;
            }

            reason = reason.Trim();

            // App-side validation
            if (reason.Length == 0)
            {
                return (false, "Please enter a reason for the report.");
            }

            if (reason.Length > 500)
            {
                return (false, "Reason must be 500 characters or fewer.");
            }

            ReportRequest request = new ReportRequest
            {
                WorkoutId = workoutId,
                Reason = reason
            };

            ApiResult<MessageResponse> result = await _apiClient.PostAsync<MessageResponse>("api/reports", request);

            return ToTuple(result);
        }

        // Dismiss report
        public async Task<(bool Success, string Message)> DismissReportAsync(int reportId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can manage reports.");
            }

            ApiResult<MessageResponse> result = await _apiClient.PostAsync<MessageResponse>($"api/reports/{reportId}/dismiss", null);

            return ToTuple(result);
        }

        // Remove the reported workout
        public async Task<(bool Success, string Message)> RemoveContentAsync(int reportId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can manage reports.");
            }

            ApiResult<MessageResponse> result = await _apiClient.PostAsync<MessageResponse>($"api/reports/{reportId}/remove-content", null);

            return ToTuple(result);
        }

        // Get the reported workout with its details
        public async Task<Workout?> GetReportedWorkoutAsync(int workoutId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return null;
            }

            ApiResult<Workout> result = await _apiClient.GetAsync<Workout>($"api/reports/workout/{workoutId}");

            if (result.Success)
            {
                return result.Data;
            }

            return null;
        }

        // Helpers
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