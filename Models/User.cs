namespace CapstoneProjectPUSH.Models
{
    public class User
    {
        public int Id { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime DateTimeCreated { get; set; } = DateTime.Now;

        // The api calls this "profileImageUrl"
        [JsonPropertyName("profileImageUrl")]
        [JsonConverter(typeof(ImageUrlJsonConverter))]
        public string ProfileImage { get; set; } = string.Empty;

        public bool IsAdmin { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public bool IsGuest { get; set; } = false;

        public string StatusLabel => IsActive ? "Active" : "Deactivated";

        public string ActionLabel => IsActive ? "Deactivate" : "Reactivate";

        public string RoleLabel => IsAdmin ? "Admin" : IsGuest ? "Guest" : "Member";

        public int? GymId { get; set; }
    }
}