namespace CapstoneProjectPUSH.ViewModels.Exercises
{
    [QueryProperty(nameof(ExerciseId), "ExerciseId")]
    public partial class ExerciseDetailsViewModel : BaseViewModel
    {
        private readonly ExerciseService _exerciseService;

        public ExerciseDetailsViewModel(ExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        [ObservableProperty]
        private int exerciseId;

        [ObservableProperty]
        private Exercise? exercise;

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
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                Exercise = await _exerciseService.GetExerciseByIdAsync(ExerciseId);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading exercise: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}