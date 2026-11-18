
namespace CapstoneProjectPUSH.Services
{
    public class EmailSettings
    {
        public string SenderName{ get; set; } = "PUSH";
        public string SenderEmail { get; set; } = string.Empty;
        public string AppPassword { get; set; } = string.Empty;
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
    }

    public class GmailEmailSender : IEmailSender
    {
        private readonly DebugEmailSender _fallback = new();
        private EmailSettings? _settings;
        private bool _loaded;

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            if(!_loaded)
            {
                _settings = await LoadSettingsAsync();
                _loaded = true;
            }

            if(_settings is null 
                || string.IsNullOrWhiteSpace(_settings.SenderEmail)
                || string.IsNullOrWhiteSpace(_settings.AppPassword))
            {
                await _fallback.SendAsync(toEmail, subject, body);
                return;
            }

            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MimeKit.MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new MimeKit.TextPart("plain") { Text = body };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.CheckCertificateRevocation = false;
            await client.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.AppPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
        }
        private static async Task<EmailSettings?> LoadSettingsAsync()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("emailsettings.json");
                return await System.Text.Json.JsonSerializer.DeserializeAsync<EmailSettings>(stream);
            }
            catch
            {
                return null;
            }
        }
    }
}