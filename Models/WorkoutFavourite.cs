namespace CapstoneProjectPUSH.Models
{
    // A workout a user has favourited (stored per user, so it also works for other people's workouts)
    [Table("WorkoutFavourites")]
    public class WorkoutFavourite
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime DateTimeAdded { get; set; } = DateTime.Now;

        // A user can only favourite the same workout once
        [Indexed("WorkoutFavouriteIndex", 1, Unique = true)]
        public int UserId { get; set; }

        [Indexed("WorkoutFavouriteIndex", 2, Unique = true)]
        public int WorkoutId { get; set; }
    }
}