// IEmailSender.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Threading.Tasks;

namespace LiftLog.Service.EmailService
{
    /// <summary>
    ///     Interface describing requried contents of en email used upon email confirmation or reset password.
    /// </summary>
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}