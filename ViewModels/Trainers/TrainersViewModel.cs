namespace CapstoneProjectPUSH.ViewModels.Trainers
{
    public partial class TrainersViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private readonly TrainerService _trainerService;


        public TrainersViewModel(CategoryService categoryService, TrainerService trainerService, UserService userService) : base(userService)
        {
            _categoryService = categoryService;
            _trainerService = trainerService;

        }

        private const string PlaceholderImage = "profile_picture_placeholder.png";

        public ObservableCollection<Trainer> Trainers { get; } = new();

        private List<Trainer> _allTrainers = new();

        private static readonly Category AllCategory = new() { Id = 0, Title = "All" };

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        [ObservableProperty]
        public partial Trainer? SelectedTrainer { get; set; }

        [ObservableProperty]
        public partial bool ShowAvailableOnly { get; set; }

        // True when the signed-in user is an admin (lets the page show admin controls)
        [ObservableProperty]
        public partial bool IsAdmin { get; set; }

        [RelayCommand]
        private async Task LoadTrainersAsync()
        {
            IsBusy = true;

            try
            {
                var user = _userService.CurrentUser;

                if (user is null)
                {
                    // Comment out to disable redirect
                    await NavigateAsync("//LandingPage");
                    return;
                }

                IsAdmin = user.IsAdmin;

                StatusMessage = "Loading trainers...";

                if (Categories.Count == 0)
                {
                    var categories = await _categoryService.GetCategoriesAsync();

                    var sortedCategories = categories.OrderBy(c => c.Title, StringComparer.OrdinalIgnoreCase).ToList();

                    Categories.Clear();
                    Categories.Add(AllCategory);

                    foreach (var category in sortedCategories)
                    {
                        Categories.Add(category);
                    }

                    SelectedCategory = AllCategory;
                }

                var trainers = await _trainerService.GetTrainersFavouriteGymFirstAsync(user.GymId);

                foreach (var trainer in trainers)
                {
                    trainer.ProfileImage = GetProfileImageOrPlaceholder(trainer.ProfileImage);
                }

                _allTrainers = trainers.ToList();

                ApplyFilter();
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

        partial void OnSelectedCategoryChanged(Category? value)
        {
            ApplyFilter();
        }

        partial void OnShowAvailableOnlyChanged(bool value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            int categoryId;

            if (SelectedCategory is null)
            {
                categoryId = 0;
            }
            else
            {
                categoryId = SelectedCategory.Id;
            }

            List<Trainer> filtered;

            if (categoryId == 0)
            {
                filtered = _allTrainers;
            }

            else
            {
                filtered = _allTrainers.Where(t => t.CategoryId == categoryId).ToList();
            }

            if (ShowAvailableOnly)
            {
                filtered = filtered.Where(t => t.Available).ToList();
            }

            Trainers.Clear();
            foreach (var trainer in filtered)
            {
                Trainers.Add(trainer);
            }

            if (Trainers.Count == 1)
            {
                StatusMessage = "Showing 1 trainer.";
            }
            else
            {
                StatusMessage = $"Showing {Trainers.Count} trainers.";
            }


        }

        partial void OnSelectedTrainerChanged(Trainer? value)
        {
            if (value is null)
            {
                return;
            }

            if (_userService.CurrentUser is { IsAdmin: true })
            {
                ManageTrainerCommand.Execute(value);
            }
            else
            {
                ViewTrainerCommand.Execute(value);
            }

            SelectedTrainer = null;
        }

        [RelayCommand]
        private async Task ViewTrainerAsync(Trainer? trainer)
        {
            if (trainer is null)
            {
                return;
            }

            await NavigateAsync($"{nameof(TrainerDetailsPage)}?TrainerId={trainer.Id}");
        }

        [RelayCommand]
        private async Task AddTrainerAsync()
        {
            if (!RequireAdmin())
            {
                return;
            }

            await NavigateAsync(nameof(CreateTrainerPage));
        }

        // Admin menu shown when an admin taps a trainer
        [RelayCommand]
        private async Task ManageTrainerAsync(Trainer? trainer)
        {
            if (trainer is null)
            {
                return;
            }

            if (!RequireAdmin())
            {
                return;
            }

            const string viewDetails = "View details";
            const string editDetails = "Edit details";
            const string deleteTrainer = "Delete trainer";
            var toggleAvailability = trainer.Available ? "Mark as unavailable" : "Mark as available";

            var choice = await Shell.Current.DisplayActionSheetAsync(
                trainer.FullName,
                "Cancel",
                deleteTrainer,
                viewDetails,
                editDetails,
                toggleAvailability);

            if (choice == viewDetails)
            {
                await ViewTrainerAsync(trainer);
            }
            else if (choice == editDetails)
            {
                await EditTrainerAsync(trainer);
            }
            else if (choice == toggleAvailability)
            {
                await ToggleTrainerAvailabilityAsync(trainer);
            }
            else if (choice == deleteTrainer)
            {
                await DeleteTrainerAsync(trainer);
            }
        }

        // Opens the create trainer page in edit mode
        [RelayCommand]
        private async Task EditTrainerAsync(Trainer? trainer)
        {
            if (trainer is null || !RequireAdmin())
            {
                return;
            }

            await NavigateAsync($"{nameof(CreateTrainerPage)}?TrainerId={trainer.Id}");
        }

        // Deactivates (unavailable) or reactivates (available) a trainer
        [RelayCommand]
        private async Task ToggleTrainerAvailabilityAsync(Trainer? trainer)
        {
            if (trainer is null || !RequireAdmin())
            {
                return;
            }

            try
            {
                var result = await _trainerService.SetTrainerAvailabilityAsync(trainer.Id, !trainer.Available);

                if (result.Success)
                {
                    await LoadTrainersAsync();
                }

                StatusMessage = result.Message;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't update the trainer.";
            }
        }

        [RelayCommand]
        private async Task DeleteTrainerAsync(Trainer? trainer)
        {
            if (trainer is null || !RequireAdmin())
            {
                return;
            }

            var confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete trainer",
                $"Delete {trainer.FullName}? This can't be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            try
            {
                var result = await _trainerService.DeleteTrainerAsync(trainer.Id);

                if (result.Success)
                {
                    await LoadTrainersAsync();
                }

                StatusMessage = result.Message;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't delete the trainer.";
            }
        }

        // Admin check for the actions above
        private bool RequireAdmin()
        {
            if (_userService.CurrentUser is { IsAdmin: true })
            {
                return true;
            }

            StatusMessage = "Only administrators can do that.";
            return false;
        }
    }
}