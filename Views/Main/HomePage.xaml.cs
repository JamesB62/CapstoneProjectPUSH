namespace CapstoneProjectPUSH.Views.Main;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
		BindingContext = new HomeViewModel();
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}
}
