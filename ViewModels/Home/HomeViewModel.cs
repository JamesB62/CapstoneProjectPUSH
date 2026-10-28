namespace CapstoneProjectPUSH.ViewModels.Home
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly ArticleService _articleService;
        private readonly CategoryService _categoryService;
        private readonly UserService _userService;
        private readonly WorkoutService _workoutService;

        public HomeViewModel(
            ArticleService articleService,
            CategoryService categoryService,
            UserService userService,
            WorkoutService workoutService)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _userService = userService;
            _workoutService = workoutService;
        }

        private List<Category> _categories = new();
        private List<Workout> AllWorkouts = new();
        private int currentCategoryId = 0;

        public ObservableCollection<Article> FeaturedArticles { get; } = new();

        public ObservableCollection<Workout> Workouts { get; } = new();

        [RelayCommand]
        private async Task LoadHomeAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                _categories = await _categoryService.GetCategoriesAsync();

                var articles = await _articleService.GetArticlesAsync();

                FeaturedArticles.Clear();

                // Currently grabs the first 3 articles, change?
                foreach (var article in articles.Take(3))
                {
                    FeaturedArticles.Add(article);
                }

                // Only shows logged in user's workouts
                var user = _userService.CurrentUser;

                if (user is null)
                {
                    AllWorkouts = new List<Workout>();
                }
                else
                {
                    AllWorkouts = await _workoutService.GetWorkoutsForUserAsync(user.Id);
                }

                ApplyFilter();
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

        // Generates SelectCategoryCommand
        [RelayCommand]
        private void SelectCategory(string categoryName)
        {
            var category = _categories.FirstOrDefault(c => c.Name == categoryName);

            if (category is null)
            {
                return;
            }

            if (currentCategoryId == category.Id)
            {
                currentCategoryId = 0;
            }
            else
            {
                currentCategoryId = category.Id;
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            List<Workout> filtered;

            if (currentCategoryId == 0)
            {
                filtered = AllWorkouts;
            }
            else
            {
                filtered = AllWorkouts.Where(w => w.CategoryId == currentCategoryId).ToList();
            }

            Workouts.Clear();

            foreach (var workout in filtered)
            {
                Workouts.Add(workout);
            }
        }
    }
}