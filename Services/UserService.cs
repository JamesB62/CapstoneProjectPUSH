namespace CapstoneProjectPUSH.Services
{
    public record AuthResult(bool Success, string Message, User? User = null);

    public class UserService
    {
        public const string PasswordRequirementsText = "Password must be at least 8 characters and contain at least one letter and one number.";

        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        private readonly DatabaseService _database;

        public UserService(DatabaseService database) => _database = database;

        // User who is currently logged in, or null
        public User? CurrentUser { get; private set; }

        public async Task<AuthResult> RegisterAsync(string displayName, string email, string password, string confirmPassword, bool hasAcceptedPrivacy)
        {
            displayName = (displayName ?? string.Empty).Trim();

            email = NormalizeEmail(email);

            if (displayName.Length < 2 || displayName.Length > 50)
            {
                return new AuthResult(false, "Display name must be between 2 and 50 characters.");
            }

            if (!IsValidEmail(email))
            {
                return new AuthResult(false, "Please enter a valid email address.");
            }

            if (!IsStrongPassword(password))
            {
                return new AuthResult(false, PasswordRequirementsText);
            }

            if (password != confirmPassword)
            {
                return new AuthResult(false, "Passwords do not match.");
            }

            if (!hasAcceptedPrivacy)
            {
                return new AuthResult(false, "You must agree to the privacy consent to register.");
            }

            var db = await _database.GetConnectionAsync();

            var existing = await db.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();

            if (existing is not null)
            {
                return new AuthResult(false, "An account with that email already exists.");
            }

            var user = new User
            {
                Email = email,
                DisplayName = displayName,
                // PBKDF2 is deliberately slow
                PasswordHash = await Task.Run(() => HashPassword(password)),
                DateTimeCreated = DateTime.Now
            };

            try
            {
                await db.InsertAsync(user);
            }
            catch (SQLiteException)
            {
                return new AuthResult(false, "An account with that email already exists.");
            }

            CurrentUser = user;
            return new AuthResult(true, "Account created successfully.", user);
        }

        public async Task<AuthResult> LoginAsync(string email, string password, bool requireAdmin = false)
        {
            email = NormalizeEmail(email);

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
            {
                return new AuthResult(false, "Please enter your email address and password.");
            }

            var db = await _database.GetConnectionAsync();

            var user = await db.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();

            // Generic error message so emails can't be probed
            const string invalid = "Invalid email or password.";

            if (user is null)
            {
                return new AuthResult(false, invalid);
            }

            var ok = await Task.Run(() => VerifyPassword(password, user.PasswordHash));

            if (!ok)
            {
                return new AuthResult(false, invalid);
            }

            // Checked after the password so the message can't be used to probe which emails are admins
            if (requireAdmin && !user.IsAdmin)
            {
                return new AuthResult(false, "This account does not have admin access.");
            }

            CurrentUser = user;
            return new AuthResult(true, $"Welcome back, {user.DisplayName}!", user);
        }

        public void Logout() => CurrentUser = null;

        public async Task<int> GetUserCountAsync()
        {
            var db = await _database.GetConnectionAsync();
            return await db.Table<User>().CountAsync();
        }

        // Helpers
        private static string NormalizeEmail(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || email.Length > 254)
            {
                return false;
            }

            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static bool IsStrongPassword(string? password) =>
            !string.IsNullOrEmpty(password)
            && password.Length >= 8
            && password.Any(char.IsLetter)
            && password.Any(char.IsDigit);

        public static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        private static bool VerifyPassword(string password, string stored)
        {
            var parts = stored.Split('.');
            if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            {
                return false;
            }

            try
            {
                var salt = Convert.FromBase64String(parts[1]);
                var expected = Convert.FromBase64String(parts[2]);
                var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);

                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}