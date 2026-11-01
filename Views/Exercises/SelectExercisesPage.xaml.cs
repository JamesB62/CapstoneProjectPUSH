namespace CapstoneProjectPUSH.Views.Exercises;

public partial class SelectExercisesPage : ContentPage
{
	private readonly ExercisesViewModel _viewModel;
	public SelectExercisesPage(ExercisesViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.LoadExerciseCommand.ExecuteAsync(null);
	}
}