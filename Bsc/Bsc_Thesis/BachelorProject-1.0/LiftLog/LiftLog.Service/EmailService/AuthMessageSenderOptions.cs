// AuthMessageSenderOptions.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

namespace LiftLog.Service.EmailService
{
    /// <summary>
    ///     This option class is used to access user account and key settings and securely fetch the email key of a user.
    ///     The option pattern uses custom options classes to represent a group of related settings.
    ///     This decoupled class for the Email service follow:
    ///     The Interface Segregation Principle (ISP): Class only depend on configuration it uses
    ///     Separation of concerns: Settings for different parts of app are not coupled
    ///     Link: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration#options-config-objects
    /// </summary>
    public class AuthMessageSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
    }
}