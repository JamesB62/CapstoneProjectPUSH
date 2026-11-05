namespace CapstoneProjectPUSH.Views.Admin;

public partial class UsersManagementPage : ContentPage
{
	private readonly UsersManagementViewModel _usersManagementViewModel;
    public UsersManagementPage(UsersManagementViewModel usersManagementViewModel)
	{
		InitializeComponent();

		_usersManagementViewModel = usersManagementViewModel;
		BindingContext = _usersManagementViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _usersManagementViewModel.LoadUsersCommand.Execute(null);
    }
}