namespace CapstoneProjectPUSH.ViewModels.Base
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        public partial bool IsBusy { get; set; }

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasStatusMessage))]
        public partial string StatusMessage { get; set; } = string.Empty;

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        private bool _isNavigating;

        protected readonly UserService _userService;

        public BaseViewModel(UserService userService)
        {
            _userService = userService;
        }

        //guest
        public bool IsGuest => _userService.IsGuest;
        public bool IsLoggedIn => !IsGuest;

        public void RefreshUserState()
        {
            OnPropertyChanged(nameof(IsGuest));
            OnPropertyChanged(nameof(IsLoggedIn));
        }

        //User login require if guest
        protected async Task<bool> RequireLoginAsync()
        {
            if (!IsGuest)
            {
                return true; 
            }

            bool goToLogin = await Shell.Current.DisplayAlertAsync(
                "Login Required",
                "You are currently browsing as a guest. Please log in to use this feature.",
                "Log In",
                "Cancel");

            if (goToLogin)
            {

                await NavigateAsync(nameof(LoginPage));
            }

            return false; //block user
        }


        //guest

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