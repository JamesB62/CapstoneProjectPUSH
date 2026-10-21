namespace CapstoneProjectPUSH.ViewModels.Auth
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly INavigationService _navigation;

        [ObservableProperty]
        public partial string DisplayName { get; set; }

        [ObservableProperty]
        public partial string Email { get; set; }

        [ObservableProperty]
        public partial string Password { get; set; }

        [ObservableProperty]
        public partial string ConfirmPassword { get; set; }

        public string PasswordRequirements => UserService.PasswordRequirementsText;

        public RegisterViewModel(UserService userService, INavigationService navigation)
        {
            _userService = userService;
            _navigation = navigation;
            DisplayName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }

        // Generates RegisterCommand
        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                StatusMessage = string.Empty;

                var result = await _userService.RegisterAsync(DisplayName, Email, Password, ConfirmPassword);

                if (!result.Success)
                {
                    StatusMessage = result.Message;
                    return;
                }

                await _navigation.ShowAlertAsync("Success", "Your account has been created. Please log in.");
                await _navigation.GoToRootAsync();
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

        // Generates GoToLoginCommand
        // Causes navigation to become stuck
        [RelayCommand]
        private Task GoToLoginAsync() => _navigation.GoToAsync<LoginPage>();
    }
}