namespace CapstoneProjectPUSH.ViewModels.Exercises
{
    [QueryProperty(nameof(ExerciseId), "ExerciseId")]
    public partial class ExerciseDetailsViewModel : BaseViewModel
    {
        private readonly ExerciseService _exerciseService;
        private readonly UserService _userService;

        public ExerciseDetailsViewModel(ExerciseService exerciseService, UserService userService)
        {
            _exerciseService = exerciseService;
            _userService = userService;
        }

        [ObservableProperty]
        public partial int ExerciseId { get; set; }

        [ObservableProperty]
        public partial Exercise? Exercise { get; set; }

        partial void OnExerciseIdChanged(int value)
        {
            if (value <= 0)
            {
                return;
            }

            LoadExerciseCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadExerciseAsync()
        {
            var user = _userService.CurrentUser;

            if (user is null)
            {
                // Comment out to disable redirect
                await Shell.Current.GoToAsync("//LandingPage");
                return;
            }

            IsBusy = true;

            try
            {
                Exercise = await _exerciseService.GetExerciseByIdAsync(ExerciseId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }

            if (Exercise is null)
            {
                await Shell.Current.DisplayAlertAsync("Exercise not found", "This PUSH exercise could not be loaded.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}