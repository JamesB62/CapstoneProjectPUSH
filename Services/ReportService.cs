namespace CapstoneProjectPUSH.Services
{
    public class ReportService
    {
        private readonly CategoryService _categoryService;
        private readonly DatabaseService _databaseService;
        private readonly UserService _userService;

        public ReportService(CategoryService categoryService, DatabaseService databaseService, UserService userService)
        {
            _categoryService = categoryService;
            _databaseService = databaseService;
            _userService = userService;
        }

        // Get pending report count
        public async Task<int> GetPendingReportCountAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Report>().Where(r => !r.IsResolved).CountAsync();
        }

        // Get all reports
        public async Task<List<Report>> GetReportsAsync()
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return new List<Report>();
            }

            var db = await _databaseService.GetConnectionAsync();

            var reports = await db.Table<Report>()
                .OrderByDescending(r => r.DateTimeCreated)
                .ToListAsync();

            await FillDetailsAsync(reports);

            return reports;
        }

        // Get report by Id
        public async Task<Report?> GetReportByIdAsync(int reportId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return null;
            }

            var db = await _databaseService.GetConnectionAsync();

            var report = await db.Table<Report>().Where(r => r.Id == reportId).FirstOrDefaultAsync();

            if (report is null)
            {
                return null;
            }

            await FillDetailsAsync(new List<Report> { report });

            return report;
        }

        // Add
        public async Task<(bool Success, string Message)> AddReportAsync(int workoutId, string reason)
        {
            var user = _userService.CurrentUser;

            if (user is null || user.IsGuest)
            {
                return (false, "You need to be logged in to report a workout.");
            }

            reason = (reason ?? string.Empty).Trim();

            if (reason.Length == 0)
            {
                return (false, "Please enter a reason for the report.");
            }

            if (reason.Length > 500)
            {
                return (false, "Reason must be 500 characters or fewer.");
            }

            var db = await _databaseService.GetConnectionAsync();

            var workout = await db.Table<Workout>().Where(w => w.Id == workoutId).FirstOrDefaultAsync();

            if (workout is null || workout.IsPrivate)
            {
                return (false, "This workout could not be found.");
            }

            if (workout.UserId == user.Id)
            {
                return (false, "You can't report your own workout.");
            }

            var userId = user.Id;

            var alreadyReported = await db.Table<Report>()
                .Where(r => r.WorkoutId == workoutId && r.UserId == userId && !r.IsResolved)
                .CountAsync() > 0;

            if (alreadyReported)
            {
                return (false, "You've already reported this workout.");
            }

            await db.InsertAsync(new Report
            {
                WorkoutId = workoutId,
                UserId = userId,
                Reason = reason
            });

            return (true, "An admin will review this workout.");
        }

        // Dismiss report
        public async Task<(bool Success, string Message)> DismissReportAsync(int reportId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can manage reports.");
            }

            var db = await _databaseService.GetConnectionAsync();

            var report = await db.Table<Report>().Where(r => r.Id == reportId).FirstOrDefaultAsync();

            if (report is null)
            {
                return (false, "Report could not be found.");
            }

            if (report.IsResolved)
            {
                return (false, "This report has already been resolved.");
            }

            report.IsResolved = true;
            report.DateTimeResolved = DateTime.Now;

            await db.UpdateAsync(report);

            return (true, "Report dismissed.");
        }

        // Remove the reported workout
        public async Task<(bool Success, string Message)> RemoveContentAsync(int reportId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return (false, "Only admins can manage reports.");
            }

            var db = await _databaseService.GetConnectionAsync();

            var report = await db.Table<Report>().Where(r => r.Id == reportId).FirstOrDefaultAsync();

            if (report is null)
            {
                return (false, "Report could not be found.");
            }

            if (report.IsResolved)
            {
                return (false, "This report has already been resolved.");
            }

            var workoutId = report.WorkoutId;
            var resolvedAt = DateTime.Now;

            // All or nothing, so the workout and its reports stay in step
            await db.RunInTransactionAsync(conn =>
            {
                conn.Table<WorkoutExercise>().Delete(we => we.WorkoutId == workoutId);
                conn.Table<Note>().Delete(n => n.WorkoutId == workoutId);
                conn.Table<Workout>().Delete(w => w.Id == workoutId);

                var pendingReports = conn.Table<Report>()
                    .Where(r => r.WorkoutId == workoutId && !r.IsResolved)
                    .ToList();

                foreach (var pending in pendingReports)
                {
                    pending.IsResolved = true;
                    pending.DateTimeResolved = resolvedAt;
                    conn.Update(pending);
                }
            });

            return (true, "Workout removed.");
        }

        // Fill details
        private async Task FillDetailsAsync(List<Report> reports)
        {
            var db = await _databaseService.GetConnectionAsync();

            var workouts = await db.Table<Workout>().ToListAsync();
            var displayNames = await _userService.GetDisplayNamesByIdAsync();

            foreach (var report in reports)
            {
                var workout = workouts.FirstOrDefault(w => w.Id == report.WorkoutId);

                if (workout is null)
                {
                    report.WorkoutTitle = "Removed workout";
                    report.WorkoutExists = false;
                }
                else
                {
                    report.WorkoutTitle = workout.Title;
                    report.WorkoutExists = true;
                }

                if (displayNames.TryGetValue(report.UserId, out var displayName))
                {
                    report.UserDisplayName = displayName;
                }
                else
                {
                    report.UserDisplayName = "Deleted user";
                }
            }
        }

        // Get the reported workout with its details
        public async Task<Workout?> GetReportedWorkoutAsync(int workoutId)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                return null;
            }

            var db = await _databaseService.GetConnectionAsync();

            var workout = await db.Table<Workout>().Where(w => w.Id == workoutId).FirstOrDefaultAsync();

            if (workout is null)
            {
                return null;
            }

            var categoryTitles = await _categoryService.GetCategoryTitlesAsync();
            workout.CategoryTitle = categoryTitles.GetValueOrDefault(workout.CategoryId, "Uncategorised");

            var displayNames = await _userService.GetDisplayNamesByIdAsync();
            workout.UserDisplayName = displayNames.GetValueOrDefault(workout.UserId, "Deleted user");

            var items = await db.Table<WorkoutExercise>().Where(we => we.WorkoutId == workoutId).ToListAsync();

            workout.ExerciseCount = items.Count;
            workout.SetCount = items.Sum(i => i.Sets);
            workout.TotalMinutes = items.Sum(i => i.DurationMinutes);

            return workout;
        }
    }
}