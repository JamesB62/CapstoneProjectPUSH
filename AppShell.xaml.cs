namespace CapstoneProjectPUSH
{
    public partial class AppShell : Shell 
    {

        public AppShell(UserService userService)
        {
            InitializeComponent();
            _userService = userService;

            //Admin
            Routing.RegisterRoute(nameof(AdminDashboardPage), typeof(AdminDashboardPage));

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

            // Workouts
            Routing.RegisterRoute(nameof(CreateWorkoutPage), typeof(CreateWorkoutPage));
            Routing.RegisterRoute(nameof(SelectExercisesPage), typeof(SelectExercisesPage));
            Routing.RegisterRoute(nameof(WorkoutDetailsPage), typeof(WorkoutDetailsPage));
        }

        //guest nav bar blocker
        private readonly UserService _userService;
        private readonly string[] _restrictedRoutes = {"TrainersPage"};

        protected override async void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            if (args.Target?.Location == null || _userService == null)
            return;

            if (_userService.IsLoggedIn)
            return;

            string targetRoute = args.Target.Location.OriginalString;

            bool isRestricted = _restrictedRoutes.Any(route => targetRoute.Contains(route));

            if(isRestricted)
            {
                args.Cancel();

                FlyoutIsPresented = false;

                string title = _userService.IsGuest ? "Guest mode is restricted" : "Login required";
                string message = _userService.IsGuest
                ? "You are currently browsing using a guest account. Log in to your registered account to view your personal profile and data."
                : "Please log in to your account to continue.";

                bool goToLogin = await MainThread.InvokeOnMainThreadAsync(() =>
                DisplayAlertAsync(title, message, "Log in", "Cancel"));

                 if (goToLogin)
                {
                    await Shell.Current.GoToAsync(nameof(LoginPage));
                }

            }

        }


    }
}