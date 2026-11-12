namespace CapstoneProjectPUSH.ViewModels.Home
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly ArticleService _articleService;
        private readonly UserService _userService;
        private readonly WeightLogService _weightLogService;
        private readonly WorkoutService _workoutService;

        public HomeViewModel(ArticleService articleService, UserService userService, WeightLogService weightLogService, WorkoutService workoutService)
        {
            _articleService = articleService;
            _userService = userService;
            _weightLogService = weightLogService;
            _workoutService = workoutService;
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

                var articles = await _articleService.GetArticlesAsync();

                List<Article> randomArticles = articles
                    .OrderBy(a => Random.Shared.Next())
                    .Take(3)
                    .ToList();

                FeaturedArticles.Clear();

                foreach (var article in randomArticles)
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

                await CheckWeighInPromptAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
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

        // Add weigh in value
        [RelayCommand]
        public async Task CheckWeighInPromptAsync()
        {
            var user = _userService.CurrentUser;

            if (user is null)
            {
                await NavigateAsync("//LandingPage");
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

            await AddWeighInAsync();
        }

        // Add weigh in value
        [RelayCommand]
        private async Task AddWeighInAsync()
        {
            var user = _userService.CurrentUser;

            if (user is null)
            {
                await NavigateAsync("//LandingPage");
                return;
            }

            // Value prompt
            string? result = await Shell.Current.DisplayPromptAsync(
                "Weigh-in",
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
                await Shell.Current.DisplayAlertAsync("Invalid weight", "Please enter a number, e.g. 72.5", "OK");
                return;
            }

            if (weightKg < MinWeightKg || weightKg > MaxWeightKg)
            {
                await Shell.Current.DisplayAlertAsync("Invalid weight", $"Weight must be between {MinWeightKg} and {MaxWeightKg} kg.", "OK");
                return;
            }

            try
            {
                await _weightLogService.SaveWeightForTodayAsync(user.Id, Math.Round(weightKg, 1));
                //await LoadStatsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                await Shell.Current.DisplayAlertAsync("Weigh-in failed", "Your weigh-in could not be saved. Please try again.", "OK");
            }
        }
    }
}