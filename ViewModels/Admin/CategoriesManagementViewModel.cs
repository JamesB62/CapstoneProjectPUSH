namespace CapstoneProjectPUSH.ViewModels.Admin
{
    public partial class CategoriesManagementViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;

        public CategoriesManagementViewModel(CategoryService categoryService, UserService userService) : base(userService)
        {
            _categoryService = categoryService;
        }

        private List<Category> _allCategories = new();

        public ObservableCollection<Category> Categories { get; } = new();

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

        // Add a category through a popup
        [RelayCommand]
        private async Task AddAsync()
        {
            ErrorMessage = string.Empty;

            var title = await PromptForTitleAsync("Add Category", "Add", string.Empty);

            if (title is null)
            {
                return;
            }

            try
            {
                var result = await _categoryService.AddCategoryAsync(title);

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
                ErrorMessage = "Couldn't add the category. Please try again.";
            }
        }

        // Rename a category through a popup
        [RelayCommand]
        private async Task EditAsync(Category? category)
        {
            if (category is null)
            {
                return;
            }

            ErrorMessage = string.Empty;

            var title = await PromptForTitleAsync("Edit Category", "Save", category.Title);

            if (title is null || title.Trim() == category.Title)
            {
                return;
            }

            try
            {
                var result = await _categoryService.UpdateCategoryAsync(category.Id, title);

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
                ErrorMessage = "Couldn't update the category. Please try again.";
            }
        }

        [RelayCommand]
        private async Task DeleteAsync(Category? category)
        {
            if (category is null)
            {
                return;
            }

            ErrorMessage = string.Empty;

            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete Category",
                $"Delete \"{category.Title}\"? This can't be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            try
            {
                var result = await _categoryService.DeleteCategoryAsync(category.Id);

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
                ErrorMessage = "Couldn't delete the category. Please try again.";
            }
        }
        private async Task RefreshAsync()
        {
            IsBusy = true;

            try
            {
                _allCategories = await _categoryService.GetCategoriesAsync();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load categories. Pull down to try again.";
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

            List<Category> filtered;

            if (term.Length == 0)
            {
                filtered = _allCategories;
            }
            else
            {
                filtered = _allCategories
                    .Where(c => c.Title.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            Categories.Clear();

            foreach (var category in filtered)
            {
                Categories.Add(category);
            }

            if (filtered.Count == 1)
            {
                StatusMessage = "Showing 1 category.";
            }
            else
            {
                StatusMessage = $"Showing {filtered.Count} categories.";
            }
        }

        // Shared popup for add and edit
        private static async Task<string?> PromptForTitleAsync(string title, string accept, string initialValue)
        {
            return await Shell.Current.DisplayPromptAsync(
                title,
                "Category title (2 to 50 characters)",
                accept: accept,
                cancel: "Cancel",
                placeholder: "Enter category title here...",
                maxLength: 50,
                initialValue: initialValue);
        }
    }
}