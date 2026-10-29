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
            Routing.RegisterRoute(nameof(PrivacyPage), typeof(PrivacyPage));
            
            // Home - incorrect
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            
            // Profile - incorrect
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            
            // Stats - incorrect
            Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));
            
            // Trainers - incorrect
            Routing.RegisterRoute(nameof(TrainersPage), typeof(TrainersPage));
            // Good
            Routing.RegisterRoute(nameof(TrainerDetailsPage), typeof(TrainerDetailsPage));
            
            // Workouts - incorrect
            Routing.RegisterRoute(nameof(WorkoutsPage), typeof(WorkoutsPage));
        }
    }
}