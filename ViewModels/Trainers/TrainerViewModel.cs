namespace CapstoneProjectPUSH.ViewModels.Trainers
{
    public partial class TrainerViewModel : BaseViewModel
    {
        private readonly TrainerService _trainerService;

        public TrainerViewModel(TrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        public ObservableCollection<Trainer> Trainers { get; } = new();

        [ObservableProperty]
        private string statusMessage = "Loading trainers...";

        [RelayCommand]
        private async Task LoadTrainersAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            StatusMessage = "Loading trainers...";

            try
            {
                var trainers = await _trainerService.GetTrainersAsync();

                Trainers.Clear();
                foreach (var trainer in trainers)
                {
                    Trainers.Add(trainer);
                }

                switch (Trainers.Count)
                {
                    case 0:
                        StatusMessage = "No trainers found";
                        break;

                    case 1:
                        StatusMessage = "Showing 1 trainer";
                        break;

                    default:
                        StatusMessage = $"Showing {Trainers.Count} trainers";
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Could not load trainers, please try again";
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Add more functions when needed
    }
}