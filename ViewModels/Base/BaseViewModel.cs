namespace CapstoneProjectPUSH.ViewModels.Base
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        // Injections
        protected readonly UserService _userService;

        public BaseViewModel(UserService userService)
        {
            _userService = userService;
        }

        // Reusable properties
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        public partial bool IsBusy { get; set; }

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasStatusMessage))]
        public partial string StatusMessage { get; set; } = string.Empty;

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        // Signed-in state
        public bool IsUnauthenticated => _userService.IsUnauthenticated;
        public bool IsGuest => _userService.IsGuest;
        public bool IsLoggedIn => _userService.IsLoggedIn;

        public void RefreshUserState()
        {
            OnPropertyChanged(nameof(IsUnauthenticated));
            OnPropertyChanged(nameof(IsGuest));
            OnPropertyChanged(nameof(IsLoggedIn));
        }

        // Registered account check -- login/register prompt for guests
        protected async Task<bool> RequireLoginAsync()
        {
            if (IsLoggedIn)
            {
                return true;
            }

            if (IsUnauthenticated)
            {
                await NavigateAsync("//LandingPage");
                return false;
            }

            bool goToLanding = await Shell.Current.DisplayAlertAsync(
                "Account Required",
                "You are browsing as a guest. Log in or register to use this feature.",
                "Log In or Register",
                "Cancel");

            if (goToLanding)
            {
                await LogoutAndRedirectAsync();
            }

            return false;
        }

        // Signs out the current user
        protected async Task LogoutAndRedirectAsync()
        {
            _userService.Logout();
            RefreshUserState();

            await NavigateAsync("//LandingPage");
        }

        // Navigation
        private bool _isNavigating;

        protected async Task NavigateAsync(string route)
        {
            if (_isNavigating)
            {
                return;
            }

            _isNavigating = true;

            try
            {
                await Shell.Current.GoToAsync(route);
            }
            finally
            {
                _isNavigating = false;
            }
        }

        // Navigation with parameters
        protected async Task NavigateAsync(string route, ShellNavigationQueryParameters parameters)
        {
            if (_isNavigating)
            {
                return;
            }

            _isNavigating = true;

            try
            {
                await Shell.Current.GoToAsync(route, parameters);
            }
            finally
            {
                _isNavigating = false;
            }
        }
    }
}