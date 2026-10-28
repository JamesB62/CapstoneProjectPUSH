namespace CapstoneProjectPUSH.Views.Trainers;

public partial class TrainerDetailsPage : ContentPage
{
    private readonly UserService _userService;
    public TrainerDetailsPage(TrainerDetailsViewModel trainerDetailsViewModel, UserService userService)
	{
		InitializeComponent();
        _userService = userService;
        BindingContext = trainerDetailsViewModel;
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