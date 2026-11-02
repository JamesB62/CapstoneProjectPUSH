namespace CapstoneProjectPUSH.ViewModels.Trainers
{
    public partial class TrainersViewModel : BaseViewModel
    {
        private readonly TrainerService _trainerService;
        private readonly CategoryService _categoryService;

        public TrainersViewModel(TrainerService trainerService, CategoryService categoryService)
        {
            _trainerService = trainerService;
            _categoryService = categoryService;
        }

        public ObservableCollection<Trainer> Trainers { get; } = new();
        
        private List<Trainer> AllTrainers = new();

        private static readonly Category AllCategory = new() { Id = 0, Name = "All" };

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        private Category? selectedCategory;

        [ObservableProperty]
        private Trainer? selectedTrainer;

        [ObservableProperty]
        private string statusMessage = "Loading trainers...";

        private bool isNavigating;

        [RelayCommand]
        private async Task LoadTrainersAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            StatusMessage = "Loading trainers...";

            try
            {
                if (Categories.Count == 0)
                {
                    Categories.Add(AllCategory);

                    foreach (var category in await _categoryService.GetCategoriesAsync())
                    {
                        Categories.Add(category);
                    }

                    SelectedCategory = AllCategory;
                }

                var trainers = await _trainerService.GetTrainersAsync();
                AllTrainers = trainers.ToList();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Could not load trainers, please try again";
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
                filtered = AllTrainers;
            }
            else
            {
                filtered = AllTrainers.Where(t => t.CategoryId == categoryId).ToList();
            }

            Trainers.Clear();
            foreach (var trainer in filtered)
            {
                Trainers.Add(trainer);
            }

            if (Trainers.Count == 1)
            {
                StatusMessage = "Showing 1 trainer";
            }
            else
            {
                StatusMessage = $"Showing {Trainers.Count} trainers";
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
            if (trainer is null || isNavigating)
            {
                return;
            }

            isNavigating = true;
            try
            {
                await Shell.Current.GoToAsync($"{nameof(TrainerDetailsPage)}?TrainerId={trainer.Id}");
            }
            finally
            {
                isNavigating = false;
            }
        }

        // Add more functions when needed
    }
}