namespace CapstoneProjectPUSH.Views.Main;

public partial class TrainersPage : ContentPage
{
	public TrainersPage()
	{
		InitializeComponent();
		BindingContext = new TrainersViewModel();
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}
}
