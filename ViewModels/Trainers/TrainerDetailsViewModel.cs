namespace CapstoneProjectPUSH.ViewModels.Trainers
{
    [QueryProperty(nameof(TrainerId), "TrainerId")]
    public partial class TrainerDetailsViewModel : BaseViewModel
    {
        private readonly TrainerService _trainerService;

        public TrainerDetailsViewModel(TrainerService trainerService, UserService userService) : base(userService)
        {
            _trainerService = trainerService;
        }

        private const string PlaceholderImage = "profile_picture_placeholder.png";

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

            // AppShell sends signed-out users to the landing page
            if (user is null)
            {
                return;
            }

            IsBusy = true;

            try
            {
                var trainer = await _trainerService.GetTrainerByIdAsync(TrainerId);

                if (trainer != null)
                {
                    trainer.ProfileImage = GetProfileImageOrPlaceholder(trainer.ProfileImage);
                }

                Trainer = trainer;
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
                await Shell.Current.DisplayAlertAsync("Trainer Not Found", "This PUSH trainer could not be loaded.", "OK");
                await NavigateAsync("..");
            }
        }

        private static string GetProfileImageOrPlaceholder(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return PlaceholderImage;
            }

            if (!File.Exists(imagePath))
            {
                return PlaceholderImage;
            }

            return imagePath;
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
                ErrorMessage = "No email app was found on this device. Please try again.";
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
                ErrorMessage = "Couldn't open the email app. Please try again.";
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
                ErrorMessage = "This device can't make phone calls.";
                return;
            }

            try
            {
                PhoneDialer.Default.Open(Trainer!.PhoneNumber.Trim());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't open the phone dialer. Please try again.";
            }
        }
    }
}