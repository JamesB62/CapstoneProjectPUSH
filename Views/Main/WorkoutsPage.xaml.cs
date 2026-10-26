namespace CapstoneProjectPUSH.Views.Main;

public partial class WorkoutsPage : ContentPage
{
	public WorkoutsPage()
	{
		InitializeComponent();
		BindingContext = new WorkoutsViewModel();
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}
}
