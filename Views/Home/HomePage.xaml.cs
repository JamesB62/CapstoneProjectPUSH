namespace CapstoneProjectPUSH.Views.Home;

public partial class HomePage : ContentPage
{
	private readonly HomeViewModel _homeViewModel;

	public HomePage(HomeViewModel homeViewModel)
	{
		InitializeComponent();
		_homeViewModel = homeViewModel;
		BindingContext = _homeViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        UpdateToolbar();

        _homeViewModel.LoadHomeCommand.Execute(null);
    }

    private void UpdateToolbar()
    {
        bool isAdmin = _homeViewModel.IsCurrentUserAdmin;

        if (isAdmin && !ToolbarItems.Contains(AddArticleToolbarItem))
        {
            ToolbarItems.Add(AddArticleToolbarItem);
        }
        else if (!isAdmin)
        {
            ToolbarItems.Remove(AddArticleToolbarItem);
        }
    }
}