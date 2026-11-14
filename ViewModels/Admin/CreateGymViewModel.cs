namespace CapstoneProjectPUSH.ViewModels.Admin
{
    [QueryProperty(nameof(GymId), "GymId")]
    public partial class CreateGymViewModel : BaseViewModel
    {
        private readonly GymService _gymService;

        public CreateGymViewModel(GymService gymService, UserService userService) : base(userService)
        {
            _gymService = gymService;
        }

        private bool _hasLoadedGym;

        [ObservableProperty]
        public partial int GymId { get; set; }

        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Add PUSH Gym";

        [ObservableProperty]
        public partial string SaveButtonText { get; set; } = "Save Gym";

        [ObservableProperty]
        public partial string Name { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Address { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string City { get; set; } = string.Empty;

        partial void OnGymIdChanged(int value)
        {
            if (value > 0)
            {
                PageTitle = "Edit PUSH Gym";
                SaveButtonText = "Save Changes";
            }
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            ErrorMessage = string.Empty;

            if (!await RequireAdminAsync())
            {
                return;
            }

            if (GymId <= 0 || _hasLoadedGym)
            {
                return;
            }

            try
            {
                var gym = await _gymService.GetGymByIdAsync(GymId);

                if (gym is null)
                {
                    await Shell.Current.DisplayAlertAsync(
                        "Gym Not Found",
                        "That gym no longer exists.",
                        "OK");

                    await NavigateAsync("..");
                    return;
                }

                Name = gym.Name;
                Address = gym.Address;
                City = gym.City;

                _hasLoadedGym = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load the gym. Please try again.";
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                bool isEditing = GymId > 0;

                (bool Success, string Message) result;

                if (isEditing)
                {
                    result = await _gymService.UpdateGymAsync(GymId, Name, Address, City);
                }
                else
                {
                    result = await _gymService.AddGymAsync(Name, Address, City);
                }

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                string gymLabel = $"{Name.Trim()} ({City.Trim()})";

                if (isEditing)
                {
                    await Shell.Current.DisplayAlertAsync("Gym Updated", $"{gymLabel} has been updated.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Gym Created", $"{gymLabel} has been created.", "OK");
                }

                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't save the gym. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await NavigateAsync("..");
        }
    }
}