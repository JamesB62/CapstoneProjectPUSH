using System.Collections.ObjectModel;
using CapstoneProjectPUSH.Models;
using CapstoneProjectPUSH.Services;

namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly GymService _gymService;

        // Gyms the user hasn't linked yet, in the same order as GymChoices (the picker items)
        private List<Gym> _availableGyms = new();

        public ProfileViewModel(GymService gymService, UserService userService) : base(userService)
        {
            _gymService = gymService;

            MyGyms.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasNoGyms));
            GymChoices.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasGymChoices));
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

        // Gyms the user has linked
        public ObservableCollection<Gym> MyGyms { get; } = new();

        // Picker items: the gyms the user hasn't linked yet
        public ObservableCollection<string> GymChoices { get; } = new();

        [ObservableProperty]
        public partial int SelectedGymIndex { get; set; } = -1;

        public bool HasNoGyms => MyGyms.Count == 0;

        public bool HasGymChoices => GymChoices.Count > 0;

        [RelayCommand]
        private async Task LoadProfileAsync()
        {
            StatusMessage = string.Empty;

            var user = _userService.CurrentUser;

            if (user != null)
            {
                DisplayName = user.DisplayName;
                Email = user.Email;
                IsAdmin = user.IsAdmin;
                ProfileImage = GetProfileImageOrPlaceholder(user.ProfileImage);

                await LoadGymsAsync(user.Id);
            }
            else
            {
                DisplayName = "Guest";
                Email = "Not signed in";
                IsAdmin = false;
                ProfileImage = PlaceholderImage;

                _availableGyms.Clear();
                MyGyms.Clear();
                GymChoices.Clear();

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

        // Link the gym chosen in the picker to the user's account
        [RelayCommand]
        private async Task AddGymAsync()
        {
            var user = _userService.CurrentUser;

            if (user is null)
            {
                await NavigateAsync("//LandingPage");
                return;
            }

            if (SelectedGymIndex < 0 || SelectedGymIndex >= _availableGyms.Count)
            {
                StatusMessage = "Choose a gym to add first.";
                return;
            }

            var gym = _availableGyms[SelectedGymIndex];

            try
            {
                var result = await _gymService.LinkGymAsync(user.Id, gym.Id);
                StatusMessage = result.Message;

                if (result.Success)
                {
                    await LoadGymsAsync(user.Id);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't add that gym. Please try again.";
            }
        }

        // Remove a gym from the user's account
        [RelayCommand]
        private async Task RemoveGymAsync(Gym? gym)
        {
            var user = _userService.CurrentUser;

            if (gym is null || user is null)
            {
                return;
            }

            try
            {
                var result = await _gymService.UnlinkGymAsync(user.Id, gym.Id);
                StatusMessage = result.Message;

                if (result.Success)
                {
                    await LoadGymsAsync(user.Id);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't remove that gym. Please try again.";
            }
        }

        // Linked gyms go in the list, every other gym goes in the picker
        private async Task LoadGymsAsync(int userId)
        {
            try
            {
                // Linked gyms come first, but every gym is returned
                var gyms = await _gymService.GetGymsForUserAsync(userId);

                _availableGyms = gyms.Where(g => !g.IsLinked).ToList();

                MyGyms.Clear();
                foreach (var gym in gyms.Where(g => g.IsLinked))
                {
                    MyGyms.Add(gym);
                }

                GymChoices.Clear();
                foreach (var gym in _availableGyms)
                {
                    GymChoices.Add($"{gym.Name} ({gym.City})");
                }

                SelectedGymIndex = -1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't load your gyms.";
            }
        }
    }
}