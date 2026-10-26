using CapstoneProjectPUSH.Models;

namespace CapstoneProjectPUSH.ViewModels.Base
{
    public partial class TrainersViewModel : BaseViewModel
    {
        private List<CardItem> allTrainers = new List<CardItem>();

        public List<string> Specialties { get; set; } = new List<string> { "All", "Strength", "Cardio", "Flexibility" };

        [ObservableProperty]
        public partial string SelectedSpecialty { get; set; }

        [ObservableProperty]
        public partial List<CardItem> Trainers { get; set; }

        public TrainersViewModel()
        {
            allTrainers.Add(new CardItem { Title = "Alex Morgan", Subtitle = "Strength coach", Category = "Strength" });
            allTrainers.Add(new CardItem { Title = "Jordan Lee", Subtitle = "Running coach", Category = "Cardio" });
            allTrainers.Add(new CardItem { Title = "Sam Patel", Subtitle = "Mobility specialist", Category = "Flexibility" });
            allTrainers.Add(new CardItem { Title = "Riley Chen", Subtitle = "Powerlifting", Category = "Strength" });
            allTrainers.Add(new CardItem { Title = "Taylor Brooks", Subtitle = "HIIT instructor", Category = "Cardio" });

            SelectedSpecialty = "All";
            Trainers = allTrainers;
        }

        partial void OnSelectedSpecialtyChanged(string value)
        {
            if (value == "All" || string.IsNullOrEmpty(value))
            {
                Trainers = allTrainers;
            }
            else
            {
                Trainers = allTrainers.Where(t => t.Category == value).ToList();
            }
        }
    }
}
