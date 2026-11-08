namespace CapstoneProjectPUSH
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

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





            // Double check/update page routing before removing

            // Workouts - incorrect
            Routing.RegisterRoute(nameof(WorkoutsPage), typeof(WorkoutsPage));
            //workout detail
            Routing.RegisterRoute(nameof(WorkoutDetailsPage), typeof(WorkoutDetailsPage));
            //create workout page
            Routing.RegisterRoute(nameof(CreateWorkoutPage), typeof(CreateWorkoutPage));
            //Exercises
            Routing.RegisterRoute(nameof(SelectExercisesPage), typeof(SelectExercisesPage));
        }
    }
}