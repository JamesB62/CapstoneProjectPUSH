namespace CapstoneProjectPUSH.Views.Workouts;

public partial class CreateWorkoutPage : ContentPage
{
    private readonly CreateWorkoutViewModel _createWorkoutViewModel;

    public CreateWorkoutPage(CreateWorkoutViewModel createWorkoutViewModel)
    {
        InitializeComponent();

        _createWorkoutViewModel = createWorkoutViewModel;
        BindingContext = _createWorkoutViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _createWorkoutViewModel.LoadFormCommand.Execute(null);
    }
}