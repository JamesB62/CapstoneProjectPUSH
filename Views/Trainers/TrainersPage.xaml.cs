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
		_trainersViewModel.LoadTrainersCommand.Execute(null);
	}
}