namespace CapstoneProjectPUSH.Views.Workouts;

public partial class WorkoutDetailsPage : ContentPage
{
    private readonly WorkoutDetailsViewModel _workoutDetailsViewModel;

    public WorkoutDetailsPage(WorkoutDetailsViewModel workoutDetailsViewModel)
    {
        InitializeComponent();

        _workoutDetailsViewModel = workoutDetailsViewModel;
        BindingContext = _workoutDetailsViewModel;

        ToolbarItems.Remove(ReportToolbarItem);
        _workoutDetailsViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _workoutDetailsViewModel.Reload();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WorkoutDetailsViewModel.CanReport))
        {
            UpdateReportToolbarItem();
        }
    }

    private void UpdateReportToolbarItem()
    {
        if (_workoutDetailsViewModel.CanReport)
        {
            if (!ToolbarItems.Contains(ReportToolbarItem))
            {
                ToolbarItems.Add(ReportToolbarItem);
            }
        }
        else
        {
            ToolbarItems.Remove(ReportToolbarItem);
        }
    }
}