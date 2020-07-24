// SetPasswordViewModel.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.ComponentModel.DataAnnotations;

namespace LiftLog.Web.ViewModels.ManageViewModels
{
    /// <summary>
    ///     Represents data used to create and confirm a new password when managing a user in the ManageController.
    /// </summary>
    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}