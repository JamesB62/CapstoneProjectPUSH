namespace CapstoneProjectPUSH.Views.Admin;

public partial class CreateGymPage : ContentPage
{
    private readonly CreateGymViewModel _createGymViewModel;

    public CreateGymPage(CreateGymViewModel createGymViewModel)
    {
        InitializeComponent();

        _createGymViewModel = createGymViewModel;
        BindingContext = _createGymViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _createGymViewModel.LoadCommand.Execute(null);
    }
}