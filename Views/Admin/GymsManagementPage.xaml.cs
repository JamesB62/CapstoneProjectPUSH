namespace CapstoneProjectPUSH.Views.Admin;

public partial class GymsManagementPage : ContentPage
{
    private readonly GymsManagementViewModel _viewModel;

    public GymsManagementPage(GymsManagementViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadCommand.Execute(null);
    }
}