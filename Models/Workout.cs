namespace CapstoneProjectPUSH.Models
{
    public class Workout
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public int ExerciseCount { get; set; }

        public int SetCount { get; set; }

        public int TotalMinutes { get; set; }

        public bool HasSets => SetCount > 0;

        public bool HasMinutes => TotalMinutes > 0;

        public DateTime DateTimeCreated { get; set; } = DateTime.Now;

        public bool IsPrivate { get; set; } = true;

        public string VisibilityText => IsPrivate ? "Private" : "Public";

        // Set per user by WorkoutService (not stored on the workout)
        public bool IsFavourite { get; set; }

        public string FavouriteIcon => IsFavourite ? "★" : "☆";

        public int UserId { get; set; }

        public string UserDisplayName { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string CategoryTitle { get; set; } = string.Empty;
    }
}