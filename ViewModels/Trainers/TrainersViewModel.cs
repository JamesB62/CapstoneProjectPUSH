namespace CapstoneProjectPUSH.ViewModels.Trainers
{
    public partial class TrainersViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private readonly TrainerService _trainerService;
        private readonly UserService _userService;

        public TrainersViewModel(CategoryService categoryService, TrainerService trainerService, UserService userService)
        {
            _categoryService = categoryService;
            _trainerService = trainerService;
            _userService = userService;
        }

        public ObservableCollection<Trainer> Trainers { get; } = new();
        
        private List<Trainer> _allTrainers = new();

        private static readonly Category AllCategory = new() { Id = 0, Name = "All" };

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        [ObservableProperty]
        public partial Trainer? SelectedTrainer { get; set; }

        [ObservableProperty]
        public partial bool ShowAvailableOnly { get; set; }

        [ObservableProperty]
        public partial string StatusMessage { get; set; } = "Loading trainers...";

        private bool _isNavigating;

        [RelayCommand]
        private async Task LoadTrainersAsync()
        {
            var user = _userService.CurrentUser;

            if (user is null)
            {
                // Comment out to disable redirect
                await Shell.Current.GoToAsync("//LandingPage");
                return;
            }

            IsBusy = true;
            StatusMessage = "Loading trainers...";

            try
            {
                if (Categories.Count == 0)
                {
                    var categories = await _categoryService.GetCategoriesAsync();

                    Categories.Clear();
                    Categories.Add(AllCategory);

                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }

                    SelectedCategory = AllCategory;
                }

                var trainers = await _trainerService.GetTrainersAsync();
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

            ViewTrainerCommand.Execute(value);
            SelectedTrainer = null;
        }

        [RelayCommand]
        private async Task ViewTrainerAsync(Trainer? trainer)
        {
            if (trainer is null || _isNavigating)
            {
                return;
            }

            _isNavigating = true;
            try
            {
                await Shell.Current.GoToAsync($"{nameof(TrainerDetailsPage)}?TrainerId={trainer.Id}");
            }
            finally
            {
                _isNavigating = false;
            }
        }
    }
}