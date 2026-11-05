namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        public ProfileViewModel(UserService userService)
        {
            _userService = userService;
        }

        [ObservableProperty]
        public partial string DisplayName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool IsAdmin { get; set; }

        [RelayCommand]
        public async Task LoadProfileAsync()
        {
            var user = _userService.CurrentUser;

            if (user != null)
            {
                DisplayName = user.DisplayName;
                Email = user.Email;
                IsAdmin = user.IsAdmin;
            }
            else
            {
                DisplayName = "Guest";
                Email = "Not signed in";
                IsAdmin = false;

                // Comment out to disable redirect
                await Shell.Current.GoToAsync("//LandingPage");
            }
        }

        [RelayCommand]
        private async Task GoToUpdateDetailsAsync()
        {
            await NavigateAsync(nameof(UpdateProfilePage));
        }

        [RelayCommand]
        private async Task GoToMyWorkoutsAsync()
        {
            await NavigateAsync(nameof(MyWorkoutsPage));
        }

        [RelayCommand]
        private async Task GoToMyNotesAsync()
        {
            await NavigateAsync(nameof(MyNotesPage));
        }

        [RelayCommand]
        private async Task GoToAdminDashboardAsync()
        {
            await NavigateAsync(nameof(AdminDashboard));
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            _userService.Logout();
            await NavigateAsync("//LandingPage");
        }

        [RelayCommand]
        private async Task GoToPrivacyAsync()
        {
            await NavigateAsync(nameof(PrivacyPage));
        }
    }
}