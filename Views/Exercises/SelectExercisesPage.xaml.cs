namespace CapstoneProjectPUSH.Views.Exercises;

public partial class SelectExercisesPage : ContentPage
{
	private readonly SelectExercisesViewModel _viewModel;
	public SelectExercisesPage(SelectExercisesViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadExercisesCommand.ExecuteAsync(null);
    }

}