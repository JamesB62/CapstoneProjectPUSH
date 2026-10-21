namespace CapstoneProjectPUSH.Views.Base;

public partial class LandingPage : ContentPage
{
	public LandingPage()
	{
		InitializeComponent();
	}

    // LoginPage/RegisterPage need view models, so they come from the DI container.
    private T Resolve<T>() where T : class => Handler?.MauiContext?.Services.GetRequiredService<T>() ?? throw new InvalidOperationException("Page is not attached to a handler yet.");

    // Resolves a page and navigates to it. Any error is shown in an alert
    // (with inner exceptions) instead of crashing the app.
    private async Task OpenAsync<TPage>() where TPage : Page
    {
        try
        {
            await Navigation.PushAsync(Resolve<TPage>());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);

            var messages = new List<string>();
            for (var e = ex; e is not null; e = e.InnerException!)
            {
                messages.Add($"{e.GetType().Name}: {e.Message}");
                if (e.InnerException is null)
                {
                    break;
                }
            }

            await DisplayAlertAsync("Navigation error", string.Join("\n\n", messages), "OK");
        }
    }

    private async void LoginButtonClicked(object? sender, EventArgs e) => await OpenAsync<LoginPage>();

    private async void RegisterButtonClicked(object? sender, EventArgs e) => await OpenAsync<RegisterPage>();

    private async void PrivacyPageButton(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new PrivacyPage());
    }
}