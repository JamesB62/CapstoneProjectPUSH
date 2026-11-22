namespace CapstoneProjectPUSH.Services
{
    public class ArticleService
    {
        private readonly ApiClient _apiClient;

        public ArticleService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // Get article count
        public async Task<int> GetArticleCountAsync()
        {
            ApiResult<int> result = await _apiClient.GetAsync<int>("api/articles/count");

            if (result.Success)
            {
                return result.Data;
            }

            return 0;
        }

        // Get all articles
        public async Task<List<Article>> GetArticlesAsync()
        {
            ApiResult<List<Article>> result = await _apiClient.GetAsync<List<Article>>("api/articles");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<Article>();
        }

        // Get article by Id
        public async Task<Article?> GetArticleByIdAsync(int articleId)
        {
            ApiResult<Article> result = await _apiClient.GetAsync<Article>($"api/articles/{articleId}");

            if (result.Success)
            {
                return result.Data;
            }

            return null;
        }
    }
}