namespace CapstoneProjectPUSH.Views.Home;
using CapstoneProjectPUSH.ViewModels.Home;

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
