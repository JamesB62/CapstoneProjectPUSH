namespace CapstoneProjectPUSH.Models
{
    [Table("Report")]
    public class Report
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Reported workout
        [Indexed]
        public int WorkoutId { get; set; }

        [Ignore]
        public string WorkoutTitle { get; set; } = string.Empty;

        [Ignore]
        public bool WorkoutExists { get; set; }

        [NotNull, MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        public DateTime DateTimeCreated { get; set; } = DateTime.Now;

        // Reporter info
        [Indexed]
        public int UserId { get; set; }

        [Ignore]
        public string UserDisplayName { get; set; } = string.Empty;

        [Indexed]
        public bool IsResolved { get; set; }

        [Ignore]
        public string StatusLabel => IsResolved ? "Resolved" : "Pending";

        public DateTime? DateTimeResolved { get; set; }

        [Ignore]
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