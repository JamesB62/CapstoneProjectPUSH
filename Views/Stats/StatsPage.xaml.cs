namespace CapstoneProjectPUSH.Views.Stats;

public partial class StatsPage : ContentPage
{
    private readonly StatsViewModel _statsViewModel;

    public StatsPage(StatsViewModel statsViewModel)
    {
        InitializeComponent();

        _statsViewModel = statsViewModel;
        BindingContext = _statsViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _statsViewModel.LoadStatsCommand.Execute(null);
    }
}