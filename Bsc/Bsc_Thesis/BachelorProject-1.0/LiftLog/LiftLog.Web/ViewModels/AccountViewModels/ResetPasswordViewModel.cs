// ResetPasswordViewModel.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.ComponentModel.DataAnnotations;

namespace LiftLog.Web.ViewModels.AccountViewModels
{
    /// <summary>
    ///     Represents data used to reset a user password on the forgot password view page (communicated to the account
    ///     controller).
    /// </summary>
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}