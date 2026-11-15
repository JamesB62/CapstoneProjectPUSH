namespace CapstoneProjectPUSH.ViewModels.Auth
{
    public partial class RegisterViewModel : BaseViewModel
    {
        public RegisterViewModel(UserService userService) : base(userService)
        {
        }

        [ObservableProperty]
        public partial string DisplayName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Password { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string ConfirmPassword { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool HasAcceptedPrivacy { get; set; }

        public string PasswordRequirements => UserService.PasswordRequirementsText;

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                var result = await _userService.RegisterAsync(DisplayName, Email, Password, ConfirmPassword, HasAcceptedPrivacy);

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                DisplayName = string.Empty;
                Email = string.Empty;
                Password = string.Empty;
                ConfirmPassword = string.Empty;
                HasAcceptedPrivacy = false;

                await Shell.Current.DisplayAlertAsync("Account Created", "Your PUSH account has been created.", "OK");
                await NavigateAsync("//HomePage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Something went wrong. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await NavigateAsync($"../{nameof(LoginPage)}");
        }

        [RelayCommand]
        private async Task GoToPrivacyAsync()
        {
            await NavigateAsync(nameof(PrivacyPage));
        }
    }
}