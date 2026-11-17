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
        [NotifyPropertyChangedFor(nameof(HasSourceLink))]
        [NotifyPropertyChangedFor(nameof(HasNoSourceLink))]
        public partial Article? Article { get; set; }

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

            IsBusy = true;

            try
            {
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
    }
}