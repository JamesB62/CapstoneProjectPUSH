namespace CapstoneProjectPUSH.Views.Trainers;

public partial class TrainerDetailsPage : ContentPage
{
    public TrainerDetailsPage(TrainerDetailsViewModel trainerDetailsViewModel)
	{
		InitializeComponent();
        BindingContext = trainerDetailsViewModel;
    }
}