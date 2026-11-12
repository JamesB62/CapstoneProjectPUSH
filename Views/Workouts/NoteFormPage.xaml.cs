namespace CapstoneProjectPUSH.Views.Workouts;

public partial class NoteFormPage : ContentPage
{
	public NoteFormPage(NoteFormViewModel noteFormViewModel)
	{
		InitializeComponent();
        BindingContext = noteFormViewModel;
    }
}