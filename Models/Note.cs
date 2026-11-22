namespace CapstoneProjectPUSH.Models
{
    public class Note
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime DateTimeCreated { get; set; } = DateTime.Now;

        public int UserId { get; set; }

        public string UserDisplayName { get; set; } = string.Empty;

        public int WorkoutId { get; set; }

        public string WorkoutTitle { get; set; } = string.Empty;
    }
}