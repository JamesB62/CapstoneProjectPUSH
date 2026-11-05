using System.Collections.ObjectModel;
using CapstoneProjectPUSH.Models;
using CapstoneProjectPUSH.Services;
using CapstoneProjectPUSH.ViewModels.Workouts;

namespace CapstoneProjectPUSH.ViewModels.Stats
{
    // One bar on the stats chart (UI only, not stored in the DB)
    public class ChartBar
    {
        public string Label { get; set; } = string.Empty;
        public double Minutes { get; set; }
        public double BarHeight { get; set; }
    }


    public class ChartLine
    {
        public string Label { get; set; } = string.Empty;
        public double Weight { get; set; }
        public double LineHeight { get; set; }
    }


    public partial class StatsViewModel : BaseViewModel
    {
        private const double MaxBarHeight = 120;

        private readonly UserService _userService;
        private readonly WorkoutLogService _workoutLogService;

        [ObservableProperty]
        public partial string SelectedFilter { get; set; }

        [ObservableProperty]
        public partial string WorkoutCountText { get; set; }

        [ObservableProperty]
        public partial string TotalTimeText { get; set; }

        [ObservableProperty]
        public partial string AvgDurationText { get; set; }

        [ObservableProperty]
        public partial string StreakText { get; set; }

        public ObservableCollection<ChartBar> Bars { get; } = new();

        public StatsViewModel(UserService userService, WorkoutLogService workoutLogService)
        {
            _userService = userService;
            _workoutLogService = workoutLogService;

            SelectedFilter = "Week";
            WorkoutCountText = "0";
            TotalTimeText = "0h 0m";
            AvgDurationText = "0 min";
            StreakText = "0 days";
        }

        public StatsViewModel(UserService userService, WorkoutLogService workoutLogService, WeightLogService WeightLogService)
        {
            _userService = userService;
            _workoutLogService = workoutLogService;
            SelectedFilter = "Week";
            WorkoutCountText = "0";
            TotalTimeText = "0h 0m";
            AvgDurationText = "0 min";
            StreakText = "0 days";
            // Load weight logs and create chart lines
            // LoadWeightLogs(WeightLogService);
        }



        


        // Generates SelectFilterCommand
        [RelayCommand]
        private async Task SelectFilter(string filter)
        {
            SelectedFilter = filter;
            await Load();
        }

        // Generates LoadCommand
        [RelayCommand]
        private async Task Load()
        {
            // Stats belong to whoever is logged in
            var user = _userService.CurrentUser;
            if (user is null)
            {
                Bars.Clear();
                return;
            }

            var allLogs = await _workoutLogService.GetWorkoutLogsAsync(user.Id);
            var today = DateTime.Today;

            // Build the buckets (one per bar) for the selected filter
            var buckets = new List<(string Label, DateTime Start, DateTime End)>();

            switch (SelectedFilter)
            {
                case "Month": // last 4 weeks, one bar per week
                    var monthStart = today.AddDays(-27);
                    for (int i = 0; i < 4; i++)
                    {
                        var s = monthStart.AddDays(i * 7);
                        buckets.Add((s.ToString("d MMM"), s, s.AddDays(7)));
                    }
                    break;

                case "Year": // last 12 months, one bar per month
                    var yearStart = new DateTime(today.Year, today.Month, 1).AddMonths(-11);
                    for (int i = 0; i < 12; i++)
                    {
                        var s = yearStart.AddMonths(i);
                        buckets.Add((s.ToString("MMM"), s, s.AddMonths(1)));
                    }
                    break;

                default: // Week: last 7 days, one bar per day
                    var weekStart = today.AddDays(-6);
                    for (int i = 0; i < 7; i++)
                    {
                        var s = weekStart.AddDays(i);
                        buckets.Add((s.ToString("ddd"), s, s.AddDays(1)));
                    }
                    break;
            }

            var rangeStart = buckets.First().Start;
            var rangeEnd = buckets.Last().End;
            var logs = allLogs
                .Where(l => l.DateTimeCompleted >= rangeStart && l.DateTimeCompleted < rangeEnd)
                .ToList();

            // Top stats
            int totalSeconds = logs.Sum(l => l.DurationSeconds);
            WorkoutCountText = logs.Count.ToString();
            TotalTimeText = $"{totalSeconds / 3600}h {totalSeconds % 3600 / 60}m";
            AvgDurationText = logs.Count == 0 ? "0 min" : $"{totalSeconds / logs.Count / 60} min";
            StreakText = $"{CalculateStreak(allLogs)} days";

            // Chart bars (minutes per bucket)
            var minutes = buckets
                .Select(b => (b.Label, Minutes: logs
                    .Where(l => l.DateTimeCompleted >= b.Start && l.DateTimeCompleted < b.End)
                    .Sum(l => l.DurationMinutes)))
                .ToList();

            double max = minutes.Max(m => m.Minutes);

            Bars.Clear();
            foreach (var (label, mins) in minutes)
            {
                Bars.Add(new ChartBar
                {
                    Label = label,
                    Minutes = mins,
                    BarHeight = max == 0 ? 2 : Math.Max(2, mins / max * MaxBarHeight)
                });
            }
        }

        // Consecutive days with a workout, counting back from today (or yesterday if none today yet)
        private static int CalculateStreak(List<WorkoutLog> logs)
        {
            var days = logs.Select(l => l.DateTimeCompleted.Date).ToHashSet();
            var day = DateTime.Today;
            if (!days.Contains(day))
                day = day.AddDays(-1);

            int streak = 0;
            while (days.Contains(day))
            {
                streak++;
                day = day.AddDays(-1);
            }
            return streak;
        }

        




    }

    



}