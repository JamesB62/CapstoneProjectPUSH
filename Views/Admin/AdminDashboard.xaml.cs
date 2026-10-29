using CapstoneProjectPUSH.ViewModels.Admins;

namespace CapstoneProjectPUSH.Views.Admin;

public partial class AdminDashboard : ContentPage
{
    private readonly AdminDashboardViewModel _viewModel;

    public AdminDashboard()
    {
        InitializeComponent();
        _viewModel = new AdminDashboardViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    


}
