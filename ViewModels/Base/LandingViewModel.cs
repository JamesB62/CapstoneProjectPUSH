namespace CapstoneProjectPUSH.ViewModels.Base
{
    public partial class LandingViewModel : BaseViewModel
    {
        private readonly UserService _userService;

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
            // var result = await _userService.GuestLoginAsync();

            // if (!result.Success)
            // {
            //     StatusMessage = result.Message;
            //     return;
            // }

            await NavigateAsync("//HomePage");
        }
    }
}