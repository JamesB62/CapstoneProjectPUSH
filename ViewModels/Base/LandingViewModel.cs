namespace CapstoneProjectPUSH.ViewModels.Base
{
    public partial class LandingViewModel : BaseViewModel
    {
        public LandingViewModel(UserService userService) : base(userService)
        {
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

        // Sign in with the seeded guest account
        [RelayCommand]
        private async Task GuestLoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                var result = await _userService.GuestLoginAsync();

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

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
    }
}