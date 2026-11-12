namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class UpdateProfileViewModel : BaseViewModel
    {


        public UpdateProfileViewModel(UserService userService) : base(userService)
        {

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
                StatusMessage = string.Empty;
            }
            else
            {
                // Comment out to disable redirect
                await NavigateAsync("//LandingPage");
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
            StatusMessage = string.Empty;

            try
            {
                var result = await _userService.UpdateDetailsAsync(DisplayName, Email, CurrentPassword, NewPassword, ConfirmPassword);

                if (!result.Success)
                {
                    StatusMessage = result.Message;
                    return;
                }

                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;

                IsBusy = false;

                await Shell.Current.DisplayAlertAsync("Update Successful", "Your PUSH account details have been updated.", "OK");
                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Something went wrong. Please try again.";
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

            await NavigateAsync("..");
        }
    }
}