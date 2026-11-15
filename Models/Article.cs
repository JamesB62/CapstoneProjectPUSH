namespace CapstoneProjectPUSH.Models
{
    [Table("Articles")]
    public class Article
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [NotNull, MaxLength(50)]
        public string Topic { get; set; } = string.Empty;

        [NotNull, MaxLength(5000)]
        public string Content { get; set; } = string.Empty;

        [NotNull, MaxLength(300)]
        public string Citation { get; set; } = string.Empty;

        [NotNull, MaxLength(300)]
        public string CitationUrl { get; set; } = string.Empty;

        // Swap to ImageUrl when using remote database
        public string Image { get; set; } = string.Empty;

        public DateTime DateTimePublished { get; set; } = DateTime.Now;

        [Ignore]
        public int ReadingTime => Math.Max(1, Content.Split(" ", StringSplitOptions.RemoveEmptyEntries).Length / 200);
    }
}