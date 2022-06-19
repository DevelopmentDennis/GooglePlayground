using GooglePlaces.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace GooglePlaces.Services
{

    public interface IEmailService
    {
        public Task<bool> SendEmail(string? reason = "");
    }

    public class EmailService : IEmailService
    {
        private readonly EmailOptions _options;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
        {

            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        /// <summary>
        /// Send a mail with the settings configurated in the options
        /// </summary>
        /// <returns>true if the mail has been sent and false otherwise</returns>
        public async Task<bool> SendEmail(string? reason = "")
        {
            var smtpClient = new SmtpClient(_options.SmtpServer, _options.Port)
            {
                Credentials = new NetworkCredential(_options.SmtpUsername, _options.SmtpPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            var message = GenerateEmail(reason);

            try
            {
                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("Sending email failed {message}", e);
                return false;
            }


        }

        private MailMessage GenerateEmail(string? reason)
        {
            var message = new MailMessage();
            message.From = new MailAddress(_options.SmtpUsername);
            message.To.Add(_options.SmtpMailTarget);
            message.Subject = "Google Maps Spielplatz Warnung";
            message.Body = string.IsNullOrEmpty(reason) ? "Der Spielplatz wurde als geschlossen markiert." : reason;
            message.Priority = MailPriority.High;

            return message;
        }
    }
}
