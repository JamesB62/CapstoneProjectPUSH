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
        ToolbarItems.Remove(EditToolbarItem);
        ToolbarItems.Remove(DeleteToolbarItem);
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
        else if (e.PropertyName == nameof(WorkoutDetailsViewModel.CanEdit))
        {
            UpdateOwnerToolbarItems();
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

    private void UpdateOwnerToolbarItems()
    {
        if (_workoutDetailsViewModel.CanEdit)
        {
            if (!ToolbarItems.Contains(EditToolbarItem))
            {
                ToolbarItems.Add(EditToolbarItem);
            }

            if (!ToolbarItems.Contains(DeleteToolbarItem))
            {
                ToolbarItems.Add(DeleteToolbarItem);
            }
        }
        else
        {
            ToolbarItems.Remove(EditToolbarItem);
            ToolbarItems.Remove(DeleteToolbarItem);
        }
    }
}