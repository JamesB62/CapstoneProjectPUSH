namespace CapstoneProjectPUSH.ViewModels.Trainers
{
    [QueryProperty(nameof(TrainerId), "TrainerId")]
    public partial class CreateTrainerViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private readonly GymService _gymService;
        private readonly ImageService _imageService;
        private readonly TrainerService _trainerService;

        public CreateTrainerViewModel(CategoryService categoryService, GymService gymService, ImageService imageService, TrainerService trainerService, UserService userService) : base(userService)
        {
            _categoryService = categoryService;
            _gymService = gymService;
            _imageService = imageService;
            _trainerService = trainerService;
        }

        private string _savedProfileImage = string.Empty;

        // Set when the page is opened to edit an existing trainer
        private bool _hasLoadedTrainer;

        // Set when opened to edit a trainer (0 means add a new trainer)
        [ObservableProperty]
        public partial int TrainerId { get; set; }

        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Create PUSH Trainer";

        [ObservableProperty]
        public partial string SaveButtonText { get; set; } = "Save Trainer";

        // Picker items
        public ObservableCollection<Category> Categories { get; } = new();

        public ObservableCollection<Gym> Gyms { get; } = new();

        [ObservableProperty]
        public partial string FirstName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string LastName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string PhoneNumber { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Description { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool Available { get; set; } = true;

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        [ObservableProperty]
        public partial Gym? SelectedGym { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasProfileImage))]
        [NotifyPropertyChangedFor(nameof(PhotoStatusText))]
        public partial string ProfileImage { get; set; } = string.Empty;

        public bool HasProfileImage => !string.IsNullOrWhiteSpace(ProfileImage);

        public string PhotoStatusText
        {
            get
            {
                if (HasProfileImage)
                {
                    return "Photo selected";
                }

                return "No photo";
            }
        }

        // Switch the page into edit mode when a trainer Id arrives
        partial void OnTrainerIdChanged(int value)
        {
            if (value > 0)
            {
                PageTitle = "Edit PUSH Trainer";
                SaveButtonText = "Save Changes";

                LoadCommand.Execute(null);
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

            try
            {
                // Picker items only need loading once per visit
                if (Categories.Count == 0)
                {
                    var categories = await _categoryService.GetCategoriesAsync();

                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }
                }

                if (Gyms.Count == 0)
                {
                    var gyms = await _gymService.GetGymsAsync();

                    foreach (var gym in gyms)
                    {
                        Gyms.Add(gym);
                    }
                }

                // Add mode, or already filled in
                if (TrainerId <= 0 || _hasLoadedTrainer)
                {
                    return;
                }

                var trainer = await _trainerService.GetTrainerByIdAsync(TrainerId);

                if (trainer is null)
                {
                    await Shell.Current.DisplayAlertAsync(
                        "Trainer Not Found",
                        "That trainer no longer exists.",
                        "OK");

                    await NavigateAsync("..");
                    return;
                }

                FirstName = trainer.FirstName;
                LastName = trainer.LastName;
                Email = trainer.Email;
                PhoneNumber = trainer.PhoneNumber;
                Description = trainer.Description;
                Available = trainer.Available;
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == trainer.CategoryId);
                SelectedGym = Gyms.FirstOrDefault(g => g.Id == trainer.GymId);

                if (trainer.ProfileImage is null)
                {
                    _savedProfileImage = string.Empty;
                }
                else
                {
                    _savedProfileImage = trainer.ProfileImage;
                }

                ProfileImage = _savedProfileImage;

                _hasLoadedTrainer = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load the form. Please try again.";
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

            if (SelectedCategory is null)
            {
                ErrorMessage = "Please choose a specialty for this trainer.";
                return;
            }

            if (SelectedGym is null)
            {
                ErrorMessage = "Please choose the gym this trainer works at.";
                return;
            }

            IsBusy = true;

            try
            {
                bool isEditing = TrainerId > 0;

                var trainer = new Trainer
                {
                    Id = TrainerId,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Description = Description,
                    Available = Available,
                    CategoryId = SelectedCategory.Id,
                    GymId = SelectedGym.Id,
                    ProfileImage = ProfileImage
                };

                (bool Success, string Message) result;

                if (isEditing)
                {
                    result = await _trainerService.UpdateTrainerAsync(trainer);
                }
                else
                {
                    result = await _trainerService.AddTrainerAsync(trainer);
                }

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                _savedProfileImage = ProfileImage;

                string trainerName = $"{FirstName.Trim()} {LastName.Trim()}";

                if (isEditing)
                {
                    await Shell.Current.DisplayAlertAsync("Trainer Updated", $"{trainerName} has been updated.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Trainer Created", $"{trainerName} has been created.", "OK");
                }

                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't save the trainer. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            DiscardUnsavedPhoto();

            await NavigateAsync("..");
        }

        [RelayCommand]
        private async Task ChangePhotoAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;

            try
            {
                string? newImage = await _imageService.PickImageAsync();

                if (newImage is null)
                {
                    return;
                }

                DiscardUnsavedPhoto();
                ProfileImage = newImage;
            }
            catch (PermissionException)
            {
                ErrorMessage = "PUSH needs permission to access your photos.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load that photo. Please try another.";
            }
        }

        [RelayCommand]
        private void RemovePhoto()
        {
            DiscardUnsavedPhoto();
            ProfileImage = string.Empty;
        }

        // Deletes unused photo
        private void DiscardUnsavedPhoto()
        {
            if (ProfileImage != _savedProfileImage)
            {
                ImageService.DeleteImage(ProfileImage);
            }
        }
    }
}