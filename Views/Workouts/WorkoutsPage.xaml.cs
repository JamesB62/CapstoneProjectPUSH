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
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _workoutsViewModel.LoadWorkoutsCommand.ExecuteAsync(null);
	}
}