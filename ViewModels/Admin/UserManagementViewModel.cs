namespace CapstoneProjectPUSH.ViewModels.Admin
{
    public partial class UserManagementViewModel : BaseViewModel
    {
        public UserManagementViewModel(UserService userService) : base(userService)
        {
        }

        private List<User> _allUsers = new();

        public ObservableCollection<User> Users { get; } = new();

        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool ShowActiveOnly { get; set; }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        partial void OnShowActiveOnlyChanged(bool value)
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

        // Deactivates an active account or reactivates a deactivated one
        [RelayCommand]
        private async Task ToggleActiveAsync(User? user)
        {
            if (user is null)
            {
                return;
            }

            ErrorMessage = string.Empty;

            var activate = !user.IsActive;

            if (!activate)
            {
                var confirmed = await Shell.Current.DisplayAlertAsync(
                    "Deactivate Account",
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
                ErrorMessage = "Couldn't update that account. Please try again.";
            }
        }

        private async Task RefreshAsync()
        {
            IsBusy = true;

            try
            {
                _allUsers = await _userService.GetUsersAsync();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load users. Pull down to try again.";
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

            List<User> filtered;

            if (term.Length == 0)
            {
                filtered = _allUsers;
            }
            else
            {
                filtered = _allUsers
                    .Where(u => u.DisplayName.Contains(term, StringComparison.OrdinalIgnoreCase)
                             || u.Email.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (ShowActiveOnly)
            {
                filtered = filtered.Where(u => u.IsActive).ToList();
            }

            Users.Clear();

            foreach (var user in filtered)
            {
                Users.Add(user);
            }

            if (filtered.Count == 1)
            {
                StatusMessage = "Showing 1 user.";
            }
            else
            {
                StatusMessage = $"Showing {filtered.Count} users.";
            }
        }
    }
}