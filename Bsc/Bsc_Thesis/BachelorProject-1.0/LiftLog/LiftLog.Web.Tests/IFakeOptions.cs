using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiftLog.Web.Tests
{

    /// <summary>
    /// Test stub used to mock the IOptions interface and decouple code from the actual implementation. 
    /// </summary>
    public interface IFakeOptions : IOptions<FakeIdentityCookieOptions>  
    {
        //new FakeIdentityCookieOptions Value { get; set; }
        new FakeIdentityCookieOptions Value { get; }
    }
}
