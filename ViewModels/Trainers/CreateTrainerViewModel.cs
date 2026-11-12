using System.Collections.ObjectModel;
using CapstoneProjectPUSH.Models;
using CapstoneProjectPUSH.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CapstoneProjectPUSH.ViewModels.Trainers
{
    public partial class CreateTrainerViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly CategoryService _categoryService;
        private readonly GymService _gymService;
        private readonly TrainerService _trainerService;
        private readonly UserService _userService;

        // Gyms in the same order as GymChoices (the picker items)
        private List<Gym> _gyms = new();

        // Set when the page is opened to edit an existing trainer
        private int? _editingTrainerId;
        private bool _hasLoadedTrainer;

        public CreateTrainerViewModel(
            CategoryService categoryService,
            GymService gymService,
            TrainerService trainerService,
            UserService userService)
        {
            _categoryService = categoryService;
            _gymService = gymService;
            _trainerService = trainerService;
            _userService = userService;
        }

        public ObservableCollection<Category> Categories { get; } = new();

        // Picker items shown as "Name (City)"
        public ObservableCollection<string> GymChoices { get; } = new();

        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Create PUSH Trainer";

        [ObservableProperty]
        public partial string SaveButtonText { get; set; } = "Save Trainer";

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
        public partial int SelectedGymIndex { get; set; } = -1;

        // Receives TrainerId when opened from the trainers page to edit a trainer so it doesnt crash😛
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("TrainerId", out var value)
                && int.TryParse(value?.ToString(), out var trainerId)
                && trainerId > 0)
            {
                _editingTrainerId = trainerId;
                PageTitle = "Edit PUSH Trainer";
                SaveButtonText = "Save Changes";
            }
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            IsBusy = true;

            try
            {
                if (!await EnsureAdminAsync())
                {
                    return;
                }

                if (Categories.Count == 0)
                {
                    var categories = await _categoryService.GetCategoriesAsync();

                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }
                }

                if (_gyms.Count == 0)
                {
                    _gyms = await _gymService.GetGymsAsync();

                    foreach (var gym in _gyms)
                    {
                        GymChoices.Add($"{gym.Name} ({gym.City})");
                    }
                }

                // Edit mode: fill the form once with the trainer's current details
                if (_editingTrainerId is int trainerId && !_hasLoadedTrainer)
                {
                    var trainer = await _trainerService.GetTrainerByIdAsync(trainerId);

                    if (trainer is null)
                    {
                        await Shell.Current.DisplayAlertAsync("Trainer not found", "That trainer no longer exists.", "OK");
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
                    SelectedGymIndex = _gyms.FindIndex(g => g.Id == trainer.GymId);

                    _hasLoadedTrainer = true;
                }
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

        [RelayCommand]
        private async Task SaveAsync()
        {
            StatusMessage = string.Empty;

            if (!await EnsureAdminAsync())
            {
                return;
            }

            if (SelectedCategory is null)
            {
                StatusMessage = "Pick a category.";
                return;
            }

            if (SelectedGymIndex < 0 || SelectedGymIndex >= _gyms.Count)
            {
                StatusMessage = "Pick a gym.";
                return;
            }

            IsBusy = true;

            try
            {
                var trainer = new Trainer
                {
                    Id = _editingTrainerId ?? 0,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Description = Description,
                    Available = Available,
                    CategoryId = SelectedCategory.Id,
                    GymId = _gyms[SelectedGymIndex].Id
                };

                var isEditing = _editingTrainerId is not null;

                var result = isEditing
                    ? await _trainerService.UpdateTrainerAsync(trainer)
                    : await _trainerService.AddTrainerAsync(trainer);

                if (!result.Success)
                {
                    StatusMessage = result.Message;
                    return;
                }

                await Shell.Current.DisplayAlertAsync(isEditing ? "Trainer updated" : "Trainer created", result.Message, "OK");
                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't save the trainer. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Same access rules as the admin dashboard
        private async Task<bool> EnsureAdminAsync()
        {
            var user = _userService.CurrentUser;

            if (user is null)
            {
                // Comment out to disable redirect
                await NavigateAsync("//LandingPage");
                return false;
            }

            if (!user.IsAdmin)
            {
                await Shell.Current.DisplayAlertAsync("Access denied", "This page is for administrators only.", "OK");
                // Comment out to disable redirect
                await NavigateAsync("..");
                return false;
            }

            return true;
        }
    }
}