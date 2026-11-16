namespace CapstoneProjectPUSH.ViewModels.Admin
{
    [QueryProperty(nameof(ReportId), "ReportId")]
    public partial class ReportDetailsViewModel : BaseViewModel
    {
        private readonly ReportService _reportService;

        public ReportDetailsViewModel(ReportService reportService, UserService userService) : base(userService)
        {
            _reportService = reportService;
        }

        [ObservableProperty]
        public partial int ReportId { get; set; }

        [ObservableProperty]
        public partial Report? Report { get; set; }

        [ObservableProperty]
        public partial bool CanResolve { get; set; }

        [ObservableProperty]
        public partial bool CanViewWorkout { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNoWorkout))]
        public partial Workout? Workout { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNoWorkout))]
        public partial bool HasWorkout { get; set; }

        public bool HasNoWorkout => !HasWorkout;

        [ObservableProperty]
        public partial string PrivacyText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string ResolvedText { get; set; } = string.Empty;

        partial void OnReportIdChanged(int value)
        {
            if (value <= 0)
            {
                return;
            }

            LoadCommand.Execute(null);
        }

        public void Reload()
        {
            if (ReportId <= 0)
            {
                return;
            }

            LoadCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            ErrorMessage = string.Empty;

            if (!await RequireAdminAsync())
            {
                return;
            }

            IsBusy = true;

            try
            {
                Report = await _reportService.GetReportByIdAsync(ReportId);

                if (Report is not null)
                {
                    CanResolve = !Report.IsResolved;

                    if (Report.IsResolved && Report.DateTimeResolved is not null)
                    {
                        ResolvedText = $"Resolved on {Report.DateTimeResolved.Value:d MMM yyyy}";
                    }
                    else
                    {
                        ResolvedText = string.Empty;
                    }

                    if (Report.WorkoutExists)
                    {
                        Workout = await _reportService.GetReportedWorkoutAsync(Report.WorkoutId);
                    }
                    else
                    {
                        Workout = null;
                    }

                    if (Workout is not null)
                    {
                        HasWorkout = true;

                        if (Workout.IsPrivate)
                        {
                            PrivacyText = "Private";
                        }
                        else
                        {
                            PrivacyText = "Shared";
                        }

                        CanViewWorkout = !Workout.IsPrivate;
                    }
                    else
                    {
                        HasWorkout = false;
                        PrivacyText = string.Empty;
                        CanViewWorkout = false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }

            if (Report is null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Report Not Found",
                    "This report could not be loaded.",
                    "OK");

                await NavigateAsync("..");
            }
        }

        [RelayCommand]
        private async Task ViewWorkoutAsync()
        {
            if (Workout is null || !CanViewWorkout)
            {
                return;
            }

            await NavigateAsync($"{nameof(WorkoutDetailsPage)}?WorkoutId={Workout.Id}");
        }

        [RelayCommand]
        private async Task DismissAsync()
        {
            if (Report is null || !CanResolve || IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                var result = await _reportService.DismissReportAsync(Report.Id);

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                await Shell.Current.DisplayAlertAsync("Report Dismissed", "The report has been dismissed.", "OK");

                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't dismiss the report. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task RemoveContentAsync()
        {
            if (Report is null || !CanResolve || IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;

            bool confirmed = await Shell.Current.DisplayAlertAsync(
                "Remove Workout",
                "Remove this workout? This can't be undone.",
                "Remove",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            string title = Report.WorkoutTitle;

            IsBusy = true;

            try
            {
                var result = await _reportService.RemoveContentAsync(Report.Id);

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                await Shell.Current.DisplayAlertAsync("Content Removed", "The workout has been removed.", "OK");

                await NavigateAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't remove the workout. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}