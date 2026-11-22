namespace CapstoneProjectPUSH.Models
{
    public class Article
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Topic { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string Citation { get; set; } = string.Empty;

        public string CitationUrl { get; set; } = string.Empty;

        // The api calls this "imageUrl"
        [JsonPropertyName("imageUrl")]
        [JsonConverter(typeof(ImageUrlJsonConverter))]
        public string Image { get; set; } = string.Empty;

        public DateTime DateTimePublished { get; set; } = DateTime.Now;

        public int ReadingTime => Math.Max(1, Content.Split(" ", StringSplitOptions.RemoveEmptyEntries).Length / 200);
    }
}