namespace CapstoneProjectPUSH.Views.Workouts;

public partial class WorkoutsPage : ContentPage
{
	private readonly WorkoutsViewModel _workoutsViewModel;

	public WorkoutsPage(WorkoutsViewModel workoutsViewModel)
	{
		InitializeComponent();

		_workoutsViewModel = workoutsViewModel;
		BindingContext = workoutsViewModel;
	}
	protected override void OnAppearing()
	{
		base.OnAppearing();
		_workoutsViewModel.LoadWorkoutsCommand.Execute(null);
	}
}