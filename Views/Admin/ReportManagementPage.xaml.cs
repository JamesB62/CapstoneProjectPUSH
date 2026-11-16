namespace CapstoneProjectPUSH.Views.Admin;

public partial class ReportManagementPage : ContentPage
{
    private readonly ReportManagementViewModel _reportManagementViewModel;

    public ReportManagementPage(ReportManagementViewModel reportManagementViewModel)
    {
        InitializeComponent();

        _reportManagementViewModel = reportManagementViewModel;
        BindingContext = _reportManagementViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _reportManagementViewModel.LoadCommand.Execute(null);
    }
}