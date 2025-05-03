using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace semsary_backend.Service
{
    public class EmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            String smtpServer = configuration["SmtpSettings:Server"];
            int smtpPort = int.Parse(configuration["SmtpSettings:Port"]);
            string SenderName = configuration["SmtpSettings:SenderName"];
            string SenderEmail = configuration["SmtpSettings:SenderEmail"];
            string Password = configuration["SmtpSettings:Password"];
            var Username = configuration["SmtpSettings:Username"];
            var EmailMessage = new MimeMessage();
            EmailMessage.From.Add(new MailboxAddress(SenderName, SenderEmail));
            EmailMessage.To.Add(new MailboxAddress("", to));
            EmailMessage.Subject = subject;
            EmailMessage.Body = new TextPart("html")
            {
                Text = body
            };
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(Username, Password);
                await client.SendAsync(EmailMessage);
                await client.DisconnectAsync(true);
            }





        }
    }
}
