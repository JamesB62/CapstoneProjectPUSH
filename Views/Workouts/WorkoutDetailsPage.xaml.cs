namespace CapstoneProjectPUSH.Views.Workouts;

public partial class WorkoutDetailsPage : ContentPage
{
    public WorkoutDetailsPage(WorkoutDetailsViewModel workoutDetailsViewModel)
    {
        InitializeComponent();
        BindingContext = workoutDetailsViewModel;
    }
}