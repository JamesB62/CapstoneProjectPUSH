namespace CapstoneProjectPUSH.ViewModels.Auth
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly INavigationService _navigation;

        [ObservableProperty]
        public partial string Email { get; set; }

        [ObservableProperty]
        public partial string Password { get; set; }

        public LoginViewModel(UserService userService, INavigationService navigation)
        {
            _userService = userService;
            _navigation = navigation;
            Email = string.Empty;
            Password = string.Empty;
        }

        // Generates LoginCommand
        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                StatusMessage = string.Empty;

                var result = await _userService.LoginAsync(Email, Password);

                if (!result.Success)
                {
                    StatusMessage = result.Message;
                    return;
                }

                Password = string.Empty;

                // Replace with navigation to dashboard page
                await _navigation.ShowAlertAsync("Logged in", result.Message);
            }
            catch (Exception ex)
            {
                StatusMessage = "Something went wrong. Please try again.";
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Generates GoToRegisterCommand
        [RelayCommand]
        private Task GoToRegisterAsync() => _navigation.GoToAsync<RegisterPage>();
    }
}