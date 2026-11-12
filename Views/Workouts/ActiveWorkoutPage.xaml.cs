namespace CapstoneProjectPUSH.Views.Workouts;

public partial class ActiveWorkoutPage : ContentPage
{
    private readonly ActiveWorkoutViewModel _activeWorkoutViewModel;

    public ActiveWorkoutPage(ActiveWorkoutViewModel activeWorkoutViewModel)
    {
        InitializeComponent();

        _activeWorkoutViewModel = activeWorkoutViewModel;
        BindingContext = _activeWorkoutViewModel;

        // Top bar back arrow asks to discard
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            Command = _activeWorkoutViewModel.DiscardWorkoutCommand
        });
    }

    // Android system back asks to discard
    protected override bool OnBackButtonPressed()
    {
        _activeWorkoutViewModel.DiscardWorkoutCommand.Execute(null);
        return true;
    }
}