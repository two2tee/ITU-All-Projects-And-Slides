// RegisterViewModel.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.ComponentModel.DataAnnotations;
using LiftLog.Core.Enums;
using LiftLog.Web.ViewModels.AccountViewModels.ViewLogic;

namespace LiftLog.Web.ViewModels.AccountViewModels
{
    /// <summary>
    ///     This view model contains the required data used to register a user on the registration page.
    ///     This data is used in the AccountController to create a new user.
    ///     Data annotations are used to validate the registration model data.
    /// </summary>
    public class RegisterViewModel
    {
        // Authentication information 

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 8)]
        [DataType(DataType.Password)] // Hide on screen 
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [StringLength(72, MinimumLength = 0)]
        public string DisplayName { get; set; }

        public string ConsentMessage { get; set; }
       
        //Consent checkbox
        public bool IsShareConsent { get; set; }
        public bool IsChallengeConsent { get; set; }
        public bool IsWorkoutConsent { get; set; }

        [Display(Name = "Terms and Conditions")]
        [MustBeTrue(ErrorMessage = "You must agree with Terms and Conditions in order to sign up.")]
        public bool IsTermsOfUse { get; set; }



    }
}