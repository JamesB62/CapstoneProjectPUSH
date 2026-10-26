using Microsoft.Extensions.DependencyInjection;
using CapstoneProjectPUSH.Services;
using CapstoneProjectPUSH.Views.Main;

namespace CapstoneProjectPUSH.Views.Controls;

public partial class MainNavBar : ContentView
{
	public static readonly BindableProperty ActiveTabProperty =
		BindableProperty.Create(nameof(ActiveTab), typeof(string), typeof(MainNavBar), string.Empty,
			propertyChanged: OnActiveTabChanged);

	public string ActiveTab
	{
		get { return (string)GetValue(ActiveTabProperty); }
		set { SetValue(ActiveTabProperty, value); }
	}

	public MainNavBar()
	{
		InitializeComponent();
	}

	private static void OnActiveTabChanged(BindableObject bindable, object oldValue, object newValue)
	{
		MainNavBar navBar = (MainNavBar)bindable;
		navBar.ColourButtons();
	}

	private void ColourButtons()
	{
		SetButtonColour(HomeButton, ActiveTab == "Home");
		SetButtonColour(WorkoutsButton, ActiveTab == "Workouts");
		SetButtonColour(StatsButton, ActiveTab == "Stats");
		SetButtonColour(TrainersButton, ActiveTab == "Trainers");
		SetButtonColour(ProfileButton, ActiveTab == "Profile");
	}

	private void SetButtonColour(Button button, bool isActive)
	{
		if (isActive)
		{
			button.BackgroundColor = Color.FromArgb("#4F46E5");
			button.TextColor = Colors.White;
		}
		else
		{
			button.BackgroundColor = Color.FromArgb("#9CA3AF");
			button.TextColor = Colors.Black;
		}
	}

	private async Task GoToPage(Page page)
	{
		Page currentPage = Shell.Current.CurrentPage;

		await Shell.Current.Navigation.PushAsync(page, false);
		Shell.Current.Navigation.RemovePage(currentPage);
	}

	private async void OnHomeClicked(object? sender, EventArgs e)
	{
		if (ActiveTab != "Home")
		{
			await GoToPage(new HomePage());
		}
	}

	private async void OnWorkoutsClicked(object? sender, EventArgs e)
	{
		if (ActiveTab != "Workouts")
		{
			await GoToPage(new WorkoutsPage());
		}
	}

	private async void OnStatsClicked(object? sender, EventArgs e)
	{
		if (ActiveTab != "Stats")
		{
			await GoToPage(new StatsPage());
		}
	}

	private async void OnTrainersClicked(object? sender, EventArgs e)
	{
		if (ActiveTab != "Trainers")
		{
			await GoToPage(new TrainersPage());
		}
	}

	private async void OnProfileClicked(object? sender, EventArgs e)
	{
		if (ActiveTab != "Profile")
		{
			var userService = Handler!.MauiContext!.Services.GetService<UserService>()!;
			await GoToPage(new ProfilePage(userService));
		}
	}
}
