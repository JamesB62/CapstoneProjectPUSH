namespace CapstoneProjectPUSH.ViewModels.Trainers
{
    [QueryProperty(nameof(TrainerId), "TrainerId")]
    public partial class TrainerDetailsViewModel : BaseViewModel
    {
        private readonly TrainerService _trainerService;
        private readonly UserService _userService;

        public TrainerDetailsViewModel(TrainerService trainerService, UserService userService)
        {
            _trainerService = trainerService;
            _userService = userService;
        }

        [ObservableProperty]
        public partial int TrainerId { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasEmail))]
        [NotifyPropertyChangedFor(nameof(HasPhone))]
        [NotifyPropertyChangedFor(nameof(HasContactDetails))]
        [NotifyPropertyChangedFor(nameof(IsTrainerAvailable))]
        public partial Trainer? Trainer { get; set; }

        public bool HasContactDetails => HasEmail || HasPhone;
        public bool HasEmail => !string.IsNullOrWhiteSpace(Trainer?.Email);
        public bool HasPhone => !string.IsNullOrWhiteSpace(Trainer?.PhoneNumber);
        public bool IsTrainerAvailable => Trainer?.Available ?? false;

        partial void OnTrainerIdChanged(int value)
        {
            if (value <= 0)
            {
                return;
            }

            LoadTrainerCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadTrainerAsync()
        {
            var user = _userService.CurrentUser;

            if (user is null)
            {
                // Comment out to disable redirect
                await Shell.Current.GoToAsync("//LandingPage");
                return;
            }

            IsBusy = true;

            try
            {
                Trainer = await _trainerService.GetTrainerByIdAsync(TrainerId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }

            if (Trainer is null)
            {
                await Shell.Current.DisplayAlertAsync("Trainer not found", "This PUSH trainer could not be loaded.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        private async Task EmailTrainerAsync()
        {
            if (!HasEmail || !IsTrainerAvailable)
            {
                return;
            }

            if (!Email.Default.IsComposeSupported)
            {
                await Shell.Current.DisplayAlertAsync("Email unavailable", "No email app was found on this device.", "OK");
                return;
            }

            try
            {
                var message = new EmailMessage
                {
                    To = new List<string> { Trainer!.Email.Trim() }
                };

                await Email.Default.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                await Shell.Current.DisplayAlertAsync("Email unavailable", "Could not open the email app.", "OK");
            }
        }

        [RelayCommand]
        private async Task CallTrainerAsync()
        {
            if (!HasPhone || !IsTrainerAvailable)
            {
                return;
            }

            if (!PhoneDialer.Default.IsSupported)
            {
                await Shell.Current.DisplayAlertAsync("Calling unavailable", "This device can't make phone calls.", "OK");
                return;
            }

            try
            {
                PhoneDialer.Default.Open(Trainer!.PhoneNumber.Trim());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                await Shell.Current.DisplayAlertAsync("Calling unavailable", "Could not open the phone dialer.", "OK");
            }
        }
    }
}