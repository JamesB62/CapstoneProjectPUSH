namespace CapstoneProjectPUSH.ViewModels.Base
{
    public partial class LandingViewModel : BaseViewModel
    {
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
    }
}