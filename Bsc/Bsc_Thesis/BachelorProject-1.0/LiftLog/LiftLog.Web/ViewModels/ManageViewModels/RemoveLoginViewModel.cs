// RemoveLoginViewModel.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

namespace LiftLog.Web.ViewModels.ManageViewModels
{
    public class RemoveLoginViewModel
    {
        /// <summary>
        ///     Represents data to be displayed when trying to remove login information from an external login provider (e.g.
        ///     Facebook)
        ///     Data is used in Manage Controller to remove user login credentials provided by an external login provider.
        /// </summary>
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}