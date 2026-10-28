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

        // Wait for 3 seconds (3000 milliseconds)
        await Task.Delay(3000);

        // Navigate to another page using Shell routes
        await Shell.Current.GoToAsync("LandingPage");

        // Animate to 100% (1.0) over 3000 milliseconds (3 seconds)
        await MyProgressBar.ProgressTo(1.0, 3000, Easing.Linear);

    }
}