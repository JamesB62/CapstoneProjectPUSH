namespace CapstoneProjectPUSH.ViewModels.Auth
{
    public partial class LoginViewModel : BaseViewModel
    {
        public LoginViewModel(UserService userService) : base(userService)
        {
        }

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Password { get; set; } = string.Empty;

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                var result = await _userService.LoginAsync(Email, Password);

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                Email = string.Empty;
                Password = string.Empty;

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

        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            await NavigateAsync($"../{nameof(RegisterPage)}");
        }
    }
}