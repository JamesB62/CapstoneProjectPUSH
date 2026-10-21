namespace CapstoneProjectPUSH.Views.Base;

public partial class LandingPage : ContentPage
{
	public LandingPage(LandingViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}