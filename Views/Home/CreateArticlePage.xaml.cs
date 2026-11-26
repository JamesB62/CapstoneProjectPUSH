namespace CapstoneProjectPUSH.Views.Home;

public partial class CreateArticlePage : ContentPage
{
    private readonly CreateArticleViewModel _createArticleViewModel;

    public CreateArticlePage(CreateArticleViewModel createArticleViewModel)
    {
        InitializeComponent();

        _createArticleViewModel = createArticleViewModel;
        BindingContext = _createArticleViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _createArticleViewModel.LoadCommand.Execute(null);
    }
}