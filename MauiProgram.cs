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
            builder.Services.AddSingleton<StatsService>();
            builder.Services.AddSingleton<TrainerService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<WorkoutService>();
            builder.Services.AddSingleton<WorkoutLogService>();

            // ======================== ViewModels ========================

            // Auth
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();

            // Base
            builder.Services.AddTransient<LandingViewModel>();

            // Home
            builder.Services.AddTransient<HomeViewModel>();

            // Profile
            builder.Services.AddTransient<ProfileViewModel>();

            // Stats
            builder.Services.AddTransient<StatsViewModel>();

            // Trainer
            builder.Services.AddTransient<TrainerDetailsViewModel>();
            builder.Services.AddTransient<TrainersViewModel>();

            // Workouts
            builder.Services.AddTransient<WorkoutsViewModel>();

            // ========================== Views ===========================

            // Auth
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();

            // Base
            builder.Services.AddTransient<LandingPage>();
            builder.Services.AddTransient<PrivacyPage>();

            // Home
            builder.Services.AddTransient<HomePage>();

            // Profile
            builder.Services.AddTransient<ProfilePage>();
          
            // Stats
            builder.Services.AddTransient<StatsPage>();
          
            // Trainer
            builder.Services.AddTransient<TrainerDetailsPage>();
            builder.Services.AddTransient<TrainersPage>();
          
            // Workout
            builder.Services.AddTransient<WorkoutsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}