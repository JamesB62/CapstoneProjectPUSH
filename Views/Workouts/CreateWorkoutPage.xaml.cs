namespace CapstoneProjectPUSH.views.Workouts;

public partial class CreateWorkoutPage : ContentPage
{
	private readonly CreateWorkoutViewModel _ViewModel;
	public CreateWorkoutPage(CreateWorkoutViewModel viewModel)
	{
		InitializeComponent();
		_ViewModel = viewModel;
		BindingContext = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _ViewModel.LoadCommand.ExecuteAsync(null);
	}
}