namespace CapstoneProjectPUSH.ViewModels.Auth
{
    public partial class RegisterViewModel : BaseViewModel, IQueryAttributable
    {
        private bool _returnToPrevious;

        [ObservableProperty]
        public partial string DisplayName { get; set; }

        [ObservableProperty]
        public partial string Email { get; set; }

        [ObservableProperty]
        public partial string Password { get; set; }

        [ObservableProperty]
        public partial string ConfirmPassword { get; set; }

        [ObservableProperty]
        public partial bool HasAcceptedPrivacy { get; set; }

        public string PasswordRequirements => UserService.PasswordRequirementsText;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
        public partial string? ErrorMessage { get; set; } = string.Empty;

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public RegisterViewModel(UserService userService) : base(userService)
        {
            DisplayName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _returnToPrevious = query.TryGetValue("returnToPrevious", out var v) && v?.ToString() == "true";
        }



        // Register command
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

                var result = await _userService.RegisterAsync(DisplayName, Email, Password, ConfirmPassword, HasAcceptedPrivacy);

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                DisplayName = string.Empty;
                Email = string.Empty;
                Password = string.Empty;
                ConfirmPassword = string.Empty;

                IsBusy = false;

                await Shell.Current.DisplayAlertAsync("Register Successful", "Your PUSH account has been created.", "OK");
                //login back to before page
                if (_returnToPrevious)
                    {
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        await GoHomeAndClearStackAsync();
                    }
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

        // Login command
        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await NavigateAsync($"../{nameof(LoginPage)}");
        }

        // Privacy link command
        [RelayCommand]
        private async Task GoToPrivacyAsync()
        {
            await NavigateAsync(nameof(PrivacyPage));
        }

        private async Task GoHomeAndClearStackAsync()
        {
            // NavigationStack 第 0 项是根页面占位,大于 1 说明有被压进来的页面
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
            {
                await Shell.Current.Navigation.PopToRootAsync(false);
            }

            await NavigateAsync("//HomePage");
        }
    }
}