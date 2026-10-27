namespace CapstoneProjectPUSH.Models
{
    [Table("Workouts")]
    public class Workout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Indexed]
        public int UserId { get; set; }

        // Display name property needed?

        [Indexed]
        public int CategoryId { get; set; }

        [Ignore]
        public string CategoryName { get; set; } = string.Empty;

        public DateTime DateTimeCreated { get; set; } = DateTime.Now;
    }
}