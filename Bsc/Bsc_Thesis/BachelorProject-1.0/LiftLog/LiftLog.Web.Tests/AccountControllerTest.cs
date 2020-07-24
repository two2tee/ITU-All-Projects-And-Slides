using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LiftLog.Data.Entities;
using LiftLog.Service.EmailService;
using LiftLog.Service.Interfaces;
using LiftLog.Web.Controllers.Web;
using LiftLog.Web.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using LiftLog.Core.Dto;
using Microsoft.AspNetCore.Http.Authentication;

namespace LiftLog.Web.Tests
{

    /// <summary>
    /// This class tests the controller logic of the AccountController and tests how it behaves based on valid and invalid inputs when creating accounts.
    /// This includes the following controller responsibilities:
    /// 1) Verify ModelState.IsValid
    /// 2) Return error response if Modelstate is invalid
    /// 3) Retrieve a business entity from persistence 
    /// 4) Perform action on business entity
    /// 5) Save business entity to persistence
    /// 6) Return appropriate IActionResult
    /// Note that 3-5 are tested in the service layer. 
    /// </summary>
    public class AccountControllerTest
    {

        #region View Result Tests 

        // Login, register, logout, ExternalLogin, ExternalLoginCallback, ExternalLoginConfirmation, ConfirmEmail, ForgotPassword, ForgotPasswordConfirmation, ResetPassword, ResetPasswordConfirmation, AccessDenied, AddErrors, RedirectToLocal 
  
