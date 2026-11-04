namespace CapstoneProjectPUSH.Models
{
    [Table("Trainers")]
    public class Trainer
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(60)]
        public string FirstName { get; set; } = string.Empty;

        [NotNull, MaxLength(60)]
        public string LastName { get; set; } = string.Empty;

        [Ignore]
        public string FullName => $"{FirstName} {LastName}";

        [NotNull, MaxLength(250)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public string ProfileImage { get; set; } = string.Empty;

        [Indexed]
        public int CategoryId { get; set; }

        [Ignore]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public bool Available { get; set; } = true;

        [Ignore]
        public string AvailableText => Available ? "Available" : "Unavailable";

        // Include schedule table?

        [Indexed]
        public int GymId { get; set; }

        [Ignore]
        public string GymName { get; set; } = string.Empty;

        [Ignore]
        public string GymCity { get; set; } = string.Empty;
    }
}