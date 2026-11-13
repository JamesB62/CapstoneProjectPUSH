namespace CapstoneProjectPUSH.ViewModels.Admin
{
    public partial class UserManagementViewModel : BaseViewModel
    {
        private List<User> _allUsers = new();

        public UserManagementViewModel(UserService userService) : base(userService)
        {

            SearchText = string.Empty;
            CountText = string.Empty;
        }

        public ObservableCollection<User> Users { get; } = new();

        [ObservableProperty]
        public partial string SearchText { get; set; }

        [ObservableProperty]
        public partial string CountText { get; set; }

        partial void OnSearchTextChanged(string value) => ApplyFilter();

        [RelayCommand]
        private async Task LoadAsync()
        {
            StatusMessage = string.Empty;
            await RefreshAsync();
        }

        // Deactivates an active account or reactivates a deactivated one
        [RelayCommand]
        private async Task ToggleActiveAsync(User? user)
        {
            if (user is null || !await EnsureAdminAsync())
            {
                return;
            }

            var activate = !user.IsActive;

            if (!activate)
            {
                var confirmed = await Shell.Current.DisplayAlertAsync(
                    "Deactivate account",
                    $"Deactivate {user.DisplayName}? They won't be able to sign in until the account is reactivated.",
                    "Deactivate",
                    "Cancel");

                if (!confirmed)
                {
                    return;
                }
            }

            try
            {
                var result = await _userService.SetUserActiveStatusAsync(user.Id, activate);
                StatusMessage = result.Message;

                if (result.Success)
                {
                    await RefreshAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't update that account.";
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

                _allUsers = await _userService.GetUsersAsync();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't load users. Pull down to try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ApplyFilter()
        {
            var term = (SearchText ?? string.Empty).Trim();

            var filtered = string.IsNullOrEmpty(term)
                ? _allUsers
                : _allUsers
                    .Where(u => u.DisplayName.Contains(term, StringComparison.OrdinalIgnoreCase)
                             || u.Email.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            Users.Clear();
            foreach (var user in filtered)
            {
                Users.Add(user);
            }

            CountText = filtered.Count == _allUsers.Count
                ? $"{_allUsers.Count} users"
                : $"{filtered.Count} of {_allUsers.Count} users";
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