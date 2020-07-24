// ForgotPasswordViewModel.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.ComponentModel.DataAnnotations;

namespace LiftLog.Web.ViewModels.AccountViewModels
{
    /// <summary>
    ///     Represents data to be displayed on the Forgot Password view page and communicated to the Account Controller.
    /// </summary>
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}