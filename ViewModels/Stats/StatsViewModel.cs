namespace CapstoneProjectPUSH.ViewModels.Stats
{
    public partial class StatsViewModel : BaseViewModel
    {
        private const double MaxBarHeight = 120;

        // Min and max weights for input validation
        private const double MinWeightKg = 20;
        private const double MaxWeightKg = 400;

        private readonly WeightLogService _weightLogService;
        private readonly WorkoutLogService _workoutLogService;

        public StatsViewModel(UserService userService, WeightLogService weightLogService, WorkoutLogService workoutLogService) : base(userService)
        {
            _weightLogService = weightLogService;
            _workoutLogService = workoutLogService;
        }

        // UI bindings
        [ObservableProperty]
        public partial string SelectedFilter { get; set; } = "Week";

        [ObservableProperty]
        public partial string WorkoutCountText { get; set; } = "0";

        [ObservableProperty]
        public partial string TotalTimeText { get; set; } = "0h 0m";

        [ObservableProperty]
        public partial string AvgDurationText { get; set; } = "0 min";

        [ObservableProperty]
        public partial string StreakText { get; set; } = "0 days";

        [ObservableProperty]
        public partial string LatestWeightText { get; set; } = "No weigh-ins yet";

        // Object used to draw the weight chart
        [ObservableProperty]
        public partial WeightChartDrawable WeightChart { get; set; } = new();

        // Bars for the workout chart
        public ObservableCollection<ChartBar> Bars { get; } = new();

        [RelayCommand]
        private async Task SelectFilterAsync(string filter)
        {
            SelectedFilter = filter;
            await LoadStatsCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task LoadStatsAsync()
        {
            ErrorMessage = string.Empty;

            var user = _userService.CurrentUser;

            // AppShell sends signed-out users to the landing page
            if (user is null)
            {
                return;
            }

            IsBusy = true;

            try
            {
                // Builds buckets for selected filter
                var buckets = BuildBuckets();

                // Gets date range from buckets
                DateTime rangeStart = buckets.First().Start;
                DateTime rangeEnd = buckets.Last().End;

                // Load workout and weight stats
                await LoadWorkoutStatsAsync(user.Id, buckets, rangeStart, rangeEnd);
                await LoadWeightStatsAsync(user.Id, buckets, rangeStart, rangeEnd);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load your stats. Pull down to try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Builds the buckets (one per bar) for the selected filter
        private List<(string Label, DateTime Start, DateTime End)> BuildBuckets()
        {
            // Creates an empty list for the buckets
            var buckets = new List<(string Label, DateTime Start, DateTime End)>();

            // Creates a starting point
            DateTime today = DateTime.Today;

            if (SelectedFilter == "Month")
            {
                // Sets the start date (4 weeks ago)
                DateTime monthStart = today.AddDays(-27);

                // Iterates once for each week
                for (int i = 0; i < 4; i++)
                {
                    // Sets the label, start, and end dates for the bucket, then adds it to the list
                    DateTime bucketStart = monthStart.AddDays(i * 7);
                    buckets.Add((bucketStart.ToString("d MMM"), bucketStart, bucketStart.AddDays(7)));
                }
            }
            else if (SelectedFilter == "Year")
            {
                // Sets the start date (11 months ago, on the first of the month)
                DateTime yearStart = new DateTime(today.Year, today.Month, 1).AddMonths(-11);

                for (int i = 0; i < 12; i++)
                {
                    DateTime bucketStart = yearStart.AddMonths(i);
                    buckets.Add((bucketStart.ToString("MMM"), bucketStart, bucketStart.AddMonths(1)));
                }
            }
            else
            {
                // Sets the start date (6 days ago)
                DateTime weekStart = today.AddDays(-6);

                for (int i = 0; i < 7; i++)
                {
                    DateTime bucketStart = weekStart.AddDays(i);
                    buckets.Add((bucketStart.ToString("ddd"), bucketStart, bucketStart.AddDays(1)));
                }
            }

            return buckets;
        }

        // Builds the workout stats
        private async Task LoadWorkoutStatsAsync(int userId, List<(string Label, DateTime Start, DateTime End)> buckets, DateTime rangeStart, DateTime rangeEnd)
        {
            var allLogs = await _workoutLogService.GetWorkoutLogsAsync(userId);

            var logs = allLogs
                .Where(l => l.DateTimeCompleted >= rangeStart && l.DateTimeCompleted < rangeEnd)
                .ToList();

            // Calculates top stats
            int totalSeconds = logs.Sum(l => l.DurationSeconds);

            WorkoutCountText = logs.Count.ToString();
            TotalTimeText = $"{totalSeconds / 3600}h {totalSeconds % 3600 / 60}m";
            StreakText = $"{CalculateStreak(allLogs)} days";

            if (logs.Count == 0)
            {
                AvgDurationText = "0 min";
            }
            else
            {
                AvgDurationText = $"{totalSeconds / logs.Count / 60} min";
            }

            var bucketMinutes = new List<(string Label, double Minutes)>();
            double maxMinutes = 0;

            // Calculates minutes trained per bucket then saves the max for scaling
            foreach (var bucket in buckets)
            {
                double minutes = logs
                    .Where(l => l.DateTimeCompleted >= bucket.Start && l.DateTimeCompleted < bucket.End)
                    .Sum(l => l.DurationMinutes);

                bucketMinutes.Add((bucket.Label, minutes));

                if (minutes > maxMinutes)
                {
                    maxMinutes = minutes;
                }
            }

            Bars.Clear();

            // Creates a bar for each bucket, scaling the height based on the max minutes
            foreach (var (label, minutes) in bucketMinutes)
            {
                double barHeight;

                if (maxMinutes == 0)
                {
                    barHeight = 2;
                }
                else
                {
                    barHeight = Math.Max(2, minutes / maxMinutes * MaxBarHeight);
                }

                Bars.Add(new ChartBar
                {
                    Label = label,
                    Minutes = minutes,
                    BarHeight = barHeight
                });
            }
        }

        // Builds the weight stats
        private async Task LoadWeightStatsAsync(int userId, List<(string Label, DateTime Start, DateTime End)> buckets, DateTime rangeStart, DateTime rangeEnd)
        {
            var allWeightLogs = await _weightLogService.GetWeightLogsAsync(userId);

            if (allWeightLogs.Count == 0)
            {
                LatestWeightText = "No weigh-ins yet";
            }
            else
            {
                LatestWeightText = $"Latest: {allWeightLogs[0].WeightKg:0.0} kg";
            }

            // Gets the weight logs for the selected range, ordered by date
            var points = allWeightLogs
                .Where(wl => wl.DateTimeLogged >= rangeStart && wl.DateTimeLogged < rangeEnd)
                .OrderBy(wl => wl.DateTimeLogged)
                .Select(wl => new WeightPoint { Date = wl.DateTimeLogged, WeightKg = wl.WeightKg })
                .ToList();

            var labels = buckets.Select(b => b.Label).ToList();

            // Creates a new drawable for the GraphicsView
            WeightChart = new WeightChartDrawable(points, rangeStart, rangeEnd, labels);
        }

        // Add weigh in value
        [RelayCommand]
        private async Task AddWeighInAsync()
        {
            ErrorMessage = string.Empty;

            if (!await RequireLoginAsync())
            {
                return;
            }

            var user = _userService.CurrentUser;

            if (user is null)
            {
                return;
            }

            // Value prompt
            string? result = await Shell.Current.DisplayPromptAsync(
                "Add Weigh-In",
                "Enter today's weight in kg. This replaces any weigh-in already saved today.",
                "Save",
                "Cancel",
                maxLength: 6,
                keyboard: Keyboard.Numeric);

            // Cancelled
            if (result is null)
            {
                return;
            }

            if (!double.TryParse(result.Trim(), out double weightKg))
            {
                ErrorMessage = "Please enter your weight as a number, e.g. 72.5.";
                return;
            }

            if (weightKg < MinWeightKg || weightKg > MaxWeightKg)
            {
                ErrorMessage = $"Please enter a weight between {MinWeightKg} and {MaxWeightKg} kg.";
                return;
            }

            try
            {
                await _weightLogService.SaveWeightForTodayAsync(user.Id, Math.Round(weightKg, 1));
                await LoadStatsCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't save your weigh-in. Please try again.";
            }
        }

        // Calculates day streak, counting back from today (or yesterday if none today yet)
        private static int CalculateStreak(List<WorkoutLog> logs)
        {
            // Saves only the date and removes any duplicates
            var days = logs.Select(l => l.DateTimeCompleted.Date).ToHashSet();
            DateTime day = DateTime.Today;

            if (!days.Contains(day))
            {
                day = day.AddDays(-1);
            }

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