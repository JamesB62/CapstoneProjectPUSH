namespace CapstoneProjectPUSH.ViewModels.Auth
{
    public partial class LoginViewModel : BaseViewModel
    {


        [ObservableProperty]
        public partial string Email { get; set; }

        [ObservableProperty]
        public partial string Password { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
        public partial string? ErrorMessage { get; set; } = string.Empty;

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public LoginViewModel(UserService userService) : base(userService)
        {
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

                Email = string.Empty;
                Password = string.Empty;

                IsBusy = false;

                await NavigateAsync("//HomePage");
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
            await NavigateAsync($"../{nameof(RegisterPage)}");
        }
    }
}