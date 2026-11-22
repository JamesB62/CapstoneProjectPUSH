namespace CapstoneProjectPUSH.ViewModels.Trainers
{
    public partial class TrainersViewModel : BaseViewModel
    {
        private readonly TrainerService _trainerService;

        public TrainersViewModel(TrainerService trainerService, UserService userService) : base(userService)
        {
            _trainerService = trainerService;
        }

        private const string PlaceholderImage = "profile_picture_placeholder.png";

        public ObservableCollection<Trainer> Trainers { get; } = new();

        private List<Trainer> _allTrainers = new();

        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial Trainer? SelectedTrainer { get; set; }

        [ObservableProperty]
        public partial bool ShowAvailableOnly { get; set; }

        [RelayCommand]
        private async Task LoadTrainersAsync()
        {
            ErrorMessage = string.Empty;
            RefreshUserState();

            var user = _userService.CurrentUser;

            // AppShell sends signed-out users to the landing page
            if (user is null)
            {
                return;
            }

            IsBusy = true;

            try
            {
                var trainers = await _trainerService.GetTrainersFavouriteGymFirstAsync(user.GymId);

                foreach (var trainer in trainers)
                {
                    trainer.ProfileImage = GetProfileImageOrPlaceholder(trainer.ProfileImage);
                }

                _allTrainers = trainers;

                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load trainers. Pull down to try again.";
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

            // Uploaded photo -- a web address from the api, so there's no file to check
            if (imagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || imagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return imagePath;
            }

            if (!File.Exists(imagePath))
            {
                return PlaceholderImage;
            }

            return imagePath;
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        partial void OnShowAvailableOnlyChanged(bool value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string term = string.Empty;

            if (SearchText is not null)
            {
                term = SearchText.Trim();
            }

            List<Trainer> filtered;

            if (term.Length == 0)
            {
                filtered = _allTrainers;
            }
            else
            {
                filtered = _allTrainers
                    .Where(t => t.FullName.Contains(term, StringComparison.OrdinalIgnoreCase)
                             || t.CategoryTitle.Contains(term, StringComparison.OrdinalIgnoreCase)
                             || t.GymName.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .ToList();
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

            if (filtered.Count == 1)
            {
                StatusMessage = "Showing 1 trainer.";
            }
            else
            {
                StatusMessage = $"Showing {filtered.Count} trainers.";
            }
        }

        partial void OnSelectedTrainerChanged(Trainer? value)
        {
            if (value is null)
            {
                return;
            }

            ViewTrainerCommand.Execute(value);
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
            await NavigateAsync(nameof(CreateTrainerPage));
        }

        // Opens the create trainer page in edit mode
        [RelayCommand]
        private async Task EditTrainerAsync(Trainer? trainer)
        {
            if (trainer is null || !IsCurrentUserAdmin)
            {
                return;
            }

            await NavigateAsync($"{nameof(CreateTrainerPage)}?TrainerId={trainer.Id}");
        }

        [RelayCommand]
        private async Task DeleteTrainerAsync(Trainer? trainer)
        {
            if (trainer is null || !IsCurrentUserAdmin)
            {
                return;
            }

            ErrorMessage = string.Empty;

            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete Trainer",
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
                    await LoadTrainersCommand.ExecuteAsync(null);
                }
                else
                {
                    ErrorMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't delete the trainer. Please try again.";
            }
        }
    }
}