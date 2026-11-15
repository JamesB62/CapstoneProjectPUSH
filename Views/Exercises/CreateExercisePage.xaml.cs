namespace CapstoneProjectPUSH.Views.Exercises;

public partial class CreateExercisePage : ContentPage
{
    private readonly CreateExerciseViewModel _createExerciseViewModel;

    public CreateExercisePage(CreateExerciseViewModel createExerciseViewModel)
    {
        InitializeComponent();

        _createExerciseViewModel = createExerciseViewModel;
        BindingContext = _createExerciseViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _createExerciseViewModel.LoadCommand.Execute(null);
    }
}