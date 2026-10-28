using CapstoneProjectPUSH.ViewModels.Home;
using CapstoneProjectPUSH.ViewModels.Profile;
using CapstoneProjectPUSH.ViewModels.Trainers;
using CapstoneProjectPUSH.ViewModels.Workouts;
using CapstoneProjectPUSH.ViewModels.Stats;
using CapstoneProjectPUSH.Views.Home;
using CapstoneProjectPUSH.Views.Profile;
using CapstoneProjectPUSH.Views.Trainers;
using CapstoneProjectPUSH.Views.Workouts;
using CapstoneProjectPUSH.Views.Stats;
using CapstoneProjectPUSH.Services;

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
            builder.Services.AddSingleton<StatsService>();
            builder.Services.AddSingleton<GymService>();
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

            // Trainer
            builder.Services.AddTransient<TrainerDetailsViewModel>();
            builder.Services.AddTransient<TrainersViewModel>();

            //Workout plan
            builder.Services.AddTransient<WorkoutsViewModel>();

            // Stats
            builder.Services.AddTransient<StatsViewModel>();
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

            // Trainer
            builder.Services.AddTransient<TrainersPage>();

            //workout plan
            builder.Services.AddTransient<WorkoutsPage>();

            // Stats
            builder.Services.AddTransient<StatsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}