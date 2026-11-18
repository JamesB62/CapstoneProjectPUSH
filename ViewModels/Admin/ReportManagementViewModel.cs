namespace CapstoneProjectPUSH.ViewModels.Admin
{
    public partial class ReportManagementViewModel : BaseViewModel
    {
        private readonly ReportService _reportService;

        public ReportManagementViewModel(ReportService reportService, UserService userService) : base(userService)
        {
            _reportService = reportService;
        }

        public ObservableCollection<Report> Reports { get; } = new();

        private List<Report> _allReports = new();

        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool ShowPendingOnly { get; set; } = true;

        [ObservableProperty]
        public partial Report? SelectedReport { get; set; }

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
                _allReports = await _reportService.GetReportsAsync();

                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ErrorMessage = "Couldn't load reports. Pull down to try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        partial void OnShowPendingOnlyChanged(bool value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string term = string.Empty;

            if (SearchText is not null)
            {
                term = SearchText.Trim();
            }

            IEnumerable<Report> filtered = _allReports;

            if (ShowPendingOnly)
            {
                filtered = filtered.Where(r => !r.IsResolved);
            }

            if (term.Length > 0)
            {
                filtered = filtered.Where(r => r.WorkoutTitle.Contains(term, StringComparison.OrdinalIgnoreCase)
                                            || r.UserDisplayName.Contains(term, StringComparison.OrdinalIgnoreCase)
                                            || r.Reason.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            var results = filtered.ToList();

            Reports.Clear();

            foreach (var report in results)
            {
                Reports.Add(report);
            }

            if (results.Count == 1)
            {
                StatusMessage = "Showing 1 report.";
            }
            else
            {
                StatusMessage = $"Showing {results.Count} reports.";
            }
        }

        partial void OnSelectedReportChanged(Report? value)
        {
            if (value is null)
            {
                return;
            }

            ViewReportCommand.Execute(value);
            SelectedReport = null;
        }

        [RelayCommand]
        private async Task ViewReportAsync(Report? report)
        {
            if (report is null)
            {
                return;
            }

            await NavigateAsync($"{nameof(ReportDetailsPage)}?ReportId={report.Id}");
        }
    }
}