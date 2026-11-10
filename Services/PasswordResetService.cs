using System.Text;

namespace CapstoneProjectPUSH.Services
{
    public class PasswordResetService
    {
        private const int CodeMinutes = 15;
        private const int MaxAttempts = 5;

        private const string InvalidMessage = "Invalid or expired code.";

        private readonly DatabaseService _databaseService;
        private readonly UserService _userService;
        private readonly IEmailSender _emailSender;

        public PasswordResetService(DatabaseService databaseService, UserService userService, IEmailSender emailSender)
        {
            _databaseService = databaseService;
            _userService = userService;
            _emailSender = emailSender;
        }

        public async Task RequestResetAsync(string email)
        {
            email = (email ?? string.Empty).Trim().ToLowerInvariant();

            var db = await _databaseService.GetConnectionAsync();
            var user = await db.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();

            if (user is null || !user.IsActive) return;

            var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

            await db.Table<PasswordReset>().DeleteAsync(r => r.UserId == user.Id); //delete old code
            await db.InsertAsync(new PasswordReset
            {
                UserId = user.Id,
                CodeHash = HashCode(code),
                ExpiresAt = DateTime.UtcNow.AddMinutes(CodeMinutes)
            });

            await _emailSender.SendAsync(
                user.Email,
                "PUSH Password reset",
                $"Your reset code is {code}. It expires in {CodeMinutes} minutes."
            );
        }

        public async Task<AuthResult> ResetPasswordAsync(string email, string code, string newPassword, string confirmPassword)
        {
            email = (email ?? string.Empty).Trim().ToLowerInvariant();
            code = (code ?? string.Empty).Trim();

            var db = await _databaseService.GetConnectionAsync();
            var user = await db.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
            if(user is null) return new AuthResult(false, InvalidMessage);

            var reset = await db.Table<PasswordReset>().Where(r => r.UserId == user.Id).FirstOrDefaultAsync();
            if(reset is null) return new AuthResult(false,InvalidMessage);

            if(reset.ExpiresAt < DateTime.UtcNow || reset.FailedAttempts >= MaxAttempts)
            {
                await db.DeleteAsync(reset);
                return new AuthResult(false, InvalidMessage);
            }

           var matches = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(HashCode(code)),
                Encoding.UTF8.GetBytes(reset.CodeHash));

            if(!matches)
            {
                reset.FailedAttempts++;
                await db.UpdateAsync(reset);
                return new AuthResult(false, InvalidMessage);
            }

            var result = await _userService.SetNewPasswordAsync(user.Id, newPassword, confirmPassword);

            if(result.Success)
            {
                await db.DeleteAsync(reset);
            }
            return result;
        }

        private static string HashCode(string code) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code)));

            // check account 
            public async Task<bool> IsEmailRegisteredAsync(string email)
            {
                var db = await _databaseService.GetConnectionAsync();
                var user = await db.Table<User>().FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
                return user != null;
            }

    }

    
}