// AuthMessageSender.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace LiftLog.Service.EmailService
{
    /// <summary>
    ///     This class is used by the application to send Email.
    /// </summary>
    public class AuthMessageSender : IEmailSender
    {
        public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            Execute(Options.SendGridKey, subject, message, email).Wait();
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Sends email
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="subject"> Subject of email</param>
        /// <param name="message"> Message of email. </param>
        /// <param name="email"> Email. </param>
        /// <returns> Request to send email with SendGrid. </returns>
        public async Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage
            {
                From = new EmailAddress("thorolesen14@gmail.com", "Thor Olesen"), // Todo Replace with company email
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            var response = await client.SendEmailAsync(msg);
        }
    }
}