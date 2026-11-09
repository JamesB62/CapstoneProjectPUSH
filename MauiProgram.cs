namespace CapstoneProjectPUSH
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // ========================= Services =========================

            builder.Services.AddSingleton<ArticleService>();
            builder.Services.AddSingleton<CategoryService>();
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<ExerciseService>(); 
            builder.Services.AddSingleton<GymService>();
            builder.Services.AddSingleton<NoteService>();
            builder.Services.AddSingleton<StatsService>();
            builder.Services.AddSingleton<TrainerService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<WeightLogService>();
            builder.Services.AddSingleton<WorkoutLogService>();
            builder.Services.AddSingleton<WorkoutService>();


            // ======================== ViewModels ========================

            // Admin
            builder.Services.AddTransient<AdminDashboardViewModel>();
            builder.Services.AddTransient<UserManagementViewModel>();
            builder.Services.AddTransient<CategoriesManagementViewModel>();
            builder.Services.AddTransient<GymsManagementViewModel>();

            // Auth
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();

            // Base
            builder.Services.AddTransient<LandingViewModel>();

            // Exercises
            builder.Services.AddTransient<ExerciseDetailsViewModel>();
            builder.Services.AddTransient<ExercisesViewModel>();

            // Home
            builder.Services.AddTransient<ArticleDetailsViewModel>();
            builder.Services.AddTransient<HomeViewModel>();

            // Profile
            builder.Services.AddTransient<MyNotesViewModel>();
            builder.Services.AddTransient<MyWorkoutsViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<UpdateProfileViewModel>();

            // Stats
            builder.Services.AddTransient<StatsViewModel>();

            // Trainers
            builder.Services.AddTransient<TrainerDetailsViewModel>();
            builder.Services.AddTransient<TrainersViewModel>();

            // Workouts
            builder.Services.AddTransient<CreateWorkoutViewModel>();
            builder.Services.AddTransient<SelectExercisesViewModel>();
            builder.Services.AddTransient<WorkoutDetailsViewModel>();
            builder.Services.AddTransient<WorkoutsViewModel>();

            // ========================== Views ===========================

            // Admin
            builder.Services.AddTransient<AdminDashboardPage>();
            builder.Services.AddTransient<UserManagementPage>();
            builder.Services.AddTransient<CategoriesManagementPage>();
            builder.Services.AddTransient<GymsManagementPage>();

            // Auth
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();

            // Base
            builder.Services.AddTransient<LandingPage>();
            builder.Services.AddTransient<PrivacyPage>();

            // Exercises
            builder.Services.AddTransient<ExerciseDetailsPage>();
            builder.Services.AddTransient<ExercisesPage>();

            // Home
            builder.Services.AddTransient<ArticleDetailsPage>();
            builder.Services.AddTransient<HomePage>();

            // Profile
            builder.Services.AddTransient<MyNotesPage>();
            builder.Services.AddTransient<MyWorkoutsPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<UpdateProfilePage>();

            // Stats
            builder.Services.AddTransient<StatsPage>();
          
            // Trainers
            builder.Services.AddTransient<TrainerDetailsPage>();
            builder.Services.AddTransient<TrainersPage>();

            // Workouts
            builder.Services.AddTransient<CreateWorkoutPage>();
            builder.Services.AddTransient<SelectExercisesPage>();
            builder.Services.AddTransient<WorkoutDetailsPage>();
            builder.Services.AddTransient<WorkoutsPage>();
            
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}