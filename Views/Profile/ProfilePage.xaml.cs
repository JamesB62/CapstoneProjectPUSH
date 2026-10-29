namespace CapstoneProjectPUSH.Views.Profile;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _profileViewModel;

    public ProfilePage(ProfileViewModel profileViewModel)
    {
        InitializeComponent();
        _profileViewModel = profileViewModel;
        BindingContext = _profileViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _profileViewModel.LoadProfile();
    }
}