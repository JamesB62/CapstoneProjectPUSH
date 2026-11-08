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

        private const string PlaceholderImage = "exercise_placeholder.jpg";

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
                await NavigateAsync("//LandingPage");
                return;
            }

            IsBusy = true;

            try
            {
                var exercise = await _exerciseService.GetExerciseByIdAsync(ExerciseId);

                if (exercise != null)
                {
                    exercise.Image = GetImageOrPlaceholder(exercise.Image);
                }

                Exercise = exercise;
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
                await NavigateAsync("..");
            }
        }

        private static string GetImageOrPlaceholder(string? imagePath)
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
    }
}