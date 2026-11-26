namespace CapstoneProjectPUSH.Models
{
    public class PasswordReset
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string CodeHash { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public int FailedAttempts { get; set; }
    }
}