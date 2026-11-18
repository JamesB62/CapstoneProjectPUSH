namespace CapstoneProjectPUSH.Views.Admin;

public partial class CategoriesManagementPage : ContentPage
{
    private readonly CategoriesManagementViewModel _viewModel;

    public CategoriesManagementPage(CategoriesManagementViewModel viewModel)
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
