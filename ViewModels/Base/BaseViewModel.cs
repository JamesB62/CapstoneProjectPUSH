namespace CapstoneProjectPUSH.ViewModels.Base
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial string StatusMessage { get; set; }

        public bool IsNotBusy => !IsBusy;

        protected BaseViewModel()
        {
            StatusMessage = string.Empty;
        }

        protected async Task NavigateAsync(string route)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                await Shell.Current.GoToAsync(route);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}