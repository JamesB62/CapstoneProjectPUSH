using CapstoneProjectPUSH.Views.Workouts;
using CapstoneProjectPUSH.Views.Home;
using CapstoneProjectPUSH.Views.Auth;
using CapstoneProjectPUSH.Views.Base;
using CapstoneProjectPUSH.Views.Profile;
using CapstoneProjectPUSH.Views.Trainers;
using CapstoneProjectPUSH.Views.Stats;


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


            Routing.RegisterRoute(nameof(WorkoutsPage), typeof(WorkoutsPage));
            Routing.RegisterRoute(nameof(TrainersPage), typeof(TrainersPage));
            Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        }
    }
}