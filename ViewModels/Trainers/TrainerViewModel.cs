namespace CapstoneProjectPUSH.ViewModels.Trainers
{
    public partial class TrainerViewModel : BaseViewModel
    {
        private readonly TrainerService _trainerService;
        private readonly CategoryService _categoryService;

        public TrainerViewModel(TrainerService trainerService, CategoryService categoryService)
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
        private string statusMessage = "Loading trainers...";

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

            switch (Trainers.Count)
            {
                case 0:
                    StatusMessage = "No trainers found";
                    break;

                case 1:
                    StatusMessage = "Showing 1 trainer";
                    break;

                default:
                    StatusMessage = $"Showing {Trainers.Count} trainers";
                    break;
            }
        }

        // Add more functions when needed
    }
}