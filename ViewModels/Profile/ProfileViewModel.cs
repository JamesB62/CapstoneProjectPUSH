namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class ProfileViewModel : BaseViewModel
    {


        public ProfileViewModel(UserService userService) : base(userService)
        {

        }

        private const string PlaceholderImage = "profile_picture_placeholder.png";

        [ObservableProperty]
        public partial string DisplayName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string ProfileImage { get; set; } = PlaceholderImage;

        [ObservableProperty]
        public partial bool IsAdmin { get; set; }

        [RelayCommand]
        private async Task LoadProfileAsync()
        {
            var user = _userService.CurrentUser;

            if (user != null)
            {
                DisplayName = user.DisplayName;
                Email = user.Email;
                IsAdmin = user.IsAdmin;
                ProfileImage = GetProfileImageOrPlaceholder(user.ProfileImage);
            }
            else
            {
                DisplayName = "Guest";
                Email = "Not signed in";
                IsAdmin = false;
                ProfileImage = PlaceholderImage;

                // Comment out to disable redirect
                await NavigateAsync("//LandingPage");
            }
        }

        private static string GetProfileImageOrPlaceholder(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return PlaceholderImage;
            }

            if (!File.Exists(imagePath))
            {
                return PlaceholderImage;
            }

            return imagePath;
        }

        [RelayCommand]
        private async Task GoToUpdateDetailsAsync()
        {
            if (!await RequireLoginAsync())
            return;

            await NavigateAsync(nameof(UpdateProfilePage));
        }

        [RelayCommand]
        private async Task GoToMyWorkoutsAsync()
        {
            if (!await RequireLoginAsync())
            return;

            await NavigateAsync(nameof(MyWorkoutsPage));
        }

        [RelayCommand]
        private async Task GoToMyNotesAsync()
        {
            if (!await RequireLoginAsync())
            return;

            await NavigateAsync(nameof(MyNotesPage));
        }

        [RelayCommand]
        private async Task GoToAdminDashboardAsync()
        {
            if (!await RequireLoginAsync())
            return;

            await NavigateAsync(nameof(AdminDashboardPage));
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {

            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Logout",
                "Are you sure you want to log out?",
                "Logout",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

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