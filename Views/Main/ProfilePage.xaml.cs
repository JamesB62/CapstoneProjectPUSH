namespace CapstoneProjectPUSH.Views.Main;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(UserService userService)
	{
		InitializeComponent();
		BindingContext = new ProfileViewModel(userService);
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}
}
