namespace CapstoneProjectPUSH.Views.Trainers;
using CapstoneProjectPUSH.ViewModels.Trainers;

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
