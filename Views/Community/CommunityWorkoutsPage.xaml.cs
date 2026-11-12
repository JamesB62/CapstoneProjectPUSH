namespace CapstoneProjectPUSH.Views.Community;

public partial class CommunityWorkoutsPage : ContentPage
{
	private readonly CommunityWorkoutsViewModel _communityWorkoutsViewModel;

	public CommunityWorkoutsPage(CommunityWorkoutsViewModel communityWorkoutsViewModel)
	{
		InitializeComponent();

		_communityWorkoutsViewModel = communityWorkoutsViewModel;
		BindingContext = _communityWorkoutsViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _communityWorkoutsViewModel.LoadWorkoutsCommand.Execute(null);
    }
}