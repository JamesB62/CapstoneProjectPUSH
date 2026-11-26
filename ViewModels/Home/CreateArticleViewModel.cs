namespace CapstoneProjectPUSH.ViewModels.Home
{
    [QueryProperty(nameof(ArticleId), "ArticleId")]
    public partial class CreateArticleViewModel : BaseViewModel
    {
        private readonly ArticleService _articleService;
        private readonly ImageService _imageService;

        public CreateArticleViewModel(ArticleService articleService, ImageService imageService, UserService userService) : base(userService)
        {
            _articleService = articleService;
            _imageService = imageService;
        }

        private string _savedArticleImage = string.Empty;

        private bool _hasLoadedArticle;

        [ObservableProperty]
        public partial int ArticleId { get; set; }

        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Add PUSH Article";

        [ObservableProperty]
        public partial string SaveButtonText { get; set; } = "Save Article";

        [ObservableProperty]
        public partial string Title { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Topic { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Content { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Citation { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string CitationUrl { get; set; } = string.Empty;

        // DatePicker uses a nullable date in .NET 10
        [ObservableProperty]
        public partial DateTime? PublishedDate { get; set; } = DateTime.Today;

        // Stops future dates being picked
        public DateTime MaxPublishedDate
        {
            get
            {
                return DateTime.Today;
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasArticleImage))]
        [NotifyPropertyChangedFor(nameof(PhotoStatusText))]
        public partial string ArticleImage { get; set; } = string.Empty;

        public bool HasArticleImage
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ArticleImage);
            }
        }

        public string PhotoStatusText
        {
            get
            {
                if (HasArticleImage)
                {
                    return "Photo selected";
                }

                return "No photo";
            }
        }

        partial void OnArticleIdChanged(int value)
        {
            if (value > 0)
            {
                PageTitle = "Edit PUSH Article";
                SaveButtonText = "Save Changes";

                LoadCommand.Execute(null);
            }
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            ErrorMessage = string.Empty;

            if (!await RequireAdminAsync())
            {
                return;
            }

            // Add mode, or already filled in
            if (ArticleId <= 0 || _hasLoadedArticle)
            {
                return;
            }

            try
            {
                Article? article = await _articleService.GetArticleByIdAsync(ArticleId);

                if (article is null)
                {
                    await Shell.Current.DisplayAlertAsync(
                        "Article Not Found",
                        "That article no longer exists.",
                        "OK");

                    await NavigateAsync("..");
                    return;
                }

                Title = article.Title;
                Topic = article.Topic;
                Content = article.Content;
                Citation = article.Citation;
                CitationUrl = article.CitationUrl;
                PublishedDate = article.DateTimePublished.Date;

                // The seeded placeholder isn't a real photo
                if (article.Image is null || article.Image == "article_placeholder.png")
                {
                    _savedArticleImage = string.Empty;
                }
                else
                {
                    _savedArticleImage = article.Image;
                }

                ArticleImage = _savedArticleImage;

                _hasLoadedArticle = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load the form. Please try again.";
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;

            if (PublishedDate is null)
            {
                ErrorMessage = "Please choose a published date.";
                return;
            }

            IsBusy = true;

            try
            {
                bool isEditing = ArticleId > 0;

                Article article = new Article
                {
                    Id = ArticleId,
                    Title = Title,
                    Topic = Topic,
                    Content = Content,
                    Citation = Citation,
                    CitationUrl = CitationUrl,
                    DateTimePublished = PublishedDate.Value.Date,
                    Image = ArticleImage
                };

                (bool Success, string Message) result;

                if (isEditing)
                {
                    result = await _articleService.UpdateArticleAsync(article);
                }
                else
                {
                    result = await _articleService.AddArticleAsync(article);
                }

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                _savedArticleImage = ArticleImage;

                string articleTitle = Title.Trim();

                if (isEditing)
                {
                    await Shell.Current.DisplayAlertAsync("Article Updated", articleTitle + " has been updated.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Article Created", articleTitle + " has been created.", "OK");
                }

                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't save the article. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            DiscardUnsavedPhoto();

            await NavigateAsync("..");
        }

        [RelayCommand]
        private async Task ChangePhotoAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;

            try
            {
                string? newImage = await _imageService.PickImageAsync();

                if (newImage is null)
                {
                    return;
                }

                DiscardUnsavedPhoto();
                ArticleImage = newImage;
            }
            catch (PermissionException)
            {
                ErrorMessage = "PUSH needs permission to access your photos.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load that photo. Please try another.";
            }
        }

        [RelayCommand]
        private void RemovePhoto()
        {
            DiscardUnsavedPhoto();
            ArticleImage = string.Empty;
        }

        // Deletes unused photo
        private void DiscardUnsavedPhoto()
        {
            if (ArticleImage != _savedArticleImage)
            {
                ImageService.DeleteImage(ArticleImage);
            }
        }
    }
}