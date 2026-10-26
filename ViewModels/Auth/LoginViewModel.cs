using CapstoneProjectPUSH.Views.Main;

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

        // Generates LoginCommand
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

                await Shell.Current.Navigation.PushAsync(new HomePage());
            }
            catch (Exception ex)
            {
                ErrorMessage = "Something went wrong. Please try again.";
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Generates GoToRegisterCommand
        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync($"../{nameof(RegisterPage)}");
        }
    }

}
