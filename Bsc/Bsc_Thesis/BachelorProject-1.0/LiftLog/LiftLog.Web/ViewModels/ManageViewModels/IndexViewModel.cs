// IndexViewModel.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LiftLog.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace LiftLog.Web.ViewModels.ManageViewModels
{
    /// <summary>
    ///     Represents data shown in user management page used to manage user logins.
    ///     Data is comprised of logins and personal data that can be configured.
    /// </summary>
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }

        public IList<UserLoginInfo> Logins { get; set; } // Represents login information

        // Represents personal data 

        [StringLength(30, MinimumLength = 3)]
        public string DisplayName { get; set; }

        [StringLength(60, MinimumLength = 3)]
        public string Name { get; set; }

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public Sex Sex { get; set; }

        public Country Country { get; set; }

        [Range(0, 300)]
        public int BodyWeight { get; set; }

        [Range(0, 220)]
        public int Height { get; set; }

        public string ExportLink { get; set; }
    }
}