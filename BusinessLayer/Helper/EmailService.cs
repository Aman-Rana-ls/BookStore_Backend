using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BusinessLayer.Helper
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendOtpEmailAsync(string recipientEmail, string otp)
        {
            try
            {
                _logger.LogInformation("Preparing to send email to {RecipientEmail}", recipientEmail);

                string smtpServer = _configuration["EmailSettings:SmtpServer"];
                int smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                string senderEmail = _configuration["EmailSettings:SenderEmail"];
                string senderPassword = _configuration["EmailSettings:SenderPassword"];

                _logger.LogInformation("SMTP Configuration - Server: {SmtpServer}, Port: {SmtpPort}, Sender: {SenderEmail}",
                    smtpServer, smtpPort, senderEmail);

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "Password Reset OTP",
                    Body = $"Your OTP for password reset is: {otp}. It is valid for 5 minutes.",
                    IsBodyHtml = true
                };

                mail.To.Add(recipientEmail);

                using SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                    Timeout = 15000
                };

                _logger.LogInformation("Sending email asynchronously...");
                await smtpClient.SendMailAsync(mail);
                _logger.LogInformation("Email sent successfully to {RecipientEmail}", recipientEmail);

                return true;
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError("SMTP Exception - Status: {StatusCode}, Message: {Message}", smtpEx.StatusCode, smtpEx.Message);
                return false;
            }
            catch (TimeoutException timeoutEx)
            {
                _logger.LogError("Timeout Exception - The email sending took too long: {Message}", timeoutEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error sending email to {RecipientEmail}: {Message}", recipientEmail, ex.Message);
                return false;
            }
        }
    }
}
