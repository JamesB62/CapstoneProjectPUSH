using CapstoneProjectPUSH.Views.Exercises;
using CapstoneProjectPUSH.ViewModels.Admins;
using CapstoneProjectPUSH.Views.Admin;

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
            builder.Services.AddSingleton<ExerciseService>();

            // ======================== ViewModels ========================

            // Auth
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();

            // Base
            builder.Services.AddTransient<LandingViewModel>();

            // Exercises
            builder.Services.AddTransient<ExercisesViewModel>();
            builder.Services.AddTransient<ExerciseDetailsViewModel>();

            // Home
            builder.Services.AddTransient<HomeViewModel>();

            // Profile
            builder.Services.AddTransient<UpdateProfileViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();

            // Stats
            builder.Services.AddTransient<StatsViewModel>();

            // Trainers
            builder.Services.AddTransient<TrainerDetailsViewModel>();
            builder.Services.AddTransient<TrainersViewModel>();

            // Workouts
            builder.Services.AddTransient<WorkoutsViewModel>();
            //workout detail
            builder.Services.AddTransient<WorkoutsDetailViewModel>();

            //Exercise
            builder.Services.AddTransient<ExercisesViewModel>();
            builder.Services.AddTransient<SelectExercisesViewModel>();

            // Admin
            builder.Services.AddTransient<AdminDashboardViewModel>();

            // ========================== Views ===========================

            // Auth
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();

            // Base
            builder.Services.AddTransient<LandingPage>();
            builder.Services.AddTransient<PrivacyPage>();

            // Exercises
            builder.Services.AddTransient<ExercisesPage>();
            builder.Services.AddTransient<ExerciseDetailsPage>();

            // Home
            builder.Services.AddTransient<HomePage>();

            // Profile
            builder.Services.AddTransient<UpdateProfilePage>();
            builder.Services.AddTransient<ProfilePage>();
          
            // Stats
            builder.Services.AddTransient<StatsPage>();
          
            // Trainers
            builder.Services.AddTransient<TrainerDetailsPage>();
            builder.Services.AddTransient<TrainersPage>();
          
            // Workout
            builder.Services.AddTransient<WorkoutsPage>();
            //workout detail 
            builder.Services.AddTransient<WorkoutDetailPage>();

            //Exercises 
            builder.Services.AddTransient<SelectExercisesPage>();

            // Admin
            builder.Services.AddTransient<AdminDashboard>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}