namespace CapstoneProjectPUSH.Services
{
    public interface INavigationService
    {
        Task GoToAsync<TPage>() where TPage : Page;
        Task GoToRootAsync();
        Task ShowAlertAsync(string title, string message);
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _services;
        public NavigationService(IServiceProvider services) => _services = services;
        private static Page CurrentRootPage => Application.Current?.Windows.FirstOrDefault()?.Page ?? throw new InvalidOperationException("No active window/page.");

        public Task GoToAsync<TPage>() where TPage : Page
        {
            var page = _services.GetRequiredService<TPage>();
            return CurrentRootPage.Navigation.PushAsync(page);
        }

        public Task GoToRootAsync() => CurrentRootPage.Navigation.PopToRootAsync();

        public Task ShowAlertAsync(string title, string message) => CurrentRootPage.DisplayAlertAsync(title, message, "OK");
    }
}