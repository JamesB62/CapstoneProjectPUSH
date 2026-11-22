namespace CapstoneProjectPUSH.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool Available { get; set; } = true;

        public string AvailableText => Available ? "Available" : "Unavailable";

        // The api calls this "profileImageUrl"
        [JsonPropertyName("profileImageUrl")]
        [JsonConverter(typeof(ImageUrlJsonConverter))]
        public string ProfileImage { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string CategoryTitle { get; set; } = string.Empty;

        public int GymId { get; set; }

        public string GymName { get; set; } = string.Empty;

        public string GymAddress { get; set; } = string.Empty;

        public string GymCity { get; set; } = string.Empty;

        public string GymFullAddress => $"{GymAddress}, {GymCity}";

        public string GymLabel => string.IsNullOrEmpty(GymCity) ? GymName : $"{GymName}, {GymCity}";

        public string GymDetailsLabel => string.IsNullOrEmpty(GymAddress) ? GymName : $"{GymName} – {GymFullAddress}";

        public bool IsAtFavouriteGym { get; set; }
    }
}