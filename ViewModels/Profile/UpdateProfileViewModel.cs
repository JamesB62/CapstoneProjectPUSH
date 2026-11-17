namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class UpdateProfileViewModel : BaseViewModel
    {
        private string _savedProfileImage = string.Empty;
        private bool _hasLoadedProfile;

        private readonly GymService _gymService;
        private readonly ImageService _imageService;

        public UpdateProfileViewModel(GymService gymService, ImageService imageService, UserService userService) : base(userService)
        {
            _gymService = gymService;
            _imageService = imageService;
        }

        [ObservableProperty]
        public partial string DisplayName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string CurrentPassword { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string NewPassword { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string ConfirmPassword { get; set; } = string.Empty;

        [ObservableProperty]
        public partial ObservableCollection<Gym> Gyms { get; set; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedGym))]
        public partial Gym? SelectedGym { get; set; }

        public bool HasSelectedGym => SelectedGym is not null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasProfileImage))]
        [NotifyPropertyChangedFor(nameof(PhotoStatusText))]
        public partial string ProfileImage { get; set; } = string.Empty;

        public bool HasProfileImage => !string.IsNullOrWhiteSpace(ProfileImage);

        public string PhotoStatusText
        {
            get
            {
                if (HasProfileImage)
                {
                    return "Photo selected";
                }

                return "No photo";
            }
        }

        public string PasswordRequirements => UserService.PasswordRequirementsText;

        [RelayCommand]
        private async Task LoadProfileAsync()
        {
            var user = _userService.CurrentUser;

            if (user is null || user.IsGuest)
            {
                await NavigateAsync("..");
                return;
            }

            if (_hasLoadedProfile)
            {
                return;
            }

            DisplayName = user.DisplayName;
            Email = user.Email;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            ErrorMessage = string.Empty;

            if (user.ProfileImage is null)
            {
                _savedProfileImage = string.Empty;
            }
            else
            {
                _savedProfileImage = user.ProfileImage;
            }

            ProfileImage = _savedProfileImage;

            await LoadGymsAsync(user.GymId);

            _hasLoadedProfile = true;
        }

        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                int? gymId = null;

                if (SelectedGym is not null)
                {
                    gymId = SelectedGym.Id;
                }

                var result = await _userService.UpdateDetailsAsync(DisplayName, Email, CurrentPassword, NewPassword, ConfirmPassword, gymId, ProfileImage);

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                _savedProfileImage = ProfileImage;

                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;

                IsBusy = false;

                await Shell.Current.DisplayAlertAsync("Details Updated", "Your PUSH account details have been updated.", "OK");
                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Something went wrong. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelProfileAsync()
        {
            if (IsBusy)
            {
                return;
            }

            DiscardUnsavedPhoto();

            await NavigateAsync("..");
        }

        [RelayCommand]
        private void ClearGym()
        {
            SelectedGym = null;
        }

        private async Task LoadGymsAsync(int? gymId)
        {
            try
            {
                var gyms = await _gymService.GetGymsAsync();

                Gym? favourite = null;

                if (gymId is not null)
                {
                    favourite = gyms.FirstOrDefault(g => g.Id == gymId.Value);
                }

                Gyms = new ObservableCollection<Gym>(gyms);
                SelectedGym = favourite;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't load gyms. Please try again.";
            }
        }

        [RelayCommand]
        private async Task ChangePhotoAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;

            try
            {
                string? newImage = await _imageService.PickImageAsync();

                if (newImage is null)
                {
                    return;
                }

                DiscardUnsavedPhoto();
                ProfileImage = newImage;
            }
            catch (PermissionException)
            {
                ErrorMessage = "PUSH needs permission to access your photos.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load that photo. Please try another.";
            }
        }

        [RelayCommand]
        private void RemovePhoto()
        {
            DiscardUnsavedPhoto();
            ProfileImage = string.Empty;
        }

        // Deletes a picked photo that hasn't been saved to the account
        private void DiscardUnsavedPhoto()
        {
            if (ProfileImage != _savedProfileImage)
            {
                ImageService.DeleteImage(ProfileImage);
            }
        }
    }
}