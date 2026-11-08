namespace CapstoneProjectPUSH.ViewModels.Admin
{
    public partial class AdminDashboardViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private readonly ExerciseService _exerciseService;
        private readonly GymService _gymService;
        private readonly TrainerService _trainerService;
        private readonly UserService _userService;

        public AdminDashboardViewModel(
            CategoryService categoryService,
            ExerciseService exerciseService,
            GymService gymService,
            TrainerService trainerService,
            UserService userService)
        {
            _categoryService = categoryService;
            _exerciseService = exerciseService;
            _gymService = gymService;
            _trainerService = trainerService;
            _userService = userService;
        }

        [ObservableProperty]
        public partial int TotalCategories { get; set; }

        [ObservableProperty]
        public partial int TotalExercises { get; set; }

        [ObservableProperty]
        public partial int TotalGyms { get; set; }

        [ObservableProperty]
        public partial int TotalTrainers { get; set; }

        [ObservableProperty]
        public partial int AvailableTrainers { get; set; }

        [ObservableProperty]
        public partial int TotalUsers { get; set; }

        [ObservableProperty]
        public partial int TotalActiveUsers { get; set; }

        [RelayCommand]
        private async Task LoadDashboardAsync()
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

                if (!user.IsAdmin)
                {
                    await Shell.Current.DisplayAlertAsync("Access denied", "This page is for administrators only.", "OK");
                    // Comment out to disable redirect
                    await NavigateAsync("..");
                    return;
                }

                TotalCategories = await _categoryService.GetCategoryCountAsync();
                TotalExercises = await _exerciseService.GetExerciseCountAsync();
                TotalGyms = await _gymService.GetGymCountAsync();
                TotalTrainers = await _trainerService.GetTrainerCountAsync();
                AvailableTrainers = await _trainerService.GetAvailableTrainerCountAsync();
                TotalUsers = await _userService.GetUserCountAsync();
                TotalActiveUsers = await _userService.GetActiveUserCountAsync();

                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                StatusMessage = "Couldn't load dashboard stats. Pull down to try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToUserManagementAsync()
        {
            //await NavigateAsync(nameof(UserManagementPage));
        }

        [RelayCommand]
        private async Task GoToCategoryManagementAsync()
        {
            //await NavigateAsync(nameof(CategoriesManagementPage));
        }

        [RelayCommand]
        private async Task GoToGymManagementAsync()
        {
            //await NavigateAsync(nameof(GymsManagementPage));
        }
    }
}