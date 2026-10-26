namespace CapstoneProjectPUSH.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, NotNull, MaxLength(250)]
        public string Email { get; set; } = string.Empty;

        // Added unique constraint
        [Unique, NotNull, MaxLength(60)]
        public string DisplayName { get; set; } = string.Empty;

        [NotNull]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime DateTimeCreated { get; set; } = DateTime.Now;
    }
}