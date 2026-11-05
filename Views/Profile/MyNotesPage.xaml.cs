namespace CapstoneProjectPUSH.Views.Profile;

public partial class MyNotesPage : ContentPage
{
    private readonly MyNotesViewModel _myNotesViewModel;

    public MyNotesPage(MyNotesViewModel myNotesViewModel)
	{
		InitializeComponent();

        _myNotesViewModel = myNotesViewModel;
        BindingContext = _myNotesViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _myNotesViewModel.LoadMyNotesCommand.Execute(null);
    }
}