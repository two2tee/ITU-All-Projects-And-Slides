using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiftLog.Web.Tests
{
    /// <summary>
    /// Test stub used to mock the IOptions interface and decouple code from the actual implementation.
    /// </summary>
    public class FakeIOptions : IFakeOptions 
    {
        public FakeIOptions()
        {
            _option = new FakeIdentityCookieOptions() { ExternalCookieAuthenticationScheme = "Scheme" };
        }

        FakeIdentityCookieOptions _option;

        public FakeIdentityCookieOptions Value { get { return this._option; } set { this._option = value; } }

        //public FakeIdentityCookieOptions Value { get => new FakeIdentityCookieOptions() { ExternalCookieAuthenticationScheme = "Scheme" }; }
    }
}
