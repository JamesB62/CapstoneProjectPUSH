namespace CapstoneProjectPUSH.Views.Trainers;

public partial class TrainersPage : ContentPage
{
	private readonly TrainersViewModel _trainersViewModel;
    private readonly UserService _userService;

    public TrainersPage(TrainersViewModel trainersViewModel, UserService userService)
	{
		InitializeComponent();
		_trainersViewModel = trainersViewModel;
		_userService = userService;
		BindingContext = _trainersViewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// Comment out to continue without being logged in
		if (_userService.CurrentUser is null)
		{
			await Shell.Current.GoToAsync("//LandingPage");
			return;
		}

		await _trainersViewModel.LoadTrainersCommand.ExecuteAsync(null);
	}
}