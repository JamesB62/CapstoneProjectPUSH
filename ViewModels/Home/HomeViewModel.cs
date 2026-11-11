namespace CapstoneProjectPUSH.ViewModels.Home
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly ArticleService _articleService;
        private readonly WorkoutService _workoutService;

        public HomeViewModel(ArticleService articleService, UserService userService, WorkoutService workoutService) : base(userService)
        {
            _articleService = articleService;
            _workoutService = workoutService;
        }

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

            // Uncomment once WorkoutDetailsPage is created
            // await NavigateAsync($"{nameof(WorkoutDetailsPage)}?WorkoutId={workout.Id}");
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
        private async Task GoToLoginAsync() => await NavigateAsync(nameof(LoginPage));

        [RelayCommand]
        private async Task GoToRegisterAsync() => await NavigateAsync(nameof(RegisterPage));


    }
}