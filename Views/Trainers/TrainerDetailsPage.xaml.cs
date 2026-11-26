namespace CapstoneProjectPUSH.Views.Trainers;

public partial class TrainerDetailsPage : ContentPage
{
    private readonly TrainerDetailsViewModel _trainerDetailsViewModel;

    public TrainerDetailsPage(TrainerDetailsViewModel trainerDetailsViewModel)
	{
		InitializeComponent();

        _trainerDetailsViewModel = trainerDetailsViewModel;
        BindingContext = _trainerDetailsViewModel;

        ToolbarItems.Remove(EditToolbarItem);
        ToolbarItems.Remove(DeleteToolbarItem);
        _trainerDetailsViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _trainerDetailsViewModel.Reload();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TrainerDetailsViewModel.CanManage))
        {
            UpdateAdminToolbarItems();
        }
    }

    private void UpdateAdminToolbarItems()
    {
        if (_trainerDetailsViewModel.CanManage)
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