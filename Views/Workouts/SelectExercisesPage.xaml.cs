namespace CapstoneProjectPUSH.Views.Workouts;

public partial class SelectExercisesPage : ContentPage
{
    private readonly SelectExercisesViewModel _selectExercisesViewModel;

    public SelectExercisesPage(SelectExercisesViewModel selectExercisesViewModel)
    {
        InitializeComponent();

        _selectExercisesViewModel = selectExercisesViewModel;
        BindingContext = _selectExercisesViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _selectExercisesViewModel.LoadExercisesCommand.Execute(null);
    }
}