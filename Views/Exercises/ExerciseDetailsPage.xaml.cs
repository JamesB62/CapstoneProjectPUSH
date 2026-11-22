namespace CapstoneProjectPUSH.Views.Exercises;

public partial class ExerciseDetailsPage : ContentPage
{
    private readonly ExerciseDetailsViewModel _exerciseDetailsViewModel;

    public ExerciseDetailsPage(ExerciseDetailsViewModel exerciseDetailsViewModel)
    {
        InitializeComponent();

        _exerciseDetailsViewModel = exerciseDetailsViewModel;
        BindingContext = _exerciseDetailsViewModel;

        ToolbarItems.Remove(EditToolbarItem);
        ToolbarItems.Remove(DeleteToolbarItem);
        _exerciseDetailsViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _exerciseDetailsViewModel.Reload();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ExerciseDetailsViewModel.CanManage))
        {
            UpdateAdminToolbarItems();
        }
    }

    private void UpdateAdminToolbarItems()
    {
        if (_exerciseDetailsViewModel.CanManage)
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