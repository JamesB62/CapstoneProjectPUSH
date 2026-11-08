namespace CapstoneProjectPUSH.Views.Profile;

public partial class UpdateProfilePage : ContentPage
{
    private readonly UpdateProfileViewModel _updateProfileViewModel;

    public UpdateProfilePage(UpdateProfileViewModel updateProfileViewModel)
	{
		InitializeComponent();

        _updateProfileViewModel = updateProfileViewModel;
        BindingContext = _updateProfileViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _updateProfileViewModel.LoadProfileCommand.Execute(null);
    }
}