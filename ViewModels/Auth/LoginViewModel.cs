namespace CapstoneProjectPUSH.ViewModels.Auth
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        [ObservableProperty]
        public partial string Email { get; set; }

        [ObservableProperty]
        public partial string Password { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
        private string? errorMessage;

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public LoginViewModel(UserService userService)
        {
            _userService = userService;
            Email = string.Empty;
            Password = string.Empty;
        }

        // Login command
        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var result = await _userService.LoginAsync(Email, Password);

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                Password = string.Empty;

                await Shell.Current.GoToAsync("//HomePage");
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

        // Register link command
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
                await Shell.Current.GoToAsync($"../{nameof(RegisterPage)}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}