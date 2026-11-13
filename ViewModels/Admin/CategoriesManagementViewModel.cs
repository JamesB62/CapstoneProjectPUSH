using System.Collections.ObjectModel;
using CapstoneProjectPUSH.Models;
using CapstoneProjectPUSH.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CapstoneProjectPUSH.ViewModels.Admin
{
    public partial class CategoriesManagementViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;

        // Id of the category being renamed, or null when adding a new one
        private int? _editingId;

        public CategoriesManagementViewModel(CategoryService categoryService, UserService userService) : base(userService)
        {
            _categoryService = categoryService;

            TitleInput = string.Empty;
            SaveButtonText = "Add category";
        }

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        public partial string TitleInput { get; set; }

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
                    ? await _categoryService.UpdateCategoryAsync(id, TitleInput)
                    : await _categoryService.AddCategoryAsync(TitleInput);

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
                StatusMessage = "Couldn't save the category.";
            }
        }

        [RelayCommand]
        private void Edit(Category? category)
        {
            if (category is null)
            {
                return;
            }

            _editingId = category.Id;
            TitleInput = category.Title;
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
        private async Task DeleteAsync(Category? category)
        {
            if (category is null || !await EnsureAdminAsync())
            {
                return;
            }

            var confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete category",
                $"Delete \"{category.Title}\"? This can't be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            try
            {
                var (success, message) = await _categoryService.DeleteCategoryAsync(category.Id);
                StatusMessage = message;

                if (!success)
                {
                    return;
                }

                if (_editingId == category.Id)
                {
                    ResetForm();
                }

                await RefreshAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't delete the category.";
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

                var categories = await _categoryService.GetCategoriesAsync();

                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't load categories. Pull down to try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ResetForm()
        {
            _editingId = null;
            TitleInput = string.Empty;
            SaveButtonText = "Add category";
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
