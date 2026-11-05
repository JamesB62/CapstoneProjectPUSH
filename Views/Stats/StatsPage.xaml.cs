namespace CapstoneProjectPUSH.Views.Stats;

using CapstoneProjectPUSH.ViewModels.Stats;

public partial class StatsPage : ContentPage
{
    private readonly StatsViewModel _viewModel;

    public StatsPage(StatsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }




}