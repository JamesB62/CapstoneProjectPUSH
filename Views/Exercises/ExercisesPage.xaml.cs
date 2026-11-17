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

        UpdateToolbar();

        _exercisesViewModel.LoadExercisesCommand.Execute(null);
    }

    private void UpdateToolbar()
    {
        bool isAdmin = _exercisesViewModel.IsCurrentUserAdmin;

        if (isAdmin && !ToolbarItems.Contains(AddExerciseToolbarItem))
        {
            ToolbarItems.Add(AddExerciseToolbarItem);
        }
        else if (!isAdmin)
        {
            ToolbarItems.Remove(AddExerciseToolbarItem);
        }
    }
}