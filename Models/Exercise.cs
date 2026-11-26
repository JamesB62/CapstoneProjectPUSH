namespace CapstoneProjectPUSH.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsTimed { get; set; } = false;

        public int Difficulty { get; set; } = 1;

        public string DifficultyLabel => Difficulty switch
        {
            1 => "Beginner",
            2 => "Easy",
            3 => "Intermediate",
            4 => "Hard",
            5 => "Advanced",
            _ => "Unknown"
        };

        // The api calls this "imageUrl"
        [JsonPropertyName("imageUrl")]
        [JsonConverter(typeof(ImageUrlJsonConverter))]
        public string Image { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string CategoryTitle { get; set; } = string.Empty;
    }
}