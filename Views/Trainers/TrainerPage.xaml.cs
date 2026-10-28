namespace CapstoneProjectPUSH.Views.Trainers;

public partial class TrainerPage : ContentPage
{
	private readonly TrainerViewModel _trainerViewModel;

	public TrainerPage(TrainerViewModel trainerViewModel)
	{
		InitializeComponent();
		_trainerViewModel = trainerViewModel;
		BindingContext = _trainerViewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _trainerViewModel.LoadTrainersCommand.ExecuteAsync(null);
	}
}