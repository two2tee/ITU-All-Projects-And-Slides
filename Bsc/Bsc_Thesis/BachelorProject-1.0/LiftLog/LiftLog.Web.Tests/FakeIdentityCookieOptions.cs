using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiftLog.Web.Tests
{
    /// <summary>
    /// Override base class (i.e. IdentityCookieOptions) property using new keyword.
    ///  Test stub used to mock the IdentityCookieOptions interface and decouple code from the actual implementation.
    ///  Used as part of the FakeIOptions test stub for the IOptions interface. 
    /// </summary>
    public class FakeIdentityCookieOptions : IdentityCookieOptions
    {
        public FakeIdentityCookieOptions()
        {
        }

        public new string ExternalCookieAuthenticationScheme { get; set; }
    }

}
