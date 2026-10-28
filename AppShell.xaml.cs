namespace CapstoneProjectPUSH
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Auth
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

            // Base
            Routing.RegisterRoute(nameof(LandingPage), typeof(LandingPage));
            Routing.RegisterRoute(nameof(PrivacyPage), typeof(PrivacyPage));
            
            // Home
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            
            // Profile
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            
            // Stats
            Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));
            
            // Trainers
            Routing.RegisterRoute(nameof(TrainersPage), typeof(TrainersPage));
            Routing.RegisterRoute(nameof(TrainerDetailsPage), typeof(TrainerDetailsPage));
            
            // Workouts
            Routing.RegisterRoute(nameof(WorkoutsPage), typeof(WorkoutsPage));
        }
    }
}