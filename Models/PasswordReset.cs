namespace CapstoneProjectPUSH.Models
{
    [Table("PasswordReset")]
    public class PasswordReset
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int UserId { get; set; }

        public string CodeHash { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public int FailedAttempts { get; set; }
    }
}