using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LiftLog.Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace LiftLog.Web.Tests
{

    /// <summary>
    /// Test stub used to mock the UserManager interface and decouple code from the actual implementation.
    /// </summary>
    public class FakeUserManager : UserManager<User>
    {
        public FakeUserManager()
            : base(new Mock<IUserStore<User>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<IPasswordHasher<User>>().Object,
                  new IUserValidator<User>[0],
                  new IPasswordValidator<User>[0],
                  new Mock<ILookupNormalizer>().Object,
                  new Mock<IdentityErrorDescriber>().Object,
                  new Mock<IServiceProvider>().Object,
                  new Mock<ILogger<UserManager<User>>>().Object)
        { }

        public override Task<IdentityResult> CreateAsync(User user, string password)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<User> GetUserAsync(ClaimsPrincipal user)
        {
            return Task.FromResult(new User { Id = 1 });
        }

        public override Task<User> FindByEmailAsync(string email)
        {
            return Task.FromResult(new User { Email = email });
        }

        public override Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<bool> IsEmailConfirmedAsync(User user)
        {
            return Task.FromResult(user.Email == "test@test.com");
        }
        public override Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return Task.FromResult("---------------");
        }

        public override Task<User> FindByIdAsync(string userId)
        {
            return Task.FromResult(new User()); //new User {Id = userId});
        }

        public override Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            return Task.FromResult(IdentityResult.Success);
        }

    }
}
