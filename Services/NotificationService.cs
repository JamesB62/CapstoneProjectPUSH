namespace CapstoneProjectPUSH.Services
{
    public class NotificationService
    {
        private readonly IEmailSender _emailSender;

        public NotificationService(IEmailSender emailSender) => _emailSender = emailSender;

        public Task SendWelcomeAsync(User user) =>
            SafeSendAsync(user.Email, "Welcome to PUSH",
                $"Hi {user.DisplayName}, welcome to PUSH! Start by exploring the workouts on the home page.");

        public Task SendPasswordChangedAsync(User user) =>
            SafeSendAsync(user.Email, "Your PUSH password was changed",
                "Your password was just changed. If this wasn't you, please reset it again right away.");

        private async Task SafeSendAsync(string to, string subject, string body)
        {
            try { await _emailSender.SendAsync(to, subject, body); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }
        }
    }
}