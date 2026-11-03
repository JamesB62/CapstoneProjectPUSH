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
        private string displayName = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string currentPassword = string.Empty;

        [ObservableProperty]
        private string newPassword = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
        private string? errorMessage;

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        [RelayCommand]
        private async Task Load()
        {
            var user = _userService.CurrentUser;
            if (user is null)
            {
                return;
            }

            DisplayName = user.DisplayName;
            Email = user.Email;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            ErrorMessage = string.Empty;
        }

        [RelayCommand]
        private async Task SaveAsync()
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

                if (result.User is not null)
                {
                    DisplayName = result.User.DisplayName;
                    Email = result.User.Email;
                }

                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;

                await Shell.Current.DisplayAlertAsync("Update Successful", "Your PUSH account details have been updated.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}