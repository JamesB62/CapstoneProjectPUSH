namespace CapstoneProjectPUSH.Models
{
    [Table("Notes")]
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [NotNull, MaxLength(5000)]
        public string Content { get; set; } = string.Empty;

        public DateTime DateTimeAdded { get; set; } = DateTime.Now;

        [Indexed]
        public int UserId { get; set; }

        [Ignore]
        public string UserFullName { get; set; } = string.Empty;

        [Indexed]
        public int WorkoutId { get; set; }

        [Ignore]
        public string WorkoutName { get; set; } = string.Empty;
    }
}