using CapstoneProjectPUSH.ViewModels.Admins;

namespace CapstoneProjectPUSH.Views.Admin;

public partial class UsersDetailsPage : ContentPage
{
        private readonly UsersDetailsViewModel _UsersDetailsViewModel;
        private readonly UserService _userService;

        public UsersDetailsPage(UsersDetailsViewModel UsersViewModel, UserService userService)
        {
            InitializeComponent();
            _UsersDetailsViewModel = UsersViewModel;
            _userService = userService;
            BindingContext = _UsersDetailsViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Comment out to continue without being logged in
            if (_userService.CurrentUser is null)
            {
                await Shell.Current.GoToAsync("//LandingPage");
                return;
            }

        }
    }