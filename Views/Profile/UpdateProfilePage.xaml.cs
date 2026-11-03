namespace CapstoneProjectPUSH.Views.Profile;

public partial class UpdateProfilePage : ContentPage
{
	private readonly UserService _userService;
    private readonly UpdateProfileViewModel _updateProfileViewModel;
    public UpdateProfilePage(UpdateProfileViewModel updateProfileViewModel, UserService userService)
	{
		InitializeComponent();
		_userService = userService;
        _updateProfileViewModel = updateProfileViewModel;
        BindingContext = _updateProfileViewModel;

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

        await _updateProfileViewModel.LoadCommand.ExecuteAsync(null);
    }
}