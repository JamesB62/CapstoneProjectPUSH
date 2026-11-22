namespace CapstoneProjectPUSH.Services
{
    public class CategoryService
    {
        private readonly ApiClient _apiClient;
        private readonly UserService _userService;

        public CategoryService(ApiClient apiClient, UserService userService)
        {
            _apiClient = apiClient;
            _userService = userService;
        }

        // Get category count
        public async Task<int> GetCategoryCountAsync()
        {
            ApiResult<int> result = await _apiClient.GetAsync<int>("api/categories/count");

            if (result.Success)
            {
                return result.Data;
            }

            return 0;
        }

        // Get all categories
        public async Task<List<Category>> GetCategoriesAsync()
        {
            ApiResult<List<Category>> result = await _apiClient.GetAsync<List<Category>>("api/categories");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<Category>();
        }

        // Get category by Id
        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            ApiResult<Category> result = await _apiClient.GetAsync<Category>($"api/categories/{categoryId}");

            if (result.Success)
            {
                return result.Data;
            }

            return null;
        }

        // Get category titles
        public async Task<Dictionary<int, string>> GetCategoryTitlesAsync()
        {
            List<Category> categories = await GetCategoriesAsync();
            Dictionary<int, string> titles = new Dictionary<int, string>();

            foreach (Category category in categories)
            {
                titles[category.Id] = category.Title;
            }

            return titles;
        }

        // Add category
        public async Task<(bool Success, string Message)> AddCategoryAsync(string title)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can add categories.");
            }

            title = Clean(title);

            // App-side validation
            if (title.Length < 2 || title.Length > 50)
            {
                return (false, "Category title must be between 2 and 50 characters.");
            }

            CategoryRequest request = new CategoryRequest
            {
                Title = title
            };

            ApiResult<MessageResponse> result = await _apiClient.PostAsync<MessageResponse>("api/categories", request);

            return ToTuple(result);
        }

        // Update category
        public async Task<(bool Success, string Message)> UpdateCategoryAsync(int categoryId, string title)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can edit categories.");
            }

            title = Clean(title);

            // App-side validation
            if (title.Length < 2 || title.Length > 50)
            {
                return (false, "Category title must be between 2 and 50 characters.");
            }

            CategoryRequest request = new CategoryRequest
            {
                Title = title
            };

            ApiResult<MessageResponse> result = await _apiClient.PutAsync<MessageResponse>($"api/categories/{categoryId}", request);

            return ToTuple(result);
        }

        // Delete category (the API sets anything using it to "Uncategorised")
        public async Task<(bool Success, string Message)> DeleteCategoryAsync(int categoryId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can delete categories.");
            }

            ApiResult<MessageResponse> result = await _apiClient.DeleteAsync<MessageResponse>($"api/categories/{categoryId}");

            return ToTuple(result);
        }

        // Helpers
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