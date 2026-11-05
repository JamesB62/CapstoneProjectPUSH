namespace CapstoneProjectPUSH.Views.Profile;

public partial class MyWorkoutsPage : ContentPage
{
    private readonly MyWorkoutsViewModel _myWorkoutsViewModel;

    public MyWorkoutsPage(MyWorkoutsViewModel myWorkoutsViewModel)
	{
		InitializeComponent();

        _myWorkoutsViewModel = myWorkoutsViewModel;
        BindingContext = _myWorkoutsViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _myWorkoutsViewModel.LoadMyWorkoutsCommand.Execute(null);
    }
}