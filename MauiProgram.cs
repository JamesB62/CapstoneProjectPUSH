using CapstoneProjectPUSH.ViewModels.Profile;
using CapstoneProjectPUSH.Views.Profile;

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
            builder.Services.AddSingleton<TrainerService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<WorkoutService>();

            // ======================== ViewModels ========================

            // Auth
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();

            // Base
            builder.Services.AddTransient<LandingViewModel>();

            // Profile
            builder.Services.AddTransient<ProfileViewModel>();

            // Trainer
            builder.Services.AddTransient<TrainerViewModel>();

            // ========================== Views ===========================

            // Auth
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();

            // Base
            builder.Services.AddTransient<LandingPage>();
            builder.Services.AddTransient<PrivacyPage>();

            // Profile
            builder.Services.AddTransient<ProfilePage>();

            // Trainer
            builder.Services.AddTransient<TrainerPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}