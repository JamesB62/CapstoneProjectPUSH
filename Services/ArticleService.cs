namespace CapstoneProjectPUSH.Services
{
    public class ArticleService
    {
        private readonly ApiClient _apiClient;
        private readonly UserService _userService;

        public ArticleService(ApiClient apiClient, UserService userService)
        {
            _apiClient = apiClient;
            _userService = userService;
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

        // Add an article
        public async Task<(bool Success, string Message)> AddArticleAsync(Article article)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can add articles.");
            }

            // App-side validation
            string? error = NormaliseAndValidate(article);

            if (error is not null)
            {
                return (false, error);
            }

            ApiResult<CreatedResponse> result = await _apiClient.PostAsync<CreatedResponse>("api/articles", ToRequest(article));

            if (!result.Success)
            {
                return (false, result.Message);
            }

            if (result.Data is null)
            {
                return (false, "Something went wrong. Please try again.");
            }

            article.Id = result.Data.Id;

            // Upload the photo once the article exists
            if (ImageService.IsAppImage(article.Image))
            {
                string? imageError = await ImageUpload.ApplyAsync(_apiClient, "api/images/articles/" + article.Id, article.Image);

                if (imageError is not null)
                {
                    return (true, result.Data.Message + " The photo couldn't be saved: " + imageError);
                }
            }

            return (true, result.Data.Message);
        }

        // Update an article
        public async Task<(bool Success, string Message)> UpdateArticleAsync(Article article)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can edit articles.");
            }

            // App-side validation
            string? error = NormaliseAndValidate(article);

            if (error is not null)
            {
                return (false, error);
            }

            ApiResult<MessageResponse> result = await _apiClient.PutAsync<MessageResponse>("api/articles/" + article.Id, ToRequest(article));

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
            string? imageError = await ImageUpload.ApplyAsync(_apiClient, "api/images/articles/" + article.Id, article.Image);

            if (imageError is not null)
            {
                return (true, message + " The photo couldn't be saved: " + imageError);
            }

            return (true, message);
        }

        // Delete an article
        public async Task<(bool Success, string Message)> DeleteArticleAsync(int articleId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can delete articles.");
            }

            ApiResult<MessageResponse> result = await _apiClient.DeleteAsync<MessageResponse>("api/articles/" + articleId);

            return ToTuple(result);
        }

        // Helpers
        // Trim and validate article details
        private static string? NormaliseAndValidate(Article article)
        {
            article.Title = Clean(article.Title);
            article.Topic = Clean(article.Topic);
            article.Content = Clean(article.Content);
            article.Citation = Clean(article.Citation);
            article.CitationUrl = Clean(article.CitationUrl);

            if (article.Title.Length < 2 || article.Title.Length > 100)
            {
                return "Title must be between 2 and 100 characters.";
            }

            if (article.Topic.Length == 0 || article.Topic.Length > 50)
            {
                return "Topic is required (50 characters max).";
            }

            if (article.Content.Length == 0 || article.Content.Length > 5000)
            {
                return "Content is required (5000 characters max).";
            }

            if (article.Citation.Length == 0 || article.Citation.Length > 300)
            {
                return "Citation is required (300 characters max).";
            }

            if (article.CitationUrl.Length > 0)
            {
                if (article.CitationUrl.Length > 300 || !IsValidLink(article.CitationUrl))
                {
                    return "Source link must be a valid web address (300 characters max).";
                }
            }

            if (article.DateTimePublished > DateTime.Now)
            {
                return "Published date can't be in the future.";
            }

            return null;
        }

        // Only allows full http or https links
        private static bool IsValidLink(string link)
        {
            Uri? uri;

            if (!Uri.TryCreate(link, UriKind.Absolute, out uri))
            {
                return false;
            }

            if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Create article object for request
        private static ArticleRequest ToRequest(Article article)
        {
            return new ArticleRequest
            {
                Title = article.Title,
                Topic = article.Topic,
                Content = article.Content,
                Citation = article.Citation,
                CitationUrl = article.CitationUrl,
                DateTimePublished = article.DateTimePublished
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