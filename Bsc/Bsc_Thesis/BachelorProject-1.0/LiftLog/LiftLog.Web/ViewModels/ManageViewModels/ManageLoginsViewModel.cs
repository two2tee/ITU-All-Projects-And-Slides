// ManageLoginsViewModel.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;

namespace LiftLog.Web.ViewModels.ManageViewModels
{
    /// <summary>
    ///     Represents data used to manage user logins and external logins (e.g. Facebook) via the ManageController.
    /// </summary>
    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }
}