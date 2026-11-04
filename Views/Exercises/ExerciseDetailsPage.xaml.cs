namespace CapstoneProjectPUSH.Views.Exercises;

public partial class ExerciseDetailsPage : ContentPage
{
    private readonly UserService _userService;
    public ExerciseDetailsPage(ExerciseDetailsViewModel exerciseDetailsViewModel, UserService userService)
    {
        InitializeComponent();
        _userService = userService;
        BindingContext = exerciseDetailsViewModel;
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
    }
}