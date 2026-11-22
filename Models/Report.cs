namespace CapstoneProjectPUSH.Models
{
    public class Report
    {
        public int Id { get; set; }

        // Reported workout
        public int WorkoutId { get; set; }

        public string WorkoutTitle { get; set; } = string.Empty;

        public bool WorkoutExists { get; set; }

        public string Reason { get; set; } = string.Empty;

        public DateTime DateTimeCreated { get; set; } = DateTime.Now;

        // Reporter info
        public int UserId { get; set; }

        public string UserDisplayName { get; set; } = string.Empty;

        public bool IsResolved { get; set; }

        public string StatusLabel => IsResolved ? "Resolved" : "Pending";

        public DateTime? DateTimeResolved { get; set; }

        public string ResolvedText
        {
            get
            {
                if (DateTimeResolved is null)
                {
                    return string.Empty;
                }

                return $"Resolved on {DateTimeResolved.Value:d MMM yyyy}";
            }
        }
    }
}