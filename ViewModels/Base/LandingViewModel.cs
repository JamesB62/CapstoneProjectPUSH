namespace CapstoneProjectPUSH.ViewModels.Base
{
    public partial class LandingViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
        public partial string? ErrorMessage { get; set; } = string.Empty;

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public LandingViewModel(UserService userService)
        {
            _userService = userService;
        }
        // Login command
        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await NavigateAsync(nameof(LoginPage));
        }

        // Register command
        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            await NavigateAsync(nameof(RegisterPage));
        }

        // Privacy link command
        [RelayCommand]
        private async Task GoToPrivacyAsync()
        {
            await NavigateAsync(nameof(PrivacyPage));
        }

        [RelayCommand]
        private async Task GuestLoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var result = await _userService.GuestLoginAsync();

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                IsBusy = false;

                await NavigateAsync("//HomePage");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Something went wrong, please try again.";
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}