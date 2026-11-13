using CapstoneProjectPUSH.ViewModels.Trainers;

namespace CapstoneProjectPUSH.Views.Trainers;

public partial class CreateTrainerPage : ContentPage
{
    private readonly CreateTrainerViewModel _viewModel;

    public CreateTrainerPage(CreateTrainerViewModel viewModel)
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