namespace CapstoneProjectPUSH.Views.Auth;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    private void PrivacyPageButton(object? sender, EventArgs e)
    {
        Navigation.PushAsync(new PrivacyPage());
    }
}