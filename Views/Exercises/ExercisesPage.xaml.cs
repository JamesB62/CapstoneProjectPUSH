namespace CapstoneProjectPUSH.Views.Exercises;

public partial class ExercisesPage : ContentPage
{
	private readonly ExercisesViewModel _exercisesViewModel;

	public ExercisesPage(ExercisesViewModel exercisesViewModel)
	{
		InitializeComponent();

        _exercisesViewModel = exercisesViewModel;
        BindingContext = _exercisesViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _exercisesViewModel.LoadExercisesCommand.Execute(null);
    }
}