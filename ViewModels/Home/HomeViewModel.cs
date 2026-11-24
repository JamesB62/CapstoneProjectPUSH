namespace CapstoneProjectPUSH.ViewModels.Home
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly ArticleService _articleService;
        private readonly WeightLogService _weightLogService;
        private readonly WorkoutService _workoutService;

        public HomeViewModel(ArticleService articleService, WorkoutService workoutService, WeightLogService weightLogService, UserService userService) : base(userService)
        {
            _articleService = articleService;
            _workoutService = workoutService;
            _weightLogService = weightLogService;
        }

        private const int WeightInPromptIntervalDays = 14;

        // Min and max weights for input validation
        private const double MinWeightKg = 20;
        private const double MaxWeightKg = 400;

        public ObservableCollection<Workout> FeaturedWorkouts { get; } = new();

        public ObservableCollection<Article> FeaturedArticles { get; } = new();

        [ObservableProperty]
        public partial Workout? SelectedWorkout { get; set; }

        [ObservableProperty]
        public partial Article? SelectedArticle { get; set; }

        [RelayCommand]
        private async Task LoadHomeAsync()
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
                List<Article> articles = await _articleService.GetArticlesAsync();

                FeaturedArticles.Clear();

                foreach (Article article in articles)
                {
                    FeaturedArticles.Add(article);
                }

                List<Workout> workouts = await _workoutService.GetWorkoutsForUserAsync(user.Id);

                List<Workout> randomWorkouts = workouts
                    .OrderBy(w => Random.Shared.Next())
                    .Take(3)
                    .ToList();

                FeaturedWorkouts.Clear();

                foreach (var workout in randomWorkouts)
                {
                    FeaturedWorkouts.Add(workout);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load your home page. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }

            await CheckWeighInPromptAsync();
        }

        partial void OnSelectedWorkoutChanged(Workout? value)
        {
            if (value is null)
            {
                return;
            }

            ViewWorkoutCommand.Execute(value);
            SelectedWorkout = null;
        }

        [RelayCommand]
        private async Task ViewWorkoutAsync(Workout? workout)
        {
            if (workout is null)
            {
                return;
            }

             await NavigateAsync($"{nameof(WorkoutDetailsPage)}?WorkoutId={workout.Id}");
        }

        partial void OnSelectedArticleChanged(Article? value)
        {
            if (value is null)
            {
                return;
            }

            ViewArticleCommand.Execute(value);
            SelectedArticle = null;
        }

        [RelayCommand]
        private async Task ViewArticleAsync(Article? article)
        {
            if (article is null)
            {
                return;
            }

            await NavigateAsync($"{nameof(ArticleDetailsPage)}?ArticleId={article.Id}");
        }

        [RelayCommand]
        private async Task AddArticleAsync()
        {
            if (!IsCurrentUserAdmin)
            {
                return;
            }

            await NavigateAsync(nameof(CreateArticlePage));
        }

        [RelayCommand]
        private async Task EditArticleAsync(Article? article)
        {
            if (article is null || !IsCurrentUserAdmin)
            {
                return;
            }

            await NavigateAsync(nameof(CreateArticlePage) + "?ArticleId=" + article.Id);
        }

        [RelayCommand]
        private async Task DeleteArticleAsync(Article? article)
        {
            if (article is null || !IsCurrentUserAdmin)
            {
                return;
            }

            ErrorMessage = string.Empty;

            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete Article",
                "Delete " + article.Title + "? This can't be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            try
            {
                (bool Success, string Message) result = await _articleService.DeleteArticleAsync(article.Id);

                if (result.Success)
                {
                    FeaturedArticles.Remove(article);
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
        }

        private async Task CheckWeighInPromptAsync()
        {
            var user = _userService.CurrentUser;

            // Guests don't record weigh-ins
            if (user is null || user.IsGuest)
            {
                return;
            }

            string key = $"LastWeighInPrompt_{user.Id}";
            DateTime lastPrompt = Preferences.Default.Get(key, DateTime.MinValue);

            bool shouldPrompt = false;

            if (lastPrompt == DateTime.MinValue)
            {
                shouldPrompt = true;
            }
            else
            {
                double daysSinceLastPrompt = (DateTime.Now - lastPrompt).TotalDays;

                if (daysSinceLastPrompt >= WeightInPromptIntervalDays)
                {
                    shouldPrompt = true;
                }
            }

            if (!shouldPrompt)
            {
                return;
            }

            Preferences.Default.Set(key, DateTime.Now);

            await AddWeighInAsync(user.Id);
        }

        private async Task AddWeighInAsync(int userId)
        {
            ErrorMessage = string.Empty;

            string? result = await Shell.Current.DisplayPromptAsync(
                "Add Weigh-In",
                "Enter today's weight in kg. This replaces any weigh-in already saved today.",
                "Save",
                "Cancel",
                maxLength: 6,
                keyboard: Keyboard.Numeric);

            if (result is null)
            {
                return;
            }

            if (!double.TryParse(result.Trim(), out double weightKg))
            {
                ErrorMessage = "Please enter your weight as a number, e.g. 72.5.";
                return;
            }

            if (weightKg < MinWeightKg || weightKg > MaxWeightKg)
            {
                ErrorMessage = $"Please enter a weight between {MinWeightKg} and {MaxWeightKg} kg.";
                return;
            }

            try
            {
                await _weightLogService.SaveWeightForTodayAsync(userId, Math.Round(weightKg, 1));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't save your weigh-in. Please try again.";
            }
        }
    }
}