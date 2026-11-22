namespace CapstoneProjectPUSH.Models
{
    // A workout a user has favourited (stored per user, so it also works for other people's workouts)
    public class WorkoutFavourite
    {
        public int Id { get; set; }

        public DateTime DateTimeAdded { get; set; } = DateTime.Now;

        // A user can only favourite the same workout once
        public int UserId { get; set; }

        public int WorkoutId { get; set; }
    }
}