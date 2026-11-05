using CapstoneProjectPUSH.ViewModels.Admins;
using CapstoneProjectPUSH.Services;


namespace CapstoneProjectPUSH.Views.Admin;

public partial class AdminDashboard : ContentPage
{
    private readonly AdminDashboardViewModel _viewModel;

    public AdminDashboard(AdminDashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}