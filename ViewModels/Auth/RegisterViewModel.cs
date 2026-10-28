namespace CapstoneProjectPUSH.ViewModels.Auth
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        [ObservableProperty]
        public partial string DisplayName { get; set; }

        [ObservableProperty]
        public partial string Email { get; set; }

        [ObservableProperty]
        public partial string Password { get; set; }

        [ObservableProperty]
        public partial string ConfirmPassword { get; set; }

        public string PasswordRequirements => UserService.PasswordRequirementsText;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
        private string? errorMessage;

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public RegisterViewModel(UserService userService)
        {
            _userService = userService;
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
                ErrorMessage = string.Empty;

                var result = await _userService.RegisterAsync(DisplayName, Email, Password, ConfirmPassword);

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                await Shell.Current.DisplayAlertAsync("Register Successful", "Your account has been created. Please log in.", "OK");
                await Shell.Current.GoToAsync($"../{nameof(LoginPage)}");
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

        // Generates GoToLoginCommand
        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync($"../{nameof(LoginPage)}");
        }
    }
}