namespace CapstoneProjectPUSH.ViewModels.Admin
{
    public partial class GymsManagementViewModel : BaseViewModel
    {
        private readonly GymService _gymService;

        public GymsManagementViewModel(GymService gymService, UserService userService) : base(userService)
        {
            _gymService = gymService;
        }

        private List<Gym> _allGyms = new();

        public ObservableCollection<Gym> Gyms { get; } = new();

        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            ErrorMessage = string.Empty;

            if (!await RequireAdminAsync())
            {
                return;
            }

            await RefreshAsync();
        }

        [RelayCommand]
        private async Task AddAsync()
        {
            await NavigateAsync(nameof(CreateGymPage));
        }

        [RelayCommand]
        private async Task EditAsync(Gym? gym)
        {
            if (gym is null)
            {
                return;
            }

            await NavigateAsync($"{nameof(CreateGymPage)}?GymId={gym.Id}");
        }

        [RelayCommand]
        private async Task DeleteAsync(Gym? gym)
        {
            if (gym is null)
            {
                return;
            }

            ErrorMessage = string.Empty;

            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete Gym",
                $"Delete {gym.NameAndCity}? This can't be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            try
            {
                var result = await _gymService.DeleteGymAsync(gym.Id);

                if (result.Success)
                {
                    await LoadCommand.ExecuteAsync(null);
                }
                else
                {
                    ErrorMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't delete the gym. Please try again.";
            }
        }

        private async Task RefreshAsync()
        {
            IsBusy = true;

            try
            {
                _allGyms = await _gymService.GetGymsAsync();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load gyms. Pull down to try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ApplyFilter()
        {
            string term = string.Empty;

            if (SearchText is not null)
            {
                term = SearchText.Trim();
            }

            List<Gym> filtered;

            if (term.Length == 0)
            {
                filtered = _allGyms;
            }
            else
            {
                filtered = _allGyms
                    .Where(g => g.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                             || g.City.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            Gyms.Clear();

            foreach (var gym in filtered)
            {
                Gyms.Add(gym);
            }

            if (filtered.Count == 1)
            {
                StatusMessage = "Showing 1 gym.";
            }
            else
            {
                StatusMessage = $"Showing {filtered.Count} gyms.";
            }
        }
    }
}