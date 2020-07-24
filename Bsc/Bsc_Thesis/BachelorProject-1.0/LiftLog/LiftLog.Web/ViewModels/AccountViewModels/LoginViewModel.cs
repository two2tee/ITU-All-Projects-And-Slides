// LoginViewModel.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.ComponentModel.DataAnnotations;

namespace LiftLog.Web.ViewModels.AccountViewModels
{
    /// <summary>
    ///     Represents data to be displayed on Login View Page and used to communicate data between the page and account
    ///     controller.
    /// </summary>
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; } // Session cookie (false) vs permanent cookie (true) 
        public string ReturnUrl { get; set; } // Original page user wanted to go to (e.g. workouts before logging in) 
    }
}