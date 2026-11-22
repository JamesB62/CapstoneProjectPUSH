namespace CapstoneProjectPUSH.Services
{
    public class PasswordResetService
    {
        private readonly ApiClient _apiClient;

        public PasswordResetService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // Request an password reset code via email
        public async Task RequestResetAsync(string email)
        {
            ResetCodeRequest request = new ResetCodeRequest
            {
                Email = Clean(email).ToLowerInvariant()
            };

            ApiResult<MessageResponse> result = await _apiClient.PostAsync<MessageResponse>("api/passwordreset/request", request);

            if (!result.Success)
            {
                System.Diagnostics.Debug.WriteLine("Reset request failed: " + result.Message);
            }
        }

        // Send the code and the new password
        public async Task<AuthResult> ResetPasswordAsync(string email, string code, string newPassword, string confirmPassword)
        {
            email = Clean(email).ToLowerInvariant();
            code = Clean(code);

            // App-side validation
            if (code.Length == 0)
            {
                return new AuthResult(false, "Invalid or expired code.");
            }

            if (!UserService.IsStrongPassword(newPassword))
            {
                return new AuthResult(false, UserService.PasswordRequirementsText);
            }

            if (newPassword != confirmPassword)
            {
                return new AuthResult(false, "Passwords do not match.");
            }

            ResetPasswordRequest request = new ResetPasswordRequest
            {
                Email = email,
                Code = code,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword
            };

            ApiResult<MessageResponse> result = await _apiClient.PostAsync<MessageResponse>("api/passwordreset/reset", request);

            if (!result.Success)
            {
                return new AuthResult(false, result.Message);
            }

            if (result.Data is null)
            {
                return new AuthResult(true, "Password updated. You can log in now.");
            }

            return new AuthResult(true, result.Data.Message);
        }

        // Helpers
        // Cleans input
        private static string Clean(string? value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            return value.Trim();
        }
    }
}