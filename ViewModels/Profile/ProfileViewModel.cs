using CapstoneProjectPUSH.Views.Admin;

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
            }
            else
            {
                DisplayName = user.DisplayName;
            }
        }

        [ObservableProperty]
        public partial string DisplayName { get; set; }

        // Generates UpdateDetailsCommand
        [RelayCommand]
        private async Task UpdateDetailsAsync()
        {
            await Shell.Current.DisplayAlertAsync("Coming soon", "Update Details isn't built yet.", "OK");
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
            await Shell.Current.Navigation.PushAsync(new AdminDashboard());
        }

        // Generates LogoutCommand
        [RelayCommand]
        private async Task LogoutAsync()
        {
            _userService.Logout();
            DisplayName = "Username";
            await Shell.Current.GoToAsync("//LoginPage");
        }

        // Generates GoToPrivacyCommand
        [RelayCommand]
        private async Task GoToPrivacyAsync()
        {
            await Shell.Current.Navigation.PushAsync(new PrivacyPage());
        }
    }
}