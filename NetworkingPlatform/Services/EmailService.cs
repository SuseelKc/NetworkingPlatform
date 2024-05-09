using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Threading.Tasks;
using NetworkingPlatform.Interface;
using NetworkingPlatform.Models;
using NetworkingPlatform.Configuration;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfig;

    public EmailService(IOptions<EmailConfiguration> emailConfig)
    {
        _emailConfig = emailConfig.Value;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        using (SmtpClient client = new SmtpClient(_emailConfig.SmtpServer, _emailConfig.SmtpPort))
        {
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(_emailConfig.SmtpUsername, _emailConfig.SmtpPassword);
            client.EnableSsl = true;

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_emailConfig.SenderEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}
