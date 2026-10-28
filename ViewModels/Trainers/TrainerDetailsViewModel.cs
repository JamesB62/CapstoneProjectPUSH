namespace CapstoneProjectPUSH.ViewModels.Trainers
{
    [QueryProperty(nameof(TrainerId), "TrainerId")]
    public partial class TrainerDetailsViewModel : BaseViewModel
    {
        private readonly TrainerService _trainerService;

        public TrainerDetailsViewModel(TrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        [ObservableProperty]
        private int? trainerId;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasEmail))]
        [NotifyPropertyChangedFor(nameof(HasPhone))]
        private Trainer? trainer;

        public bool HasEmail => !string.IsNullOrWhiteSpace(Trainer?.Email);
        public bool HasPhone => !string.IsNullOrWhiteSpace(Trainer?.PhoneNumber);

        partial void OnTrainerIdChanged(int? value)
        {
            if (value is null)
            {
                return;
            }

            LoadTrainerCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadTrainerAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                Trainer = await _trainerService.GetTrainerByIdAsync(TrainerId!.Value);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}