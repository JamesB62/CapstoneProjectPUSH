namespace CapstoneProjectPUSH.Views.Workouts;

public partial class WorkoutDetailsPage : ContentPage
{
    private readonly WorkoutDetailsViewModel _workoutDetailsViewModel;

    public WorkoutDetailsPage(WorkoutDetailsViewModel workoutDetailsViewModel)
    {
        InitializeComponent();

        _workoutDetailsViewModel = workoutDetailsViewModel;
        BindingContext = _workoutDetailsViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _workoutDetailsViewModel.Reload();
    }
}