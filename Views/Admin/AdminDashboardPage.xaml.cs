namespace CapstoneProjectPUSH.Views.Admin;

public partial class AdminDashboardPage : ContentPage
{
    private readonly AdminDashboardViewModel _adminDashboardViewModel;

    public AdminDashboardPage(AdminDashboardViewModel adminDashboardViewModel)
    {
        InitializeComponent();

        _adminDashboardViewModel = adminDashboardViewModel;
        BindingContext = _adminDashboardViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
       _adminDashboardViewModel.LoadDashboardCommand.Execute(null);
    }
}