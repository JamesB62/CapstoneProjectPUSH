namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private const string PlaceholderImage = "profile_picture_placeholder.png";

        private readonly GymService _gymService;

        public ProfileViewModel(GymService gymService, UserService userService) : base(userService)
        {
            _gymService = gymService;
        }

        [ObservableProperty]
        public partial string DisplayName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string ProfileImage { get; set; } = PlaceholderImage;

        [ObservableProperty]
        public partial bool IsAdmin { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasFavouriteGym))]
        public partial string FavouriteGymText { get; set; } = string.Empty;

        public bool HasFavouriteGym => !string.IsNullOrEmpty(FavouriteGymText);

        [ObservableProperty]
        private bool canDeleteAccount;

        [RelayCommand]
        private async Task LoadProfileAsync()
        {
            RefreshUserState();
            StatusMessage = string.Empty;

            var user = _userService.CurrentUser;

            if (user is null)
            {
                return;
            }

            DisplayName = user.DisplayName;
            Email = user.Email;
            IsAdmin = user.IsAdmin;
            ProfileImage = GetProfileImageOrPlaceholder(user.ProfileImage);

            if (IsLoggedIn && !IsCurrentUserAdmin)
            {
                CanDeleteAccount = true;
            }
            else
            {
                CanDeleteAccount = false;
            }

            await LoadFavouriteGymAsync(user.GymId);
        }

        private async Task LoadFavouriteGymAsync(int? gymId)
        {
            FavouriteGymText = string.Empty;

            if (gymId is null)
            {
                return;
            }

            try
            {
                var gym = await _gymService.GetGymByIdAsync(gymId.Value);

                if (gym is not null)
                {
                    FavouriteGymText = gym.NameAndCity;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private static string GetProfileImageOrPlaceholder(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return PlaceholderImage;
            }

            // Uploaded photo -- a web address from the api, so there's no file to check
            if (imagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || imagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return imagePath;
            }

            if (!File.Exists(imagePath))
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
            {
                return;
            }

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
            if (!await RequireLoginAsync())
            {
                return;
            }

            if (!IsAdmin)
            {
                return;
            }

            await NavigateAsync(nameof(AdminDashboardPage));
        }

        [RelayCommand]
        private async Task DeleteAccountAsync()
        {
            if (IsBusy)
            {
                return;
            }

            var confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete Account?",
                "This will permanently delete your account, workouts, notes and stats. This can't be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;

            AuthResult result;

            try
            {
                result = await _userService.DeleteAccountAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Your account couldn't be deleted. Please try again.";
                return;
            }
            finally
            {
                IsBusy = false;
            }

            if (!result.Success)
            {
                ErrorMessage = result.Message;
                return;
            }

            await Shell.Current.DisplayAlertAsync("Account Deleted", "Your account and all your data has been deleted.", "OK");
            await LogoutAndRedirectAsync();
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

            await LogoutAndRedirectAsync();
        }

        [RelayCommand]
        private async Task GoToPrivacyAsync()
        {
            await NavigateAsync(nameof(PrivacyPage));
        }
    }
}