using CapstoneProjectPUSH.Models;

namespace CapstoneProjectPUSH.ViewModels.Home
{
    public partial class HomeViewModel : BaseViewModel
    {
        private List<Workout> allPlans = new List<Workout>();
        private string currentCategory = string.Empty;

        public List<Article> FeaturedArticles { get; set; } = new List<Article>();

        [ObservableProperty]
        public partial List<Workout> FeaturedPlans { get; set; }

        public HomeViewModel()
        {
            allPlans.Add(new Workout { Name = "Push Day Basics", Subtitle = "Chest, shoulders, triceps", CategoryName = "Strength" });
            allPlans.Add(new Workout { Name = "Full Body Starter", Subtitle = "3 days a week", CategoryName = "Strength" });
            allPlans.Add(new Workout { Name = "Couch to 5K", Subtitle = "8 week running plan", CategoryName = "Cardio" });
            allPlans.Add(new Workout { Name = "HIIT Burner", Subtitle = "20 minute intervals", CategoryName = "Cardio" });
            allPlans.Add(new Workout { Name = "Morning Stretch", Subtitle = "10 minute routine", CategoryName = "Flexibility" });

            FeaturedArticles.Add(new Article { Title = "How Much Protein Do You Need?", Subtitle = "5 min read" });
            FeaturedArticles.Add(new Article { Title = "Progressive Overload Explained", Subtitle = "4 min read" });
            FeaturedArticles.Add(new Article { Title = "Sleep and Muscle Recovery", Subtitle = "6 min read" });

            FeaturedPlans = allPlans;
        }

        // Generates SelectCategoryCommand
        [RelayCommand]
        private void SelectCategory(string category)
        {
            if (currentCategory == category)
            {
                currentCategory = string.Empty;
                FeaturedPlans = allPlans;
            }
            else
            {
                currentCategory = category;
                FeaturedPlans = allPlans.Where(p => p.CategoryName == category).ToList();
            }
        }
    }
}
