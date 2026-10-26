using CapstoneProjectPUSH.Models;

namespace CapstoneProjectPUSH.ViewModels.Base
{
    public partial class WorkoutsViewModel : BaseViewModel
    {
        private List<CardItem> allWorkouts = new List<CardItem>();

        [ObservableProperty]
        public partial string SearchText { get; set; }

        [ObservableProperty]
        public partial List<CardItem> Workouts { get; set; }

        public WorkoutsViewModel()
        {
            allWorkouts.Add(new CardItem { Title = "Push Day", Subtitle = "5 exercises" });
            allWorkouts.Add(new CardItem { Title = "Pull Day", Subtitle = "6 exercises" });
            allWorkouts.Add(new CardItem { Title = "Leg Day", Subtitle = "5 exercises" });
            allWorkouts.Add(new CardItem { Title = "Upper Body", Subtitle = "8 exercises" });
            allWorkouts.Add(new CardItem { Title = "Core Finisher", Subtitle = "4 exercises" });

            SearchText = string.Empty;
            Workouts = allWorkouts;
        }

        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Workouts = allWorkouts;
            }
            else
            {
                Workouts = allWorkouts.Where(w => w.Title.ToLower().Contains(value.ToLower())).ToList();
            }
        }

        // Generates AddWorkoutCommand
        [RelayCommand]
        private async Task AddWorkoutAsync()
        {
            await Shell.Current.DisplayAlertAsync("Coming soon", "Adding workouts isn't built yet.", "OK");
        }
    }
}
