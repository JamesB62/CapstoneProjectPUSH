namespace CapstoneProjectPUSH.Views.Home;

public partial class ArticleDetailsPage : ContentPage
{
	public ArticleDetailsPage(ArticleDetailsViewModel articleDetailsViewModel)
	{
		InitializeComponent();
        BindingContext = articleDetailsViewModel;
    }
}