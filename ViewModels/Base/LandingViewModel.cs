namespace CapstoneProjectPUSH.ViewModels.Base
{
    public partial class LandingViewModel : BaseViewModel
    {
        // Generates GoToLoginCommand
        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.Auth.LoginPage));
        }

        // Generates GoToRegisterCommand
        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.Auth.RegisterPage));
        }

        // Generates GoToPrivacyCommand
        [RelayCommand]
        private async Task GoToPrivacyAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.Base.PrivacyPage));
        }
    }
}
