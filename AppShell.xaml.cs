namespace CapstoneProjectPUSH
{
    public partial class AppShell : Shell
    {
        private readonly UserService _userService;

        // Public pages
        private readonly string[] _publicRoutes =
        {
            nameof(FirstLoadingPage),
            nameof(LandingPage),
            nameof(LoginPage),
            nameof(RegisterPage),
            nameof(PrivacyPage)
        };

        // Guest restricted pages
        private readonly string[] _guestRestrictedRoutes =
        {
            nameof(TrainersPage),
            nameof(CommunityWorkoutsPage)
        };

        public AppShell(UserService userService)
        {
            _userService = userService;

            InitializeComponent();

            // Admin
            Routing.RegisterRoute(nameof(AdminDashboardPage), typeof(AdminDashboardPage));
            Routing.RegisterRoute(nameof(CreateGymPage), typeof(CreateGymPage));
            Routing.RegisterRoute(nameof(UserManagementPage), typeof(UserManagementPage));
            Routing.RegisterRoute(nameof(CategoriesManagementPage), typeof(CategoriesManagementPage));
            Routing.RegisterRoute(nameof(GymsManagementPage), typeof(GymsManagementPage));

            // Auth
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

            // Base
            Routing.RegisterRoute(nameof(PrivacyPage), typeof(PrivacyPage));

            // Exercises
            Routing.RegisterRoute(nameof(ExerciseDetailsPage), typeof(ExerciseDetailsPage));

            // Home
            Routing.RegisterRoute(nameof(ArticleDetailsPage), typeof(ArticleDetailsPage));

            // Profile
            Routing.RegisterRoute(nameof(MyNotesPage), typeof(MyNotesPage));
            Routing.RegisterRoute(nameof(MyWorkoutsPage), typeof(MyWorkoutsPage));
            Routing.RegisterRoute(nameof(UpdateProfilePage), typeof(UpdateProfilePage));

            // Trainers
            Routing.RegisterRoute(nameof(TrainerDetailsPage), typeof(TrainerDetailsPage));
            Routing.RegisterRoute(nameof(CreateTrainerPage), typeof(CreateTrainerPage));

            // Workouts
            Routing.RegisterRoute(nameof(ActiveWorkoutPage), typeof(ActiveWorkoutPage));
            Routing.RegisterRoute(nameof(CreateWorkoutPage), typeof(CreateWorkoutPage));
            Routing.RegisterRoute(nameof(NoteFormPage), typeof(NoteFormPage));
            Routing.RegisterRoute(nameof(SelectExercisesPage), typeof(SelectExercisesPage));
            Routing.RegisterRoute(nameof(WorkoutDetailsPage), typeof(WorkoutDetailsPage));
        }

        // Runs every time the user navigates between pages
        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            if (args.Target?.Location == null)
            {
                return;
            }

            if (_userService.IsLoggedIn)
            {
                return;
            }

            string targetRoute = args.Target.Location.OriginalString;

            // Handles signed out users
            if (_userService.IsUnauthenticated)
            {
                bool isPublic = _publicRoutes.Any(route => targetRoute.Contains(route));

                if (!isPublic)
                {
                    args.Cancel();
                    FlyoutIsPresented = false;

                    Dispatcher.Dispatch(async () => await GoToAsync("//LandingPage"));
                }

                return;
            }

            // Handles guest users
            bool isRestricted = _guestRestrictedRoutes.Any(route => targetRoute.Contains(route));

            if (isRestricted)
            {
                args.Cancel();
                FlyoutIsPresented = false;

                Dispatcher.Dispatch(async () => await ShowGuestRestrictedAlertAsync());
            }
        }

        // Guest prompt for restricted pages
        private async Task ShowGuestRestrictedAlertAsync()
        {
            bool goToLanding = await DisplayAlertAsync(
                "Account Required",
                "You are browsing as a guest. Log in or register to gain access to the complete application.",
                "Log In or Register",
                "Cancel");

            if (goToLanding)
            {
                _userService.Logout();
                await GoToAsync("//LandingPage");
            }
        }
    }
}