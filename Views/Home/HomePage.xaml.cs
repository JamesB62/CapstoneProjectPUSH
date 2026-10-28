namespace CapstoneProjectPUSH.Views.Home;
using CapstoneProjectPUSH.ViewModels.Home;
using CapstoneProjectPUSH.Models;
using CapstoneProjectPUSH.Services;

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
