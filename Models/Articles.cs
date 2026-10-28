namespace CapstoneProjectPUSH.Models
{
    [Table("Articles")]
    public class Article
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [NotNull, MaxLength(100)]
        public string Subtitle { get; set; } = string.Empty;

        // May need to increase MaxLength value
        [NotNull, MaxLength(5000)]
        public string Content { get; set; } = string.Empty;

        [NotNull, MaxLength(300)]
        public string Citation { get; set; } = string.Empty;

        public DateTime DateTimeAdded { get; set; } = DateTime.Now;

        // Approx. word count divided by 200
        [Ignore]
        public int ReadingTime => Math.Max(1, Content.Split(" ").Length / 200);

        [Indexed]
        public int CategoryId { get; set; }

        [Ignore]
        public string CategoryName { get; set; } = string.Empty;


    }
}