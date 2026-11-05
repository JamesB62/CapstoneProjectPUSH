namespace CapstoneProjectPUSH.ViewModels.Admins
{
    public partial class UsersManagementViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        public UsersManagementViewModel(UserService userService)
        {
            _userService = userService;
        }

        public ObservableCollection<User> Users { get; } = new();

        private List<User> _allUsers = new();

        [ObservableProperty]
        public partial User? SelectedUser { get; set; }

        [ObservableProperty]
        public partial string StatusMessage { get; set; } = "Loading users...";

        private bool _isNavigating;

        [RelayCommand]
        private async Task LoadUsersAsync()
        {
            var user = _userService.CurrentUser;

            if (user is null)
            {
                // Comment out to disable redirect
                await Shell.Current.GoToAsync("//LandingPage");
                return;
            }

            if (user.IsAdmin == false)
            {
                // Comment out to disable redirect
                await Shell.Current.GoToAsync("//ProfilePage");
                return;
            }

            IsBusy = true;
            StatusMessage = "Loading users...";

            try
            {
                var users = await _userService.GetUsersAsync();
                _allUsers = users.ToList();
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

        partial void OnSelectedUserChanged(User? value)
        {
            if (value is null)
            {
                return;
            }

            ViewUserCommand.Execute(value);
            SelectedUser = null;
        }

        [RelayCommand]
        private async Task ViewUserAsync(User? user)
        {
            if (user is null || _isNavigating)
            {
                return;
            }

            _isNavigating = true;
            try
            {
                //await Shell.Current.GoToAsync($"{nameof(UserDetailsPage)}?UserId={user.Id}");
            }
            finally
            {
                _isNavigating = false;
            }
        }
    }
}