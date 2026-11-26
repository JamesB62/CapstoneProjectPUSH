namespace CapstoneProjectPUSH.Views.Home;

public partial class ArticleDetailsPage : ContentPage
{
    private readonly ArticleDetailsViewModel _articleDetailsViewModel;

    public ArticleDetailsPage(ArticleDetailsViewModel articleDetailsViewModel)
    {
        InitializeComponent();

        _articleDetailsViewModel = articleDetailsViewModel;
        BindingContext = _articleDetailsViewModel;

        ToolbarItems.Remove(EditToolbarItem);
        ToolbarItems.Remove(DeleteToolbarItem);
        _articleDetailsViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _articleDetailsViewModel.Reload();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ArticleDetailsViewModel.CanManage))
        {
            UpdateAdminToolbarItems();
        }
    }

    private void UpdateAdminToolbarItems()
    {
        if (_articleDetailsViewModel.CanManage)
        {
            if (!ToolbarItems.Contains(EditToolbarItem))
            {
                ToolbarItems.Add(EditToolbarItem);
            }

            if (!ToolbarItems.Contains(DeleteToolbarItem))
            {
                ToolbarItems.Add(DeleteToolbarItem);
            }
        }
        else
        {
            ToolbarItems.Remove(EditToolbarItem);
            ToolbarItems.Remove(DeleteToolbarItem);
        }
    }
}