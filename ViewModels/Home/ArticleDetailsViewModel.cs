namespace CapstoneProjectPUSH.ViewModels.Home
{
    [QueryProperty(nameof(ArticleId), "ArticleId")]
    public partial class ArticleDetailsViewModel : BaseViewModel
    {
        private readonly ArticleService _articleService;


        public ArticleDetailsViewModel(ArticleService articleService, UserService userService) : base(userService)
        {
            _articleService = articleService;

        }

        [ObservableProperty]
        public partial int ArticleId { get; set; }

        [ObservableProperty]
        public partial Article? Article { get; set; }

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
            IsBusy = true;

            try
            {
                var user = _userService.CurrentUser;

                if (user is null)
                {
                    // Comment out to disable redirect
                    await NavigateAsync("//LandingPage");
                    return;
                }

                Article = await _articleService.GetArticleByIdAsync(ArticleId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }

            if (Article is null && _userService.CurrentUser is not null)
            {
                await Shell.Current.DisplayAlertAsync("Article not found", "This PUSH article could not be loaded.", "OK");
                await NavigateAsync("..");
            }
        }

        [RelayCommand]
        private async Task OpenSourceAsync()
        {
            if (Article is null || string.IsNullOrWhiteSpace(Article.CitationUrl))
            {
                return;
            }

            if (!Uri.TryCreate(Article.CitationUrl, UriKind.Absolute, out Uri? uri))
            {
                await Shell.Current.DisplayAlertAsync("Invalid link", "This source link could not be opened.", "OK");
                return;
            }

            try
            {
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                await Shell.Current.DisplayAlertAsync("Unable to open link", "Something went wrong opening the source.", "OK");
            }
        }
    }
}