namespace CapstoneProjectPUSH.Views.Stats;
using CapstoneProjectPUSH.ViewModels.Stats;
public partial class StatsPage : ContentPage
{
	public StatsPage()
	{
		InitializeComponent();
		BindingContext = new StatsViewModel();
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}
}
