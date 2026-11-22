namespace CapstoneProjectPUSH.ViewModels.Exercises
{
    [QueryProperty(nameof(ExerciseId), "ExerciseId")]
    public partial class ExerciseDetailsViewModel : BaseViewModel
    {
        private readonly ExerciseService _exerciseService;

        public ExerciseDetailsViewModel(ExerciseService exerciseService, UserService userService) : base(userService)
        {
            _exerciseService = exerciseService;
        }

        private const string PlaceholderImage = "exercise_placeholder.png";

        [ObservableProperty]
        public partial int ExerciseId { get; set; }

        [ObservableProperty]
        public partial Exercise? Exercise { get; set; }

        [ObservableProperty]
        public partial bool CanManage { get; set; }

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

            // AppShell sends signed-out users to the landing page
            if (user is null)
            {
                return;
            }

            CanManage = _userService.IsCurrentUserAdmin();
            IsBusy = true;

            try
            {
                var exercise = await _exerciseService.GetExerciseByIdAsync(ExerciseId);

                if (exercise is not null)
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
                await Shell.Current.DisplayAlertAsync("Exercise Not Found", "This PUSH exercise could not be loaded.", "OK");
                await NavigateAsync("..");
            }
        }

        // Reloads after coming back from the edit form
        public void Reload()
        {
            if (Exercise is null)
            {
                return;
            }

            LoadExerciseCommand.Execute(null);
        }

        [RelayCommand]
        private async Task EditExerciseAsync()
        {
            if (Exercise is null || !CanManage)
            {
                return;
            }

            var parameters = new ShellNavigationQueryParameters
            {
                { "ExerciseId", Exercise.Id }
            };

            await NavigateAsync(nameof(CreateExercisePage), parameters);
        }

        [RelayCommand]
        private async Task DeleteExerciseAsync()
        {
            if (Exercise is null || !CanManage || IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;

            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete Exercise",
                $"Delete {Exercise.Title}? It will also be removed from every workout that uses it. This can't be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            bool deleted = false;
            IsBusy = true;

            try
            {
                var result = await _exerciseService.DeleteExerciseAsync(Exercise.Id);

                if (result.Success)
                {
                    deleted = true;
                }
                else
                {
                    ErrorMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't delete the exercise. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }

            if (deleted)
            {
                await NavigateAsync("..");
            }
        }

        private static string GetImageOrPlaceholder(string? imagePath)
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

            if (!File.Exists(imagePath))
            {
                return PlaceholderImage;
            }

            return imagePath;
        }
    }
}