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

            // Exercises
            Routing.RegisterRoute(nameof(ExerciseDetailsPage), typeof(ExerciseDetailsPage));

            // Profile
            Routing.RegisterRoute(nameof(MyNotesPage), typeof(MyNotesPage));
            Routing.RegisterRoute(nameof(MyWorkoutsPage), typeof(MyWorkoutsPage));
            Routing.RegisterRoute(nameof(UpdateProfilePage), typeof(UpdateProfilePage));




            // Double check/update page routing before removing
            // Home - incorrect
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));

            // Stats - incorrect
            Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));
            
            // Trainers - incorrect
            Routing.RegisterRoute(nameof(TrainersPage), typeof(TrainersPage));
            // Good
            Routing.RegisterRoute(nameof(TrainerDetailsPage), typeof(TrainerDetailsPage));
            
            // Workouts - incorrect
            Routing.RegisterRoute(nameof(WorkoutsPage), typeof(WorkoutsPage));
            //workout detail
            Routing.RegisterRoute(nameof(WorkoutDetailPage), typeof(WorkoutDetailPage));

            //Exercises
            Routing.RegisterRoute(nameof(SelectExercisesPage), typeof(SelectExercisesPage));

            //Admin
            Routing.RegisterRoute(nameof(AdminDashboard), typeof(AdminDashboard));
        }
    }
}