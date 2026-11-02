namespace CapstoneProjectPUSH.Views.Workouts;

public partial class WorkoutDetailPage : ContentPage
{
	private readonly WorkoutsDetailViewModel _viewModel;
	public WorkoutDetailPage(WorkoutsDetailViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.LoadCommand.ExecuteAsync(null);
	}
}