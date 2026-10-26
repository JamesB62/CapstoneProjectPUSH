namespace CapstoneProjectPUSH.ViewModels.Stats
{
    public partial class StatsViewModel : BaseViewModel
    {
        [ObservableProperty]
        public partial string SelectedFilter { get; set; }

        public StatsViewModel()
        {
            SelectedFilter = "Week";
        }

        // Generates SelectFilterCommand
        [RelayCommand]
        private void SelectFilter(string filter)
        {
            SelectedFilter = filter;
        }
    }
}
