namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        public ProfileViewModel(UserService userService)
        {
            _userService = userService;
            LoadProfile();
        }

        public void LoadProfile()
        {
            var user = _userService.CurrentUser;

            if (user is null)
            {
                DisplayName = "Username";
                Email = "Email Address";
            }
            else
            {
                DisplayName = user.DisplayName;
                Email = user.Email;
            }
        }

        [ObservableProperty]
        public partial string DisplayName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        // Generates GoToUpdateDetailsCommand
        [RelayCommand]
        private async Task GoToUpdateDetailsAsync()
        {
            await Shell.Current.GoToAsync(nameof(UpdateProfilePage));
        }

        // Generates MyWorkoutsCommand
        [RelayCommand]
        private async Task MyWorkoutsAsync()
        {
            await Shell.Current.DisplayAlertAsync("Coming soon", "My Workouts isn't built yet.", "OK");
        }

        // Generates MyNotesCommand
        [RelayCommand]
        private async Task MyNotesAsync()
        {
            await Shell.Current.DisplayAlertAsync("Coming soon", "My Notes isn't built yet.", "OK");
        }

        // Generates AdminCommand
        [RelayCommand]
        private async Task AdminAsync()
        {
            await Shell.Current.DisplayAlertAsync("Coming soon", "Admin isn't built yet.", "OK");
        }

        // Generates LogoutCommand
        [RelayCommand]
        private async Task LogoutAsync()
        {
            _userService.Logout();
            await Shell.Current.GoToAsync("//LandingPage");
            DisplayName = "Username";
        }

        // Generates GoToPrivacyCommand
        [RelayCommand]
        private async Task GoToPrivacyAsync()
        {
            await Shell.Current.Navigation.PushAsync(new PrivacyPage());
        }
    }
}