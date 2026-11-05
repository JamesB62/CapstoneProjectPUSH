using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CapstoneProjectPUSH.Models;
using CapstoneProjectPUSH.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CapstoneProjectPUSH.ViewModels.Admins
{
    public class AdminDashboardViewModel : INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private readonly TrainerService _trainerService;
        private readonly GymService _gymService;
        private readonly ExerciseService _exerciseService;
        private readonly ArticleService _articleService;

        private bool _isSignedIn;
        private bool _isBusy;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _statusMessage = string.Empty;
        private string _adminName = string.Empty;
        private int _totalUsers;
        private int _totalTrainers;
        private int _availableTrainers;
        private int _totalGyms;
        private int _totalExercises;
        private int _totalArticles;

        public AdminDashboardViewModel()
        {
            // The page creates this view model with "new", so resolve services from the app container
            var services = IPlatformApplication.Current?.Services
                ?? throw new InvalidOperationException("The service provider is not available.");

            _userService = services.GetRequiredService<UserService>();
            _trainerService = services.GetRequiredService<TrainerService>();
            _gymService = services.GetRequiredService<GymService>();
            _exerciseService = services.GetRequiredService<ExerciseService>();
            _articleService = services.GetRequiredService<ArticleService>();


            Trainers.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasNoTrainers));

            SignInCommand = new Command(async () => await SignInAsync());
            SignOutCommand = new Command(SignOut);
            RefreshCommand = new Command(async () => await RefreshAsync());
            ToggleAvailabilityCommand = new Command<Trainer>(async t => await ToggleAvailabilityAsync(t));
            DeleteTrainerCommand = new Command<Trainer>(async t => await DeleteTrainerAsync(t));
            NavigateToTrainersCommand = new Command(async () => await NavigateToTrainersAsync());
            NavigateToHomeCommand = new Command(async () => await NavigateToHomeAsync());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        // Sign in state
        public bool IsSignedIn
        {
            get => _isSignedIn;
            private set
            {
                if (SetProperty(ref _isSignedIn, value))
                {
                    OnPropertyChanged(nameof(IsSignedOut));
                }
            }
        }

        public bool IsSignedOut => !IsSignedIn;

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    OnPropertyChanged(nameof(IsNotBusy));
                }
            }
        }

        public bool IsNotBusy => !IsBusy;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set
            {
                if (SetProperty(ref _statusMessage, value))
                {
                    OnPropertyChanged(nameof(HasStatusMessage));
                }
            }
        }

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        public string AdminName
        {
            get => _adminName;
            private set => SetProperty(ref _adminName, value);
        }

        // Dashboard data
        public int TotalUsers { get => _totalUsers; private set => SetProperty(ref _totalUsers, value); }
        public int TotalTrainers { get => _totalTrainers; private set => SetProperty(ref _totalTrainers, value); }
        public int AvailableTrainers { get => _availableTrainers; private set => SetProperty(ref _availableTrainers, value); }
        public int TotalGyms { get => _totalGyms; private set => SetProperty(ref _totalGyms, value); }
        public int TotalExercises { get => _totalExercises; private set => SetProperty(ref _totalExercises, value); }
        public int TotalArticles { get => _totalArticles; private set => SetProperty(ref _totalArticles, value); }


        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

        public bool HasNoUsers => Users.Count == 0;

        public bool HasUsers => Users.Count == 0;

        public ObservableCollection<Trainer> Trainers { get; } = new();

        public bool HasNoTrainers => Trainers.Count == 0;

        public bool HasTrainers => Trainers.Count > 0;

        // Commands
        public ICommand SignInCommand { get; }
        public ICommand SignOutCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleAvailabilityCommand { get; }
        public ICommand DeleteTrainerCommand { get; }
        public ICommand NavigateToTrainersCommand { get; }
        public ICommand NavigateToHomeCommand { get; }

        // Called from the page's OnAppearing
        public async Task LoadAsync()
        {
            // Covers an admin who is already signed in (e.g. returning to the page)
            if (IsAdmin(_userService.CurrentUser))
            {
                AdminName = _userService.CurrentUser!.DisplayName;
                IsSignedIn = true;
                await RefreshAsync();
            }
            else
            {
                IsSignedIn = false;
            }
        }

        public async Task NavigateToTrainersAsync()
        {
            await Shell.Current.GoToAsync("//TrainersPage");
        }

        public async Task NavigateToUsersDetailsAsync()
        {
            await Shell.Current.GoToAsync("//UsersDetailsPage");
        }


        public async Task NavigateToHomeAsync()
        {
            await Shell.Current.GoToAsync("//HomePage");
        }

        private async Task SignInAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            StatusMessage = string.Empty;

            try
            {
                var result = await _userService.LoginAsync(Email, Password, requireAdmin: true);

                if (!result.Success)
                {
                    StatusMessage = result.Message;
                    return;
                }

                Email = string.Empty;
                Password = string.Empty;
                AdminName = result.User?.DisplayName ?? "Admin";
                IsSignedIn = true;

                await RefreshAsync();
            }
            catch (Exception)
            {
                StatusMessage = "Something went wrong while signing in. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SignOut()
        {
            _userService.Logout();

            Trainers.Clear();
            AdminName = string.Empty;
            StatusMessage = string.Empty;
            IsSignedIn = false;
        }

        private async Task RefreshAsync()
        {
            if (!RequireAdmin())
            {
                return;
            }

            try
            {
                var trainers = await _trainerService.GetTrainersAsync();
                var gyms = await _gymService.GetGymsAsync();
                var exercises = await _exerciseService.GetExercisesAsync();
                var articles = await _articleService.GetArticlesAsync();

                TotalUsers = await _userService.GetUserCountAsync();
                TotalTrainers = trainers.Count;
                AvailableTrainers = trainers.Count(t => t.Available);
                TotalGyms = gyms.Count;
                TotalExercises = exercises.Count;
                TotalArticles = articles.Count;

                Trainers.Clear();
                foreach (var trainer in trainers)
                {
                    Trainers.Add(trainer);
                }

                StatusMessage = string.Empty;
            }
            catch (Exception)
            {
                StatusMessage = "Could not load dashboard data.";
            }
        }

        private async Task ToggleAvailabilityAsync(Trainer? trainer)
        {
            if (trainer is null || !RequireAdmin())
            {
                return;
            }

            try
            {
                trainer.Available = !trainer.Available;
                await _trainerService.UpdateTrainerAsync(trainer);
                await RefreshAsync();
            }
            catch (Exception)
            {
                StatusMessage = "Could not update the trainer.";
            }
        }

        private async Task DeleteTrainerAsync(Trainer? trainer)
        {
            if (trainer is null || !RequireAdmin())
            {
                return;
            }

            var confirmed = await Shell.Current.DisplayAlertAsync(
                "Delete trainer",
                $"Delete {trainer.FullName}? This cannot be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            try
            {
                await _trainerService.DeleteTrainerAsync(trainer.Id);
                await RefreshAsync();
            }
            catch (Exception)
            {
                StatusMessage = "Could not delete the trainer.";
            }
        }

        // Re-checked before every admin action so a signed-out session can't keep acting
        private bool RequireAdmin()
        {
            if (IsAdmin(_userService.CurrentUser))
            {
                return true;
            }

            IsSignedIn = false;
            StatusMessage = "Your admin session has ended. Please sign in again.";
            return false;
        }

        private static bool IsAdmin(User? user) => user is { IsAdmin: true };

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(name);
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}