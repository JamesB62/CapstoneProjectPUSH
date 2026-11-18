namespace CapstoneProjectPUSH.Views.Admin;

public partial class ReportDetailsPage : ContentPage
{
    private readonly ReportDetailsViewModel _reportDetailsViewModel;

    public ReportDetailsPage(ReportDetailsViewModel reportDetailsViewModel)
    {
        InitializeComponent();

        _reportDetailsViewModel = reportDetailsViewModel;
        BindingContext = _reportDetailsViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _reportDetailsViewModel.Reload();
    }
}