namespace CapstoneProjectPUSH.ViewModels.Base
{
    public partial class LandingViewModel : BaseViewModel
    {
        // Login command
        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }

        // Register command
        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }

        // Privacy link command
        [RelayCommand]
        private async Task GoToPrivacyAsync()
        {
            await Shell.Current.GoToAsync(nameof(PrivacyPage));
        }
    }
}