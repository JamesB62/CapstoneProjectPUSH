namespace CapstoneProjectPUSH.Views.Profile;

public partial class MyNotesPage : ContentPage
{
    private readonly MyNotesViewModel _myNotesViewModel;
    private readonly UserService _userService;
    public MyNotesPage(MyNotesViewModel myNotesViewModel, UserService userService)
	{
		InitializeComponent();
        _myNotesViewModel = myNotesViewModel;
        _userService = userService;
        BindingContext = _myNotesViewModel;
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

        await _myNotesViewModel.LoadMyNotesCommand.ExecuteAsync(null);
    }
}