        /// <summary>
        /// Test Login method by verifying that a ViewResult is returned. 
        /// </summary>
        /// <returns> True if a view result is returned. </returns>
        [Fact(DisplayName = "Login returns a ViewResult")]
        public void Login_ReturnsAViewResult()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new FakeUserManager(); 
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object); 
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = controller.Login();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Test Logout method by verifying that a ViewResult is returned. 
        /// </summary>
        /// <returns> True if a view result is returned. </returns>
        [Fact(DisplayName = "Logout returns a ViewResult")]
        public async Task Logout_ReturnsAViewResult()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new FakeUserManager(); 
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object); 
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object); 
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = await controller.Logout() as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        /// <summary>
        /// Test Register method by verifying that a ViewResult is returned. 
        /// </summary>
        /// <returns> True if a view result is returned. </returns>
        [Fact(DisplayName = "Register returns a ViewResult")]
        public void Register_ReturnsAViewResult()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new FakeUserManager();
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object); 
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = controller.Register();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Test ForgotPassword method by verifying that a ViewResult is returned. 
        /// </summary>
        /// <returns> True if a view result is returned. </returns>
        [Fact(DisplayName = "Forgot password returns a ViewResult")]
        public void ForgotPassword_ReturnsAViewResult()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new FakeUserManager(); 
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object); 
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = controller.ForgotPassword();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Test ForgotPasswordConfirmation method by verifying that a ViewResult is returned. 
        /// </summary>
        /// <returns> True if a view result is returned. </returns>
        [Fact(DisplayName = "Forgot password confirmation returns a ViewResult")]
        public void ForgotPasswordConfirmation_ReturnsAViewResult()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new FakeUserManager();
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = controller.ForgotPasswordConfirmation();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Test ResetPassword method by verifying that a ViewResult is returned. 
        /// </summary>
        /// <returns> True if a view result is returned. </returns>
        [Fact(DisplayName = "Reset password returns a ViewResult")]
        public void ResetPassword_ReturnsAViewResult_WithToken()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new FakeUserManager();
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = controller.ResetPassword("TokenCode");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Test ResetPassword method by verifying that a ViewResult is returned. 
        /// </summary>
        /// <returns> True if a view result is returned. </returns>
        [Fact(DisplayName = "Reset password returns a ViewResult")]
        public void ResetPassword_ReturnsAViewErrorResult_WithoutToken()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new FakeUserManager();
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = controller.ResetPassword();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);

        }

        /// <summary>
        /// Test ResetPasswordConfirmation method by verifying that a ViewResult is returned. 
        /// </summary>
        /// <returns> True if a view result is returned. </returns>
        [Fact(DisplayName = "Reset password confirmation returns a ViewResult")]
        public void ResetPasswordConfirmation_ReturnsAViewResult()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new FakeUserManager();
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = controller.ResetPasswordConfirmation();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Test AccessDenied method by verifying that a ViewResult is returned. 
        /// </summary>
        /// <returns> True if a view result is returned. </returns>
        [Fact(DisplayName = "Access denied returns a ViewResult")]
        public void AccessDenied_ReturnsAViewResult()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new FakeUserManager();
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = controller.AccessDenied();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact(DisplayName = "ConfirmEmail returns view result when email was confirmed")]
        public async Task ConfirmEmail_ReturnsViewResult_WhenEmailConfirmed()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new FakeUserManager();
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = await controller.ConfirmEmail("Test", "Test") as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("ConfirmEmail", result.ViewName);
        }

        [Fact(DisplayName = "ConfirmEmail returns view error when email was not confirmed")]
        public async Task ConfirmEmail_ReturnsViewError_WhenEmailNotConfirmed()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            mockUserManager.Setup(u => u.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed()) // Returns result error 
                .Verifiable();
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = await controller.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact(DisplayName = "ConfirmEmail returns view error when userId is empty")]
        public async Task ConfirmEmail_ReturnsViewError_WhenUserIdEmpty()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            mockUserManager.Setup(u => u.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed()) // Returns result error 
                .Verifiable();
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = await controller.ConfirmEmail(null, null) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact(DisplayName = "ConfirmEmail returns view error when user is empty")]
        public async Task ConfirmEmail_ReturnsViewError_WhenUserIsEmpty()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            mockUserManager.Setup(u => u.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed()) // Returns result error 
                .Verifiable();
            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null)
                .Verifiable();
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = await controller.ConfirmEmail(null, null) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", result.ViewName);
        }

        #endregion

        #region Model State Tests

        /// <summary>
        /// Test model state for reset password with invalid model state. 
        /// </summary>
        /// <returns></returns>
        [Fact(DisplayName = "Reset password returns view result for retry when model state is invalid")]
        public async Task ResetPassword_ReturnsViewResultRetry_WhenModelStateIsInvalid()
        {
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new FakeUserManager(); 
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object); 
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var resetPasswordViewModel = new ResetPasswordViewModel(){ Code = "", ConfirmPassword = "Password", Email = "test@gmail.com", Password = "Password" };

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            controller.ModelState.AddModelError("SessionName", "Required");

            // Act 
            var result = await controller.ResetPassword(resetPasswordViewModel) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            var model = Assert.IsType<ResetPasswordViewModel>(result.Model);
            Assert.Equal("Password", model.Password);
            Assert.Equal("Password", model.ConfirmPassword);
            Assert.Equal("", model.Code);
            Assert.Equal("test@gmail.com", model.Email);
        }

        /// <summary>
        /// Test reset password when model state is valid. 
        /// </summary>
        /// <returns></returns>
        [Fact(DisplayName = "Reset password returns view result for valid model state")]
        public async Task ResetPassword_ReturnsViewResult_WhenModelStateIsValid()
        {
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new FakeUserManager();
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object); 
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            var model = new ResetPasswordViewModel() { Email = "test@test.com", Password = "12345678", ConfirmPassword = "12345678"};

            // Act 
            var result = await controller.ResetPassword(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Account", redirectToActionResult.ControllerName);
            Assert.Equal("ResetPasswordConfirmation", redirectToActionResult.ActionName);
        }

        #endregion

        #region External login methods 

        // ExternalLogin, ExternalLoginCallback, ExternalLoginConfirmation, 

        [Fact(DisplayName = "ExternalLogin returns challenge result")]
        public async Task ExternalLogin_ReturnsLogin()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            var mockUrlHelper = new Mock<IUrlHelper>();
            controller.Url = mockUrlHelper.Object;
            //mockUrlHelper.Setup(u => u.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
            //    .Returns("redirectUrl") // TODO cannot mock Action extension method
            //    .Verifiable();

            mockSignInManager.Setup(s => s.ConfigureExternalAuthenticationProperties(It.IsAny<string>(), It.IsAny<string>(), null))
                .Returns(new AuthenticationProperties())
                .Verifiable();

            // Act
            var result = controller.ExternalLogin("Provider", "ReturnUrl") as ChallengeResult;

            // Assert
            Assert.IsType<ChallengeResult>(result);
        }

        [Fact(DisplayName = "ExternalLoginCallback returns view result retry if remote error")]
        public async Task ExternalLoginCallback_ReturnsViewResultRetry_IfRemoteError()
        {
            // Arrange 
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new FakeUserManager();
            var mockSignInManager = new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager,
                mockUserManager,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = await controller.ExternalLoginCallback(null, "remoterror") as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Login", result.ViewName);
        }

        [Fact(DisplayName = "ExternalLoginCallback returns redirect action if external login info is empty")]
        public async Task ExternalLoginCallback_ReturnsRedirectToAction_IfExternalLoginInfoIsEmpty()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            mockSignInManager.Setup(s => s.GetExternalLoginInfoAsync(It.IsAny<string>()))
                .ReturnsAsync((ExternalLoginInfo) null) // Return empty external login info 
                .Verifiable();
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            // Act 
            var result = await controller.ExternalLoginCallback(null, null) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", result.ActionName);
        }

        [Fact(DisplayName = "ExternalLoginCallback returns redirect action if direct to local result is true and user was signed in")]
        public async Task ExternalLoginCallback_ReturnsToLocalResultTrue_IfUserSignedIn()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); // Success return default in class 
            mockSignInManager.Setup(s => s.GetExternalLoginInfoAsync(null))
                .ReturnsAsync(new ExternalLoginInfo(new ClaimsPrincipal(), "loginprovider", "providerkey", "displayname"))
                .Verifiable();
            mockSignInManager.Setup(s => s.ExternalLoginSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success)
                .Verifiable();
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.IsLocalUrl(It.IsAny<string>()))
                .Returns(false)
                .Verifiable(); // Return false and RedirectToActionResult 

            controller.Url = mockUrlHelper.Object;

            // Act 
            var result = await controller.ExternalLoginCallback(null, null) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        [Fact(DisplayName = "ExternalLoginCallback returns redirect result if direct to local result is false and user was signed in")]
        public async Task ExternalLoginCallback_ReturnsToLocalResultFalse_IfUserSignedIn()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); // Success return default in class 
            mockSignInManager.Setup(s => s.GetExternalLoginInfoAsync(null))
                .ReturnsAsync(new ExternalLoginInfo(new ClaimsPrincipal(), "loginprovider", "providerkey", "displayname"))
                .Verifiable();
            mockSignInManager.Setup(s => s.ExternalLoginSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success)
                .Verifiable();
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.IsLocalUrl(It.IsAny<string>()))
                .Returns(true)
                .Verifiable(); // Return true and Redirect result  

            controller.Url = mockUrlHelper.Object;

            // Act 
            var result = await controller.ExternalLoginCallback(returnUrl : "returnUrl", remoteError: null) as RedirectResult;

            // Assert
            Assert.IsType<RedirectResult>(result);
        }

        [Fact(DisplayName = "External login callback returns lockout if user is locked out")]
        public async Task ExternalLoginCallback_Lockout_IfResultLockedOut()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            mockSignInManager.Setup(s => s.GetExternalLoginInfoAsync(null))
                .ReturnsAsync(new ExternalLoginInfo(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable(); // Return external login info 

            mockSignInManager.Setup(s => s.ExternalLoginSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.LockedOut)
                .Verifiable(); // Return user lock out 

            // Act
            var result = await controller.ExternalLoginCallback("returnUrl", remoteError: null) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Lockout", result.ViewName);
        }

        [Fact(DisplayName = "External login callback returns external login confirmation if no account")]
        public async Task ExternalLoginCallback_ExternalLoginConfirmationViewResult_IfNoAccount()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            var externalInfo = new ExternalLoginInfo(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            externalInfo.Principal = new ClaimsPrincipal();

            mockSignInManager.Setup(s => s.GetExternalLoginInfoAsync(null))
                .ReturnsAsync(externalInfo)
                .Verifiable(); // Return external login info 

            mockSignInManager.Setup(s => s.ExternalLoginSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed)
                .Verifiable(); // Return error, no account  

            // Act
            var result = await controller.ExternalLoginCallback("returnUrl", remoteError: null) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("ExternalLoginConfirmation", result.ViewName);
        }

        [Fact(DisplayName = "External login confirmation returns view result if model state is invalid")]
        public async Task ExternalLoginConfirmation_ReturnsViewResult_IfModelStateIsInvalid()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            controller.ModelState.AddModelError("", ""); // Add error 

            var externalViewModel = new ExternalLoginConfirmationViewModel();

            // Act
            var result = await controller.ExternalLoginConfirmation(externalViewModel) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<ExternalLoginConfirmationViewModel>(result.Model);
            //Assert.Equal("ExternalLoginConfirmation", result.ViewName);
        }
        
        [Fact(DisplayName = "External login confirmation returns login failure if login info is empty")]
        public async Task ExternalLoginConfirmation_ReturnsLoginFailureViewResult_IfLoginInfoIsEmpty()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            mockSignInManager.Setup(s => s.GetExternalLoginInfoAsync(null))
                .ReturnsAsync((ExternalLoginInfo)null)
                .Verifiable(); // Returns empty info 

            var externalViewModel = new ExternalLoginConfirmationViewModel();

            // Act
            var result = await controller.ExternalLoginConfirmation(externalViewModel) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("ExternalLoginFailure", result.ViewName);
        }

        [Fact(DisplayName = "External login confirmation returns redirect action if login was added and url is not local")]
        public async Task ExternalLoginConfirmation_ReturnsRedirectAction_IfLoginAddedSuccessAndNonLocalUrl()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            mockSignInManager.Setup(s => s.GetExternalLoginInfoAsync(null))
                .ReturnsAsync(new ExternalLoginInfo(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable(); // Returns empty info 

            mockUserManager.Setup(c => c.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable(); // User created

            mockUserManager.Setup(c => c.AddLoginAsync(It.IsAny<User>(), It.IsAny<ExternalLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable(); // User login added 

            mockSignInManager.Setup(s => s.SignInAsync(It.IsAny<User>(), false, null))
                .Returns(Task.FromResult(0)) 
                .Verifiable(); // Sign in starts task 

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.IsLocalUrl(It.IsAny<string>()))
                .Returns(false)
                .Verifiable(); // Return false and RedirectToActionResult 

            controller.Url = mockUrlHelper.Object;

            var externalViewModel = new ExternalLoginConfirmationViewModel();

            // Act
            var result = await controller.ExternalLoginConfirmation(externalViewModel, returnUrl: "returnUrl") as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", result.ControllerName);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact(DisplayName = "External login confirmation returns redirect result if login was added and url is local")]
        public async Task ExternalLoginConfirmation_ReturnsRedirectResult_IfLoginAddedSuccessAndLocalUrl()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            mockSignInManager.Setup(s => s.GetExternalLoginInfoAsync(null))
                .ReturnsAsync(new ExternalLoginInfo(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable(); // Returns empty info 

            mockUserManager.Setup(c => c.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable(); // User created

            mockUserManager.Setup(c => c.AddLoginAsync(It.IsAny<User>(), It.IsAny<ExternalLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable(); // User login added 

            mockSignInManager.Setup(s => s.SignInAsync(It.IsAny<User>(), false, null))
                .Returns(Task.FromResult(0))
                .Verifiable(); // Sign in starts task 

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.IsLocalUrl(It.IsAny<string>()))
                .Returns(true)
                .Verifiable(); // Return false and RedirectToActionResult 

            controller.Url = mockUrlHelper.Object;

            var externalViewModel = new ExternalLoginConfirmationViewModel();

            // Act
            var result = await controller.ExternalLoginConfirmation(externalViewModel, returnUrl: "returnUrl") as RedirectResult;

            // Assert
            Assert.IsType<RedirectResult>(result);
        }

        #endregion

        [Fact(DisplayName = "Login returns view result with model if email is confirmed")]
        public async Task Login_ReturnsViewResultWithModel_IfUserEmailConfirmed()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            mockUserManager.Setup(u => u.IsEmailConfirmedAsync(It.IsAny<User>()))
                .ReturnsAsync(false) // Email not verified 
                .Verifiable();
            mockUserManager.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new User()) // Return user on search 
                .Verifiable();

            // Act
            var result = await controller.Login(new LoginViewModel()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.True(result.ViewData.ModelState.Count == 1);
            Assert.True(result.ViewData.ModelState.ErrorCount == 1);
            var model = Assert.IsType<LoginViewModel>(result.Model);
            //Assert.Equal("You must have a confirmed email to log in.", result.ViewData.ModelState.Errors);
        }


        [Fact(DisplayName = "Login returns redirect home action if signed in")]
        public async Task Login_ReturnsRedirectAction_IfUserSignedIn()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object)
                .Verifiable();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            mockUserManager.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User) null) // Return user on search 
                .Verifiable();

            mockSignInManager.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success)
                .Verifiable();

            var loginViewModel = new LoginViewModel() {ReturnUrl = "ReturnUrl" };

            // Act
            var result = await controller.Login(loginViewModel) as RedirectResult;

            // Assert
            Assert.IsType<RedirectResult>(result);
        }

        [Fact(DisplayName = "Login returns redirect result if return url exists")]
        public async Task Login_ReturnsRedirectResult_IfReturnUrlExists()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object)
                .Verifiable();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            mockUserManager.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null) // Return user on search 
                .Verifiable();

            mockSignInManager.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success)
                .Verifiable();

            var loginViewModel = new LoginViewModel() { }; // ReturnUrl is empty 

            // Act
            var result = await controller.Login(loginViewModel) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }


        [Fact(DisplayName = "Login returns lockout view result if user is locked out")]
        public async Task Login_ReturnsLockoutViewResult_IfUserIsLockedOut()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object)
                .Verifiable();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            mockUserManager.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null) // Return user on search 
                .Verifiable();

            mockSignInManager.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.LockedOut) // User locked out 
                .Verifiable();

            var loginViewModel = new LoginViewModel() { }; // ReturnUrl is empty 

            // Act
            var result = await controller.Login(loginViewModel) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Lockout", result.ViewName);
        }

        [Fact(DisplayName = "Login returns view result with model if model state is invalid")]
        public async Task Login_ReturnsViewResultWithModel_IfModelStateIsInvalid()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object)
                .Verifiable();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            controller.ModelState.AddModelError("", ""); // Add error 
            var loginViewModel = new LoginViewModel() { }; 

            // Act
            var result = await controller.Login(loginViewModel) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.IsType<LoginViewModel>(result.Model);
        }

        [Fact(DisplayName = "Register returns view result with model if model state is invalid")]
        public async Task Register_ReturnsViewResultWithModel_IfModelStateIsInvalid()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object)
                .Verifiable();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            controller.ModelState.AddModelError("", ""); // Add error 
            var registerViewModel = new RegisterViewModel() { };

            // Act
            var result = await controller.Register(registerViewModel) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<RegisterViewModel>(result.Model);
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
        }

        // TODO
        //[Fact(DisplayName = "Register returns redirect action if user was saved and model state is valid")]
        //public async Task Register_ReturnsRedirectAction_IfUserSavedAndValidModelState()
        //{
        //    // Arrange
        //    var context = new Mock<HttpContext>();
        //    var contextAccessor = new Mock<IHttpContextAccessor>();
        //    contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
        //    var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
        //    var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
        //    var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
        //    var mockLoggerFactory = new Mock<ILoggerFactory>();
        //    var mockLogger = new Mock<ILogger>();
        //    mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
        //        .Returns(mockLogger.Object)
        //        .Verifiable();
        //    var mockEmailSender = new Mock<IEmailSender>();
        //    var mockUserService = new Mock<IUserService>();

        //    var controller = new AccountController(
        //        mockSignInManager.Object,
        //        mockUserManager.Object,
        //        mockIdentityCookieOptions.Object,
        //        mockLoggerFactory.Object,
        //        mockEmailSender.Object,
        //        mockUserService.Object);

        //    var registerViewModel = new RegisterViewModel() { };

        //    mockUserManager.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
        //        .ReturnsAsync(IdentityResult.Success) // Return create success 
        //        .Verifiable();

        //    mockUserService.Setup(u => u.SaveNewUserAsync(It.IsAny<NewUserDto>()))
        //        .ReturnsAsync(It.IsAny<int>) // Return saved user id 
        //        .Verifiable();

        //    mockUserManager.Setup(u => u.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
        //        .ReturnsAsync("code") // Return email token 
        //        .Verifiable();

        //    mockEmailSender.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //        .Returns(Task.FromResult(0)) // Return email 
        //        .Verifiable();

        //    var mockUrlHelper = new Mock<IUrlHelper>(); // Mock Url Helper 
        //    controller.Url = mockUrlHelper.Object;
          

        //    var mockHttpContext = new Mock<HttpContext>();
        //    var mockHttpRequest = new Mock<HttpRequest>();
        //    mockHttpContext.Setup(x => x.Request).
        //        Returns(mockHttpRequest.Object).
        //        Verifiable();

        //    mockHttpContext.Setup(h => h.Request.Scheme)
        //        .Returns("scheme") //It.IsAny<string>()
        //        .Verifiable();

        //    var mockControllerContext = new Mock<ControllerContext>();
        //    controller.ControllerContext = mockControllerContext.Object;

        //    mockControllerContext.Setup(c => c.HttpContext)
        //        .Returns(mockHttpContext.Object)
        //        .Verifiable(); 

        //    // TODO HttpContext in controller is currently null 

        //    //controller.HttpContext = mockHttpContext;

        //    //var mockActionContext = new Mock<ActionContext>();
        //    //mockActionContext.Setup(a => a.HttpContext).Returns(mockHttpContext.Object)
        //    //    .Verifiable();

        //    //actionContext.HttpContext = mockHttpContext.Object;

        //    //mockUrlHelper.Setup(u => u.ActionContext).
        //    //    Returns(mockActionContext.Object).
        //    //    Verifiable();

        //    // Act
        //    var result = await controller.Register(registerViewModel) as RedirectToActionResult;

        //    // Assert
        //    Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", result.ControllerName);
        //    Assert.Equal("Home", result.ControllerName);
        //}

        // TODO
        //[Fact(DisplayName = "ForgotPassword returns confirmation view result if user does not exist")]
        //public async Task ForgotPassword_ReturnsConfirmationViewResult_IfUserNotExists()
        //{
        //    // Arrange
        //    var context = new Mock<HttpContext>();
        //    var contextAccessor = new Mock<IHttpContextAccessor>();
        //    contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
        //    var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
        //    var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
        //    var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
        //    var mockLoggerFactory = new Mock<ILoggerFactory>();
        //    var mockLogger = new Mock<ILogger>();
        //    mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
        //        .Returns(mockLogger.Object)
        //        .Verifiable();
        //    var mockEmailSender = new Mock<IEmailSender>();
        //    var mockUserService = new Mock<IUserService>();

        //    var controller = new AccountController(
        //        mockSignInManager.Object,
        //        mockUserManager.Object,
        //        mockIdentityCookieOptions.Object,
        //        mockLoggerFactory.Object,
        //        mockEmailSender.Object,
        //        mockUserService.Object);

        //    mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
        //        .ReturnsAsync(new User())
        //        .Verifiable(); // Find returns user

        //    mockUserManager.Setup(u => u.IsEmailConfirmedAsync(It.IsAny<User>()))
        //        .ReturnsAsync(true)
        //        .Verifiable(); // Email confirmed 
 
        //    var mockUrlHelper = new Mock<IUrlHelper>(); // Mock Url Helper 
        //    controller.Url = mockUrlHelper.Object;

        //    // Mock HttpRequest TODO  
        //    mockUrlHelper.SetupGet(u => u.ActionContext.HttpContext.Request.Scheme)
        //        .Returns("scheme")
        //        .Verifiable();

        //    var registerViewModel = new RegisterViewModel() { };

        //    // Act
        //    var result = await controller.Register(registerViewModel) as ViewResult;

        //    // Assert
        //    Assert.IsType<ViewResult>(result);
        //    Assert.Equal("ForgotPasswordConfirmation", result.ViewName);
        //    Assert.Equal(true, result.ViewData.ModelState.IsValid);
        //}

        [Fact(DisplayName = "ForgotPassword returns confirmation view result if already exists")]
        public async Task ForgotPassword_ReturnsConfirmationViewResult_IfUserExists()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object)
                .Verifiable();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User) null)
                .Verifiable(); // Find user returns null 

            var forgotPasswordViewModel = new ForgotPasswordViewModel() { };

            // Act
            var result = await controller.ForgotPassword(forgotPasswordViewModel) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("ForgotPasswordConfirmation", result.ViewName);
            Assert.Equal(true, result.ViewData.ModelState.IsValid);
        }

        [Fact(DisplayName = "ForgotPassword returns view result if model state is invalid")]
        public async Task ForgotPassword_ReturnsViewResultRetry_IfModelStateIsInvalid()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object)
                .Verifiable();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            controller.ModelState.AddModelError("", ""); // Add error 

            var forgotPasswordViewModel = new ForgotPasswordViewModel() { };

            // Act
            var result = await controller.ForgotPassword(forgotPasswordViewModel) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<ForgotPasswordViewModel>(result.Model);
            Assert.True(result.ViewData.ModelState.ErrorCount == 1);
            //Assert.Equal("ForgotPassword", result.ViewName);
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
        }

        [Fact(DisplayName = "ResetPassword returns view result if model state is invalid")]
        public async Task ResetPassword_ReturnsViewResultRetry_IfModelStateIsInvalid()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object)
                .Verifiable();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            controller.ModelState.AddModelError("", ""); // Add error 

            var resetPasswordViewModel = new ResetPasswordViewModel() { };

            // Act
            var result = await controller.ResetPassword(resetPasswordViewModel) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<ResetPasswordViewModel>(result.Model);
            Assert.True(result.ViewData.ModelState.ErrorCount == 1);
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
        }

        [Fact(DisplayName = "ResetPassword returns redirect action if user does not exist")]
        public async Task ResetPassword_ReturnsRedirectAction_IfUserNotExists()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object)
                .Verifiable();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null)
                .Verifiable(); // Return null user not exists 

            var resetPasswordViewModel = new ResetPasswordViewModel() { };

            // Act
            var result = await controller.ResetPassword(resetPasswordViewModel) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ResetPasswordConfirmation", result.ActionName);
            Assert.Equal("Account", result.ControllerName);
        }

        [Fact(DisplayName = "ResetPassword returns redirect action if user password was reset")]
        public async Task ResetPassword_ReturnsRedirectAction_IfPasswordReset()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object)
                .Verifiable();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<User>())
                .Verifiable(); // Return user 

            mockUserManager.Setup(u => u.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable(); // Password reset success 

            var resetPasswordViewModel = new ResetPasswordViewModel() { };

            // Act
            var result = await controller.ResetPassword(resetPasswordViewModel) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ResetPasswordConfirmation", result.ActionName);
            Assert.Equal("Account", result.ControllerName);
        }

        [Fact(DisplayName = "ResetPassword returns view if error occured")]
        public async Task ResetPassword_ReturnsViewResult_Default()
        {
            // Arrange
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>(); // new FakeUserManager();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object); //new FakeSignInManager(contextAccessor.Object);
            var mockIdentityCookieOptions = new Mock<IOptions<IdentityCookieOptions>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object)
                .Verifiable();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AccountController(
                mockSignInManager.Object,
                mockUserManager.Object,
                mockIdentityCookieOptions.Object,
                mockLoggerFactory.Object,
                mockEmailSender.Object,
                mockUserService.Object);

            controller.ModelState.AddModelError("", ""); // Add error

            mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<User>())
                .Verifiable(); // Return user 

            mockUserManager.Setup(u => u.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(It.IsAny<IdentityError>()))
                .Verifiable(); // Password reset failure  

            var resetPasswordViewModel = new ResetPasswordViewModel() { };

            // Act
            var result = await controller.ResetPassword(resetPasswordViewModel) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
        }


    }
}
