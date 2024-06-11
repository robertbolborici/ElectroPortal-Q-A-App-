using System.Net.Mail;
using System.Net;

namespace ElectroPortal.NewFolder
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }

    public class EmailSenderService : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(IConfiguration configuration, ILogger<EmailSenderService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                using (var client = new SmtpClient(emailSettings["MailServer"], int.Parse(emailSettings["MailPort"])))
                {
                    client.Credentials = new NetworkCredential(emailSettings["Sender"], emailSettings["Password"]);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(emailSettings["Sender"], emailSettings["SenderName"]),
                        Subject = subject,
                        Body = htmlMessage,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpException ex)
            {
                _logger.LogError($"SMTP Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email sending failed: {ex.Message}");
                throw;
            }
        }
    }
}
