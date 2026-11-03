namespace CapstoneProjectPUSH.Views.Base;

public partial class FirstLoadingPage : ContentPage
{
	public FirstLoadingPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Wait 1 second
        await Task.Delay(1000);

        // Animate to 100% over 3 seconds
        await MyProgressBar.ProgressTo(1.0, 3000, Easing.Linear);

        await Shell.Current.GoToAsync("//LandingPage");
    }
}