namespace CapstoneProjectPUSH.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, NotNull, MaxLength(60)]
        public string DisplayName { get; set; } = string.Empty;

        [Unique, NotNull, MaxLength(250)]
        public string Email { get; set; } = string.Empty;

        [NotNull]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime DateTimeCreated { get; set; } = DateTime.Now;

        // Swap to ProfileImageUrl when using remote database
        public string ProfileImage { get; set; } = string.Empty;

        public bool IsAdmin { get; set; } = false;

        public bool IsActive { get; set; } = true;
        public bool IsGuest { get; set; } = false;

        [Ignore]
        public string StatusLabel => IsActive ? "Active" : "Deactivated";

        [Ignore]
        public string ActionLabel => IsActive ? "Deactivate" : "Reactivate";
    }
}