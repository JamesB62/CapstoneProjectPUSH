using CapstoneProjectPUSH.Models;

namespace CapstoneProjectPUSH.ViewModels.Home
{
    public partial class HomeViewModel : BaseViewModel
    {
        private List<Workout> allPlans = new List<Workout>();
        private string currentCategory = string.Empty;

        public List<Articles> FeaturedArticles { get; set; } = new List<Articles>();

        [ObservableProperty]
        public partial List<Workout> FeaturedPlans { get; set; }

        public HomeViewModel()
        {
            allPlans.Add(new Workout { Title = "Push Day Basics", Subtitle = "Chest, shoulders, triceps", Category = "Strength" });
            allPlans.Add(new Workout { Title = "Full Body Starter", Subtitle = "3 days a week", Category = "Strength" });
            allPlans.Add(new Workout { Title = "Couch to 5K", Subtitle = "8 week running plan", Category = "Cardio" });
            allPlans.Add(new Workout { Title = "HIIT Burner", Subtitle = "20 minute intervals", Category = "Cardio" });
            allPlans.Add(new Workout { Title = "Morning Stretch", Subtitle = "10 minute routine", Category = "Flexibility" });

            FeaturedArticles.Add(new Articles { Title = "How Much Protein Do You Need?", Subtitle = "5 min read" });
            FeaturedArticles.Add(new Articles { Title = "Progressive Overload Explained", Subtitle = "4 min read" });
            FeaturedArticles.Add(new Articles { Title = "Sleep and Muscle Recovery", Subtitle = "6 min read" });

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
                FeaturedPlans = allPlans.Where(p => p.Category == category).ToList();
            }
        }
    }
}
