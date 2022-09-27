


using UserSignUpAPI.Models.Email;

namespace UserSignUpAPI.Services.EmailServices
{

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(EmailDTO request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailFrom").Value));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

            // Configure options MailKit
            using var stmpClient = new SmtpClient();
            stmpClient.Connect(host: _configuration.GetSection("EmailHost").Value, port: Convert.ToInt32(_configuration.GetSection("SmtpPort").Value), SecureSocketOptions.StartTls);
            stmpClient.Authenticate(userName: _configuration.GetSection("EmailUserName").Value, password: _configuration.GetSection("EmailPassword").Value);
            stmpClient.Send(email);
            stmpClient.Disconnect(true);
        }
    }
}
