using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LiftLog.Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace LiftLog.Web.Tests
{

    /// <summary>
    /// Test stub used to mock the SignInManager interface and decouple code from the actual implementation.
    /// </summary>
    public class FakeSignInManager : SignInManager<User>
    {
        public FakeSignInManager(IHttpContextAccessor contextAccessor)
            : base(new FakeUserManager(),
                  contextAccessor,
                  new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<ILogger<SignInManager<User>>>().Object)
        {
        }

        public override Task SignInAsync (User user, bool isPersistent, string authenticationMethod = null)
        {
            return Task.FromResult(0);
        }

        public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return Task.FromResult(SignInResult.Success);
        }

        public override Task SignOutAsync()
        {
            return Task.FromResult(0);
        }

        public override AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl,
            string userId = null)
        {
            return new AuthenticationProperties();
            //return base.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userId);
        }

        public override Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey,
            bool isPersistent)
        {
            return Task.FromResult(SignInResult.Success);
        }

        public override Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
        {
            return
                Task.FromResult(new ExternalLoginInfo(new ClaimsPrincipal(), "loginprovider", "providerkey",
                    "displayname"));
        }

    }
}
