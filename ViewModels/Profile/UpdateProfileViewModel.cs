namespace CapstoneProjectPUSH.ViewModels.Profile
{
    public partial class UpdateProfileViewModel : BaseViewModel
    {
        private readonly GymService _gymService;

        public UpdateProfileViewModel(GymService gymService, UserService userService) : base(userService)
        {
            _gymService = gymService;
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

            DisplayName = user.DisplayName;
            Email = user.Email;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            StatusMessage = string.Empty;

            await LoadGymsAsync(user.GymId);
        }

        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            StatusMessage = string.Empty;

            try
            {
                int? gymId = null;

                if (SelectedGym is not null)
                {
                    gymId = SelectedGym.Id;
                }

                var result = await _userService.UpdateDetailsAsync(DisplayName, Email, CurrentPassword, NewPassword, ConfirmPassword, gymId);

                if (!result.Success)
                {
                    StatusMessage = result.Message;
                    return;
                }

                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;

                IsBusy = false;

                await Shell.Current.DisplayAlertAsync("Update Successful", "Your PUSH account details have been updated.", "OK");
                await NavigateAsync("..");
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

        [RelayCommand]
        private async Task CancelProfileAsync()
        {
            if (IsBusy)
            {
                return;
            }

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
                StatusMessage = "Couldn't load gyms.";
            }
        }
    }
}