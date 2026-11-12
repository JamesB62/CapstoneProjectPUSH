using System.Collections.ObjectModel;
using CapstoneProjectPUSH.Models;
using CapstoneProjectPUSH.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CapstoneProjectPUSH.ViewModels.Admin
{
    public partial class GymsManagementViewModel : BaseViewModel
    {
        private readonly GymService _gymService;
        private readonly UserService _userService;

        // Id of the gym being edited, or null when adding a new one
        private int? _editingId;

        public GymsManagementViewModel(GymService gymService, UserService userService)
        {
            _gymService = gymService;
            _userService = userService;

            NameInput = string.Empty;
            AddressInput = string.Empty;
            CityInput = string.Empty;
            SaveButtonText = "Add gym";
        }

        public ObservableCollection<Gym> Gyms { get; } = new();

        [ObservableProperty]
        public partial string NameInput { get; set; }

        [ObservableProperty]
        public partial string AddressInput { get; set; }

        [ObservableProperty]
        public partial string CityInput { get; set; }

        [ObservableProperty]
        public partial string SaveButtonText { get; set; }

        [ObservableProperty]
        public partial bool IsEditing { get; set; }

        [RelayCommand]
        private async Task LoadAsync()
        {
            StatusMessage = string.Empty;
            await RefreshAsync();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (!await EnsureAdminAsync())
            {
                return;
            }

            try
            {
                var (success, message) = _editingId is int id
                    ? await _gymService.UpdateGymAsync(id, NameInput, AddressInput, CityInput)
                    : await _gymService.AddGymAsync(NameInput, AddressInput, CityInput);

                StatusMessage = message;

                if (!success)
                {
                    return;
                }

                ResetForm();
                await RefreshAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't save the gym.";
            }
        }

        [RelayCommand]
        private void Edit(Gym? gym)
        {
            if (gym is null)
            {
                return;
            }

            _editingId = gym.Id;
            NameInput = gym.Name;
            AddressInput = gym.Address;
            CityInput = gym.City;
            SaveButtonText = "Save changes";
            IsEditing = true;
            StatusMessage = string.Empty;
        }

        [RelayCommand]
        private void CancelEdit()
        {
            ResetForm();
            StatusMessage = string.Empty;
        }

        [RelayCommand]
        private async Task DeleteAsync(Gym? gym)
        {
            if (gym is null || !await EnsureAdminAsync())
            {
                return;
            }

            var confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete gym",
                $"Delete {gym.Name} ({gym.City})? This can't be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            try
            {
                var (success, message) = await _gymService.DeleteGymAsync(gym.Id);
                StatusMessage = message;

                if (!success)
                {
                    return;
                }

                if (_editingId == gym.Id)
                {
                    ResetForm();
                }

                await RefreshAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't delete the gym.";
            }
        }

        private async Task RefreshAsync()
        {
            IsBusy = true;

            try
            {
                if (!await EnsureAdminAsync())
                {
                    return;
                }

                var gyms = await _gymService.GetGymsAsync();

                Gyms.Clear();
                foreach (var gym in gyms)
                {
                    Gyms.Add(gym);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't load gyms. Pull down to try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ResetForm()
        {
            _editingId = null;
            NameInput = string.Empty;
            AddressInput = string.Empty;
            CityInput = string.Empty;
            SaveButtonText = "Add gym";
            IsEditing = false;
        }

        // Same access rules as the dashboard
        private async Task<bool> EnsureAdminAsync()
        {
            var user = _userService.CurrentUser;

            if (user is null)
            {
                await NavigateAsync("//LandingPage");
                return false;
            }

            if (!user.IsAdmin)
            {
                await Shell.Current.DisplayAlertAsync("Access denied", "This page is for administrators only.", "OK");
                await NavigateAsync("..");
                return false;
            }

            return true;
        }
    }
}
