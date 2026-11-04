namespace CapstoneProjectPUSH.Views.Exercises;

public partial class ExercisesPage : ContentPage
{
	private readonly ExercisesViewModel _exercisesViewModel;
	private readonly UserService _userService;

	public ExercisesPage(ExercisesViewModel exercisesViewModel, UserService userService)
	{
		InitializeComponent();
        _exercisesViewModel = exercisesViewModel;
        _userService = userService;
        BindingContext = _exercisesViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Comment out to continue without being logged in
        if (_userService.CurrentUser is null)
        {
            await Shell.Current.GoToAsync("//LandingPage");
            return;
        }

        await _exercisesViewModel.LoadExercisesCommand.ExecuteAsync(null);
    }
}