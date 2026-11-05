namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class UpdateProfileViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        public UpdateProfileViewModel(UserService userService)
        {
            _userService = userService;
        }

        [ObservableProperty]
        public partial string DisplayName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string CurrentPassword { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string NewPassword { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string ConfirmPassword { get; set; } = string.Empty;

        public string PasswordRequirements => UserService.PasswordRequirementsText;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
        public partial string? ErrorMessage { get; set; }

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        [RelayCommand]
        private async Task LoadProfileAsync()
        {
            var user = _userService.CurrentUser;

            if (user != null)
            {
                DisplayName = user.DisplayName;
                Email = user.Email;
                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
                ErrorMessage = string.Empty;
            }
            else
            {
                // Comment out to disable redirect
                await Shell.Current.GoToAsync("//LandingPage");
            }
        }

        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _userService.UpdateDetailsAsync(DisplayName, Email, CurrentPassword, NewPassword, ConfirmPassword);

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;

                IsBusy = false;

                await Shell.Current.DisplayAlertAsync("Update Successful", "Your PUSH account details have been updated.", "OK");
                await Shell.Current.GoToAsync("..");
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
        private async Task CancelProfileAsync()
        {
            if (IsBusy)
            {
                return;
            }

            await Shell.Current.GoToAsync("..");
        }
    }
}