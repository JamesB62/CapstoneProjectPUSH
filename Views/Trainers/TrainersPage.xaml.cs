namespace CapstoneProjectPUSH.Views.Trainers;

public partial class TrainersPage : ContentPage
{
	private readonly TrainersViewModel _trainersViewModel;

    public TrainersPage(TrainersViewModel trainersViewModel)
	{
		InitializeComponent();

		_trainersViewModel = trainersViewModel;
		BindingContext = _trainersViewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

        UpdateToolbar();

        _trainersViewModel.LoadTrainersCommand.Execute(null);
	}

    private void UpdateToolbar()
    {
        bool isAdmin = _trainersViewModel.IsCurrentUserAdmin;

        if (isAdmin && !ToolbarItems.Contains(AddTrainerToolbarItem))
        {
            ToolbarItems.Add(AddTrainerToolbarItem);
        }
        else if (!isAdmin)
        {
            ToolbarItems.Remove(AddTrainerToolbarItem);
        }
    }
}