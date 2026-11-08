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