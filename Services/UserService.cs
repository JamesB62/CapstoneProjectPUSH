namespace CapstoneProjectPUSH.Services
{
    public record AuthResult(bool Success, string Message, User? User = null);

    public class UserService
    {
        // Matches DbSeeder.SeedAdminEmail in the api
        public const string SeedAdminEmail = "admin@push.app";

        private const int MinDisplayNameLength = 5;
        private const int MaxDisplayNameLength = 60;

        public const string PasswordRequirementsText = "Password must be at least 8 characters and contain at least one letter and one number.";

        private readonly ApiClient _apiClient;

        public UserService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public User? CurrentUser { get; private set; }

        // No one is signed in
        public bool IsUnauthenticated => CurrentUser == null;

        // Signed in with the seeded guest account
        public bool IsGuest => CurrentUser != null && CurrentUser.IsGuest;

        // Signed in with a registered account
        public bool IsLoggedIn => CurrentUser != null && !CurrentUser.IsGuest;

        // Registration
        public async Task<AuthResult> RegisterAsync(string displayName, string email, string password, string confirmPassword, bool hasAcceptedPrivacy)
        {
            if (displayName is null)
            {
                displayName = string.Empty;
            }

            displayName = displayName.Trim();

            email = NormaliseEmail(email);

            // App-side validation
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

            // Create user request
            RegisterRequest request = new RegisterRequest
            {
                DisplayName = displayName,
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword,
                HasAcceptedPrivacy = hasAcceptedPrivacy
            };

            ApiResult<AuthResponse> result = await _apiClient.PostAsync<AuthResponse>("api/auth/register", request);

            return SignInFromResult(result);
        }

        // Login
        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            email = NormaliseEmail(email);

            // App-side validation
            if (string.IsNullOrWhiteSpace(email))
            {
                return new AuthResult(false, "Please enter your email address.");
            }

            if (string.IsNullOrEmpty(password))
            {
                return new AuthResult(false, "Please enter your password.");
            }

            LoginRequest request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            ApiResult<AuthResponse> result = await _apiClient.PostAsync<AuthResponse>("api/auth/login", request);

            return SignInFromResult(result);
        }

        // Login guest
        public async Task<AuthResult> GuestLoginAsync()
        {
            ApiResult<AuthResponse> result = await _apiClient.PostAsync<AuthResponse>("api/auth/guest", null);

            return SignInFromResult(result);
        }

        // Logout
        public void Logout()
        {
            CurrentUser = null;
            _apiClient.SetToken(null);
        }

        // Saves the token and signed-in user from a register/login reply
        private AuthResult SignInFromResult(ApiResult<AuthResponse> result)
        {
            if (!result.Success)
            {
                return new AuthResult(false, result.Message);
            }

            if (result.Data == null || result.Data.User == null || string.IsNullOrEmpty(result.Data.Token))
            {
                return new AuthResult(false, "Something went wrong. Please try again.");
            }

            _apiClient.SetToken(result.Data.Token);
            CurrentUser = result.Data.User;

            return new AuthResult(true, result.Data.Message, CurrentUser);
        }

        // Update details
        public async Task<AuthResult> UpdateDetailsAsync(string displayName, string email, string currentPassword, string newPassword, string confirmPassword, int? gymId, string? profileImage = null)
        {
            if (CurrentUser is null)
            {
                return new AuthResult(false, "You must be signed in to update your details.");
            }

            if (CurrentUser.IsGuest)
            {
                return new AuthResult(false, "Guest accounts can't update details. Register to create your own account.");
            }

            if (displayName is null)
            {
                displayName = string.Empty;
            }

            displayName = displayName.Trim();

            email = NormaliseEmail(email);

            // App-side validation
            if (displayName.Length < MinDisplayNameLength || displayName.Length > MaxDisplayNameLength)
            {
                return new AuthResult(false, $"Display name must be between {MinDisplayNameLength} and {MaxDisplayNameLength} characters.");
            }

            if (!IsValidEmail(email))
            {
                return new AuthResult(false, "Please enter a valid email address.");
            }

            if (IsSeededAdmin(CurrentUser) && email != CurrentUser.Email)
            {
                return new AuthResult(false, "The main admin account's email can't be changed.");
            }

            bool changingPassword = !string.IsNullOrEmpty(newPassword) || !string.IsNullOrEmpty(confirmPassword);

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

            UpdateDetailsRequest request = new UpdateDetailsRequest
            {
                DisplayName = displayName,
                Email = email,
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword,
                GymId = gymId
            };

            ApiResult<UserResponse> result = await _apiClient.PutAsync<UserResponse>("api/users/me", request);

            if (!result.Success)
            {
                return new AuthResult(false, result.Message);
            }

            if (result.Data == null || result.Data.User == null)
            {
                return new AuthResult(false, "Something went wrong. Please try again.");
            }

            CurrentUser = result.Data.User;
            string message = result.Data.Message;

            // New photo, removed photo, or unchanged
            if (profileImage is not null)
            {
                string? imageError = await ImageUpload.ApplyAsync(_apiClient, "api/images/profile", profileImage);

                if (imageError is not null)
                {
                    return new AuthResult(true, message + " The photo couldn't be saved: " + imageError, CurrentUser);
                }

                // Reload the account so current user has the new photo's address
                ApiResult<User> meResult = await _apiClient.GetAsync<User>("api/users/me");

                if (meResult.Success && meResult.Data != null)
                {
                    CurrentUser = meResult.Data;
                }
            }

            return new AuthResult(true, message, CurrentUser);
        }

        // Delete account
        public async Task<AuthResult> DeleteAccountAsync()
        {
            if (CurrentUser is null)
            {
                return new AuthResult(false, "You must be logged in to delete your account.");
            }

            if (CurrentUser.IsGuest)
            {
                return new AuthResult(false, "The guest account can't be deleted.");
            }

            if (CurrentUser.IsAdmin)
            {
                return new AuthResult(false, "Admin accounts can't be deleted.");
            }

            ApiResult<MessageResponse> result = await _apiClient.DeleteAsync<MessageResponse>("api/users/me");

            if (!result.Success)
            {
                return new AuthResult(false, result.Message);
            }

            Logout();

            return new AuthResult(true, "Your account has been deleted.");
        }

        // Get user count
        public async Task<int> GetUserCountAsync()
        {
            ApiResult<int> result = await _apiClient.GetAsync<int>("api/users/count");

            if (result.Success)
            {
                return result.Data;
            }

            return 0;
        }

        // Get active user count
        public async Task<int> GetActiveUserCountAsync()
        {
            ApiResult<int> result = await _apiClient.GetAsync<int>("api/users/active-count");

            if (result.Success)
            {
                return result.Data;
            }

            return 0;
        }

        // Get all users
        public async Task<List<User>> GetUsersAsync()
        {
            if (!IsCurrentUserAdmin())
            {
                return new List<User>();
            }

            ApiResult<List<User>> result = await _apiClient.GetAsync<List<User>>("api/users");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<User>();
        }

        // Set user active status
        public async Task<AuthResult> SetUserActiveStatusAsync(User user, bool isActive)
        {
            if (!IsCurrentUserAdmin())
            {
                return new AuthResult(false, "Only administrators can change account status.");
            }

            if (user.Id == CurrentUser!.Id)
            {
                return new AuthResult(false, "You can't deactivate your own account.");
            }

            if (IsSeededAdmin(user) && !isActive)
            {
                return new AuthResult(false, "The main admin account can't be deactivated.");
            }

            UserStatusRequest request = new UserStatusRequest
            {
                IsActive = isActive
            };

            ApiResult<UserResponse> result = await _apiClient.PutAsync<UserResponse>($"api/users/{user.Id}/status", request);

            if (!result.Success)
            {
                return new AuthResult(false, result.Message);
            }

            if (result.Data == null)
            {
                return new AuthResult(false, "Something went wrong. Please try again.");
            }

            return new AuthResult(true, result.Data.Message, result.Data.User);
        }

        // Set user role
        public async Task<AuthResult> SetUserRoleAsync(User user, bool isAdmin)
        {
            if (!IsCurrentUserAdmin())
            {
                return new AuthResult(false, "Only administrators can change account roles.");
            }

            if (user.Id == CurrentUser!.Id)
            {
                return new AuthResult(false, "You can't change your own role.");
            }

            if (user.IsGuest)
            {
                return new AuthResult(false, "The guest account's role can't be changed.");
            }

            if (IsSeededAdmin(user))
            {
                return new AuthResult(false, "The main admin account's role can't be changed.");
            }

            UserRoleRequest request = new UserRoleRequest
            {
                IsAdmin = isAdmin
            };

            ApiResult<UserResponse> result = await _apiClient.PutAsync<UserResponse>($"api/users/{user.Id}/role", request);

            if (!result.Success)
            {
                return new AuthResult(false, result.Message);
            }

            if (result.Data == null)
            {
                return new AuthResult(false, "Something went wrong. Please try again.");
            }

            return new AuthResult(true, result.Data.Message, result.Data.User);
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
        public static bool IsStrongPassword(string? password)
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

        // Admin check
        public bool IsCurrentUserAdmin()
        {
            if (CurrentUser == null)
            {
                return false;
            }

            return CurrentUser.IsAdmin;
        }

        // Signed-in user check
        public bool IsCurrentUser(int userId)
        {
            if (CurrentUser == null)
            {
                return false;
            }

            return CurrentUser.Id == userId;
        }

        // Registered user check
        public bool IsCurrentMember(int userId)
        {
            if (CurrentUser == null || CurrentUser.IsGuest)
            {
                return false;
            }

            return CurrentUser.Id == userId;
        }

        // Valid user get check
        public User? GetCurrentMember()
        {
            if (CurrentUser == null || CurrentUser.IsGuest)
            {
                return null;
            }
            return CurrentUser;
        }

        // The seeded admin is locked so there's always a way into the admin content
        public static bool IsSeededAdmin(User user)
        {
            return string.Equals(user.Email, SeedAdminEmail, StringComparison.OrdinalIgnoreCase);
        }
    }
}