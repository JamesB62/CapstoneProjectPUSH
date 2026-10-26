using CapstoneProjectPUSH.Views.Base;

namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        [ObservableProperty]
        public partial string DisplayName { get; set; }

        public ProfileViewModel(UserService userService)
        {
            _userService = userService;

            if (_userService.CurrentUser != null)
            {
                DisplayName = _userService.CurrentUser.DisplayName;
            }
            else
            {
                DisplayName = "Username";
            }
        }

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
            await Shell.Current.DisplayAlertAsync("Coming soon", "Admin isn't built yet.", "OK");
        }

        // Generates LogoutCommand
        [RelayCommand]
        private async Task LogoutAsync()
        {
            _userService.Logout();
            await Shell.Current.GoToAsync("//MainTabs/Base/LandingPage");
        }

        // Generates GoToPrivacyCommand
        [RelayCommand]
        private async Task GoToPrivacyAsync()
        {
            await Shell.Current.Navigation.PushAsync(new PrivacyPage());
        }
    }
}
