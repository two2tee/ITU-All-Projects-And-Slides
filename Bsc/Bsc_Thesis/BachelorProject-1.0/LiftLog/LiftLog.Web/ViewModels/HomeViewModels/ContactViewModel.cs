// ContactViewModel.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.ComponentModel.DataAnnotations;

namespace LiftLog.Web.ViewModels.HomeViewModels
{
    /// <summary>
    ///     This class is used to communicate with the Contact View page and fetch data.
    ///     Model Binding: accept data directly inside method of controller
    ///     The Contact View Model represents the data to be displayed on the Contacts view page.
    ///     The View Model is used to communicate data to the HomeController.
    /// </summary>
    public class ContactViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(4096, MinimumLength = 10)]
        public string Message { get; set; }
    }
}