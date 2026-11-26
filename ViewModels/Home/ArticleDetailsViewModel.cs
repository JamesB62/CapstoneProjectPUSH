namespace CapstoneProjectPUSH.ViewModels.Home
{
    [QueryProperty(nameof(ArticleId), "ArticleId")]
    public partial class ArticleDetailsViewModel : BaseViewModel
    {
        private const string PlaceholderImage = "article_placeholder.png";

        private readonly ArticleService _articleService;

        public ArticleDetailsViewModel(ArticleService articleService, UserService userService) : base(userService)
        {
            _articleService = articleService;
        }

        [ObservableProperty]
        public partial int ArticleId { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSourceLink))]
        [NotifyPropertyChangedFor(nameof(HasNoSourceLink))]
        public partial Article? Article { get; set; }

        [ObservableProperty]
        public partial bool CanManage { get; set; }

        public bool HasSourceLink => Article is not null && !string.IsNullOrWhiteSpace(Article.CitationUrl);

        public bool HasNoSourceLink => !HasSourceLink;

        partial void OnArticleIdChanged(int value)
        {
            if (value <= 0)
            {
                return;
            }

            LoadArticleCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadArticleAsync()
        {
            ErrorMessage = string.Empty;

            var user = _userService.CurrentUser;

            // AppShell sends signed-out users to the landing page
            if (user is null)
            {
                return;
            }

            CanManage = _userService.IsCurrentUserAdmin();
            IsBusy = true;

            try
            {
                Article? article = await _articleService.GetArticleByIdAsync(ArticleId);

                if (article is not null)
                {
                    article.Image = GetImageOrPlaceholder(article.Image);
                }

                Article = article;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }

            if (Article is null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Article Not Found",
                    "This PUSH article could not be loaded.",
                    "OK");

                await NavigateAsync("..");
            }
        }

        [RelayCommand]
        private async Task OpenSourceAsync()
        {
            ErrorMessage = string.Empty;

            if (!HasSourceLink)
            {
                return;
            }

            if (!Uri.TryCreate(Article!.CitationUrl, UriKind.Absolute, out Uri? uri))
            {
                ErrorMessage = "This source link isn't valid.";
                return;
            }

            try
            {
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't open the source link.";
            }
        }

        // Reloads after coming back from the edit form
        public void Reload()
        {
            if (Article is null)
            {
                return;
            }

            LoadArticleCommand.Execute(null);
        }

        [RelayCommand]
        private async Task EditArticleAsync()
        {
            if (Article is null || !CanManage)
            {
                return;
            }

            await NavigateAsync(nameof(CreateArticlePage) + "?ArticleId=" + Article.Id);
        }

        [RelayCommand]
        private async Task DeleteArticleAsync()
        {
            if (Article is null || !CanManage || IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;

            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete Article",
                "Delete " + Article.Title + "? This can't be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            IsBusy = true;

            try
            {
                (bool Success, string Message) result = await _articleService.DeleteArticleAsync(Article.Id);

                if (result.Success)
                {
                    await NavigateAsync("..");
                }
                else
                {
                    ErrorMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't delete the article. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private static string GetImageOrPlaceholder(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return PlaceholderImage;
            }

            // Uploaded photo -- a web address from the api, so there's no file to check
            if (imagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || imagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return imagePath;
            }

            if (!File.Exists(imagePath))
            {
                return PlaceholderImage;
            }

            return imagePath;
        }
    }
}