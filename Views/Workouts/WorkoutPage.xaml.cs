namespace CapstoneProjectPUSH.Views.Workouts;

public partial class WorkoutPage : ContentPage
{
	private readonly WorkoutViewModel _workoutViewModel;

	public WorkoutPage(WorkoutViewModel workoutViewModel)
	{
		InitializeComponent();
		_workoutViewModel = workoutViewModel;
		BindingContext = workoutViewModel;
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _workoutViewModel.LoadWorkoutsCommand.ExecuteAsync(null);
	}
}