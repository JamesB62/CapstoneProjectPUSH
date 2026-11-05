namespace CapstoneProjectPUSH.Views.Exercises;

public partial class ExerciseDetailsPage : ContentPage
{
    public ExerciseDetailsPage(ExerciseDetailsViewModel exerciseDetailsViewModel)
    {
        InitializeComponent();
        BindingContext = exerciseDetailsViewModel;
    }
}