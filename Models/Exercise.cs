namespace CapstoneProjectPUSH.Models
{
    [Table("Exercises")]
    public class Exercise
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [NotNull, MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public int Difficulty { get; set; } = 1;

        [Ignore]
        public string DifficultyLabel => Difficulty switch
        {
            1 => "Beginner",
            2 => "Easy",
            3 => "Intermediate",
            4 => "Hard",
            5 => "Advanced",
            _ => "Unknown"
        };

        // Swap to ImageUrl when using remote database
        public string Image { get; set; } = string.Empty;

        [Indexed]
        public int CategoryId { get; set; }

        [Ignore]
        public string CategoryTitle { get; set; } = string.Empty;
    }
}