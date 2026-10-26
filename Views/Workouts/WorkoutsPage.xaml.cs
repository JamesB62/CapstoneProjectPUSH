namespace CapstoneProjectPUSH.Views.Workouts;
using CapstoneProjectPUSH.ViewModels.Workouts;
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
