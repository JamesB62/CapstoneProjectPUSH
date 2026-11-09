namespace CapstoneProjectPUSH.Services
{
    public record AuthResult(bool Success, string Message, User? User = null);

    public class UserService
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        private const int MinDisplayNameLength = 5;
        private const int MaxDisplayNameLength = 60;

        public const string PasswordRequirementsText = "Password must be at least 8 characters and contain at least one letter and one number.";

        private readonly DatabaseService _databaseService;

        public UserService(DatabaseService database)
        {
            _databaseService = database;
        }

        public User? CurrentUser { get; private set; }

        // Registration
        public async Task<AuthResult> RegisterAsync(string displayName, string email, string password, string confirmPassword, bool hasAcceptedPrivacy)
        {
            if (displayName is null)
            {
                displayName = string.Empty;
            }

            displayName = displayName.Trim();

            email = NormaliseEmail(email);

            // Validation
            if (displayName.Length < MinDisplayNameLength || displayName.Length > MaxDisplayNameLength)
            {
                return new AuthResult(false, $"Display name must be between {MinDisplayNameLength} and {MaxDisplayNameLength} characters.");
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

            // Database checks
            var db = await _databaseService.GetConnectionAsync();

            var emailTaken = await db.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();

            if (emailTaken is not null)
            {
                return new AuthResult(false, "An account with that email already exists.");
            }

            var nameTaken = await db.Table<User>().Where(u => u.DisplayName.ToLower() == displayName.ToLower()).FirstOrDefaultAsync();

            if (nameTaken is not null)
            {
                return new AuthResult(false, "An account with that display name already exists.");
            }

            // Create user
            var user = new User
            {
                Email = email,
                DisplayName = displayName,
                PasswordHash = await Task.Run(() => HashPassword(password)),
            };

            try
            {
                await db.InsertAsync(user);
            }
            catch (SQLiteException)
            {
                return new AuthResult(false, "An account with that email or display name already exists.");
            }

            CurrentUser = user;

            return new AuthResult(true, "Account created successfully.", user);
        }

        // Login
        public async Task<AuthResult> LoginAsync(string email, string password, bool requireAdmin = false)
        {
            email = NormaliseEmail(email);

            // Validation
            if (string.IsNullOrWhiteSpace(email))
            {
                return new AuthResult(false, "Please enter your email address.");
            }

            if (string.IsNullOrEmpty(password))
            {
                return new AuthResult(false, "Please enter your password.");
            }

            // Database check
            var db = await _databaseService.GetConnectionAsync();

            var user = await db.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();

            if (user is null)
            {
                return new AuthResult(false, "Invalid email or password.");
            }

            var passwordCorrect = await Task.Run(() => VerifyPassword(password, user.PasswordHash));

            if (!passwordCorrect)
            {
                return new AuthResult(false, "Invalid email or password.");
            }

            if (!user.IsActive)
            {
                return new AuthResult(false, "This account has been deactivated.");
            }

            // Login user
            CurrentUser = user;

            return new AuthResult(true, $"Welcome back, {user.DisplayName}!", user);
        }

        // Login guest
        public async Task<AuthResult> GuestLoginAsync()
        {
            // Database check
            var db = await _databaseService.GetConnectionAsync();

            var guestUser = await db.Table<User>().Where(u => u.Email == "guest@push.local").FirstOrDefaultAsync();

            if (guestUser is null)
            {
                return new AuthResult(false, "Guest account not found.");
            }

            // Login user
            CurrentUser = guestUser;

            return new AuthResult(true, $"Welcome back, {guestUser.DisplayName}!", guestUser);
        }


        // Logout
        public void Logout()
        {
            CurrentUser = null;
        }

        // Update details
        public async Task<AuthResult> UpdateDetailsAsync(string displayName, string email, string currentPassword, string newPassword, string confirmPassword)
        {
            if (CurrentUser is null)
            {
                return new AuthResult(false, "You must be signed in to update your details.");
            }

            if (!CurrentUser.IsActive)
            {
                return new AuthResult(false, "This account has been deactivated.");
            }

            if (displayName is null)
            {
                displayName = string.Empty;
            }

            displayName = displayName.Trim();

            email = NormaliseEmail(email);

            // Validation
            if (displayName.Length < MinDisplayNameLength || displayName.Length > MaxDisplayNameLength)
            {
                return new AuthResult(false, $"Display name must be between {MinDisplayNameLength} and {MaxDisplayNameLength} characters.");
            }

            if (!IsValidEmail(email))
            {
                return new AuthResult(false, "Please enter a valid email address.");
            }

            var changingPassword = !string.IsNullOrEmpty(newPassword) || !string.IsNullOrEmpty(confirmPassword);

            if (changingPassword)
            {
                if (string.IsNullOrEmpty(currentPassword))
                {
                    return new AuthResult(false, "Enter your current password to set a new one.");
                }

                if (!IsStrongPassword(newPassword))
                {
                    return new AuthResult(false, PasswordRequirementsText);
                }

                if (newPassword != confirmPassword)
                {
                    return new AuthResult(false, "New passwords do not match.");
                }
            }

            // Database checks
            var db = await _databaseService.GetConnectionAsync();

            var user = await db.Table<User>().Where(u => u.Id == CurrentUser.Id).FirstOrDefaultAsync();

            if (user is null)
            {
                return new AuthResult(false, "Account could not be found.");
            }

            var emailTaken = await db.Table<User>().Where(u => u.Email == email && u.Id != user.Id).FirstOrDefaultAsync();

            if (emailTaken is not null)
            {
                return new AuthResult(false, "An account with that email already exists.");
            }

            var nameTaken = await db.Table<User>().Where(u => u.DisplayName == displayName && u.Id != user.Id).FirstOrDefaultAsync();

            if (nameTaken is not null)
            {
                return new AuthResult(false, "An account with that display name already exists.");
            }

            if (changingPassword)
            {
                var passwordCorrect = await Task.Run(() => VerifyPassword(currentPassword, user.PasswordHash));

                if (!passwordCorrect)
                {
                    return new AuthResult(false, "Current password is incorrect.");
                }

                user.PasswordHash = await Task.Run(() => HashPassword(newPassword));
            }

            // Update details
            user.DisplayName = displayName;
            user.Email = email;

            try
            {
                await db.UpdateAsync(user);
            }
            catch (SQLiteException)
            {
                return new AuthResult(false, "An account with that email already exists.");
            }

            CurrentUser = user;

            return new AuthResult(true, "Details updated successfully.", user);
        }

        // Get user count
        public async Task<int> GetUserCountAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<User>().CountAsync();
        }

        // Get active user count
        public async Task<int> GetActiveUserCountAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<User>().Where(u => u.IsActive).CountAsync();
        }

        // Get all users
        public async Task<List<User>> GetUsersAsync()
        {
            if (!IsCurrentUserAdmin())
            {
                return new List<User>();
            }

            var db = await _databaseService.GetConnectionAsync();

            var users = await db.Table<User>()
                .OrderBy(u => u.DateTimeCreated)
                .ToListAsync();

            return users;
        }

        // Get user by Id
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            if (!IsCurrentUserAdmin())
            {
                return null;
            }

            var db = await _databaseService.GetConnectionAsync();

            var user = await db.Table<User>()
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user is null)
            {
                return null;
            }

            return user;
        }

        // Set user active status
        public async Task<AuthResult> SetUserActiveStatusAsync(int userId, bool isActive)
        {
            if (!IsCurrentUserAdmin())
            {
                return new AuthResult(false, "Only administrators can change account status.");
            }

            if (userId == CurrentUser!.Id)
            {
                return new AuthResult(false, "You can't deactivate your own account.");
            }

            var db = await _databaseService.GetConnectionAsync();

            var user = await db.Table<User>().Where(u => u.Id == userId).FirstOrDefaultAsync();

            if (user is null)
            {
                return new AuthResult(false, "Account could not be found.");
            }

            user.IsActive = isActive;

            await db.UpdateAsync(user);

            if (isActive)
            {
                return new AuthResult(true, "Account reactivated.", user);
            }

            return new AuthResult(true, "Account deactivated.", user);
        }

        // Helpers
        // Normalise email
        private static string NormaliseEmail(string? email)
        {
            if (email is null)
            {
                return string.Empty;
            }

            return email.Trim().ToLowerInvariant();
        }

        // Validate email format
        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || email.Length > 254)
            {
                return false;
            }

            try
            {
                var address = new MailAddress(email);
                return address.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        // Validate password strength
        private static bool IsStrongPassword(string? password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            if (password.Length < 8)
            {
                return false;
            }

            return password.Any(char.IsLetter) && password.Any(char.IsDigit);
        }

        // Hash password using PBKDF2
        public static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        // Verify password against stored hash
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

        // Admin check
        private bool IsCurrentUserAdmin()
        {
            if (CurrentUser == null)
            {
                return false;
            }

            return CurrentUser.IsAdmin;
        }
    }
}