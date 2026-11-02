namespace CapstoneProjectPUSH.ViewModels.Base
{
    public partial class LandingViewModel : BaseViewModel
    {
        // Login command
        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            try
            {
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Register command
        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            try
            {
                await Shell.Current.GoToAsync(nameof(RegisterPage));
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Privacy link command
        [RelayCommand]
        private async Task GoToPrivacyAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            try
            {
                await Shell.Current.GoToAsync(nameof(PrivacyPage));
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}