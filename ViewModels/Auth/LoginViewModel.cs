namespace CapstoneProjectPUSH.ViewModels.Auth
{
    public partial class LoginViewModel : BaseViewModel, IQueryAttributable
    {
        public bool _returnToPrevious;

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

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _returnToPrevious = query.TryGetValue("returnToPrevious", out var v) && v?.ToString() == "true";
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
                await GoHomeAndClearStackAsync();//clean page after login

                IsBusy = false;

                //after login back to the page before
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

        //after login clean all page
        private async Task GoHomeAndClearStackAsync()
        {
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
            {
                await Shell.Current.Navigation.PopToRootAsync(false);
            }

            await NavigateAsync("//HomePage");
        }    
    }
}