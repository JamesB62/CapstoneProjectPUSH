namespace CapstoneProjectPUSH.Services
{
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string body);
    }

    public class DebugEmailSender : IEmailSender
    {
        public async Task SendAsync(string toEmail, string subject, string body)
        {
            System.Diagnostics.Debug.WriteLine($"[DEMO EMAIL] to={toEmail} | {subject} | {body}");
            // await Shell.Current.DisplayAlertAsync("DEMO email (not really sent)", body, "OK");
        }
    }
}