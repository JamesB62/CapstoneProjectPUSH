using CapstoneProjectPUSH.Models;

namespace CapstoneProjectPUSH.ViewModels.Base
{
    public partial class HomeViewModel : BaseViewModel
    {
        private List<CardItem> allPlans = new List<CardItem>();
        private string currentCategory = string.Empty;

        public List<CardItem> FeaturedArticles { get; set; } = new List<CardItem>();

        [ObservableProperty]
        public partial List<CardItem> FeaturedPlans { get; set; }

        public HomeViewModel()
        {
            allPlans.Add(new CardItem { Title = "Push Day Basics", Subtitle = "Chest, shoulders, triceps", Category = "Strength" });
            allPlans.Add(new CardItem { Title = "Full Body Starter", Subtitle = "3 days a week", Category = "Strength" });
            allPlans.Add(new CardItem { Title = "Couch to 5K", Subtitle = "8 week running plan", Category = "Cardio" });
            allPlans.Add(new CardItem { Title = "HIIT Burner", Subtitle = "20 minute intervals", Category = "Cardio" });
            allPlans.Add(new CardItem { Title = "Morning Stretch", Subtitle = "10 minute routine", Category = "Flexibility" });

            FeaturedArticles.Add(new CardItem { Title = "How Much Protein Do You Need?", Subtitle = "5 min read" });
            FeaturedArticles.Add(new CardItem { Title = "Progressive Overload Explained", Subtitle = "4 min read" });
            FeaturedArticles.Add(new CardItem { Title = "Sleep and Muscle Recovery", Subtitle = "6 min read" });

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
