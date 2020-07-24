// ExternalLoginConfirmationViewModel.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.ComponentModel.DataAnnotations;

namespace LiftLog.Web.ViewModels.AccountViewModels
{
    /// <summary>
    ///     Represents data to be displayed after having registered an account via an external login provider (e.g. Facebook)
    ///     Data is used in Account Controller to register user via the email provided by the third party provider.
    /// </summary>
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}