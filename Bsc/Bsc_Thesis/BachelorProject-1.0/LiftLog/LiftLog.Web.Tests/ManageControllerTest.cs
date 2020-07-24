
using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Core.Dto.PortableDto;
using LiftLog.Data.Entities;
using LiftLog.Service.EmailService;
using LiftLog.Service.Interfaces;
using LiftLog.Web.Controllers.Web;
using LiftLog.Web.ViewModels.ManageViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LiftLog.Web.Tests
{

    /// <summary>
    /// This class tests the controller logic of the ManageController and tests how it behaves based on valid and invalid inputs when managing accounts.
    /// This includes the following controller responsibilities:
    /// 1) Verify ModelState.IsValid
    /// 2) Return error response if Modelstate is invalid
    /// 3) Retrieve a business entity from persistence 
    /// 4) Perform action on business entity
    /// 5) Save business entity to persistence
    /// 6) Return appropriate IActionResult
    /// Note that 3-5 are tested in the service layer. 
    /// </summary>
    public class ManageControllerTest
    {

        [Fact(DisplayName = "Index returns view error result if user does not exist")]
        public async Task Index_ViewResultError_UserNotExists()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions(); 
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((User)null)
                .Verifiable(); // Return user not exist 

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            // Act 
            var result = await manageController.Index(new ParamsDto(), message: null) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", result.ViewName);
        }

        //[Fact(DisplayName = "Index returns view result with model")]
        //public async Task Index_ViewResultWithModel()
        //{
        //    // Arrange 
        //    var context = new Mock<HttpContext>();
        //    var contextAccessor = new Mock<IHttpContextAccessor>();
        //    contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
        //    var mockUserManager = new Mock<FakeUserManager>();
        //    var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
        //    var mockIdentityCookieOptions = new FakeIOptions();
        //    var mockEmailSender = new Mock<IEmailSender>();
        //    var mockLoggerFactory = new Mock<ILoggerFactory>();
        //    var mockLogger = new Mock<ILogger>();
        //    mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
        //        .Returns(mockLogger.Object);
        //    var mockMapper = new Mock<IMapper>();
        //    mockMapper.Setup(m => m.Map<User, IndexViewModel>(It.IsAny<User>()))
        //        .Returns(new IndexViewModel())
        //        .Verifiable();
        //    var mockUserService = new Mock<IUserService>();
        //    var mockExportService = new Mock<IExportService>();
        //    var mockWorkoutService = new Mock<IWorkoutService>();
        //    var mockChallengeService = new Mock<IChallengeService>();

        //    mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        //        .ReturnsAsync(new User())
        //        .Verifiable(); // Return user  

        //    mockUserManager.Setup(u => u.HasPasswordAsync(It.IsAny<User>()))
        //        .ReturnsAsync(true)
        //        .Verifiable(); // User password true 

        //    mockUserManager.Setup(u => u.GetLoginsAsync(It.IsAny<User>()))
        //        .ReturnsAsync(new List<UserLoginInfo>())
        //        .Verifiable(); // User logins 

        //    mockUserService.Setup(u => u.ModifyUserAsync(It.IsAny<ModifiedUserDto>()))
        //        .ReturnsAsync(false)
        //        .Verifiable(); // User modified not success 

        //    var manageController = new ManageController(
        //        mockUserManager.Object,
        //        mockSignInManager.Object,
        //        mockIdentityCookieOptions,
        //        mockEmailSender.Object,
        //        mockLoggerFactory.Object,
        //        mockMapper.Object,
        //        mockUserService.Object,
        //        mockExportService.Object,
        //        mockWorkoutService.Object,
        //        mockChallengeService.Object);

        //    // Act 
        //    var result = await manageController.Index(new ParamsDto(), message: null) as ViewResult;

        //    // Assert
        //    Assert.IsType<ViewResult>(result);
        //    Assert.IsType<IndexViewModel>(result.Model);
        //}

        [Fact(DisplayName = "Index post returns view error if user is empty")]
        public async Task IndexPost_ViewError_UserEmpty()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<IndexViewModel, ModifiedUserDto>(It.IsAny<IndexViewModel>()))
                .Returns(new ModifiedUserDto())
                .Verifiable();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((User)null)
                .Verifiable(); // No user 

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            // Act 
            var result = await manageController.Index(new IndexViewModel()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact(DisplayName = "Index post returns result with model if valid model state")]
        public async Task IndexPost_ViewResultWithModel_IfModelStateIsInvalid()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<IndexViewModel, ModifiedUserDto>(It.IsAny<IndexViewModel>()))
                .Returns(new ModifiedUserDto())
                .Verifiable();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // User found 

            mockUserManager.Setup(u => u.HasPasswordAsync(It.IsAny<User>()))
                .ReturnsAsync(true)
                .Verifiable(); // User has password 

            mockUserManager.Setup(u => u.GetLoginsAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<UserLoginInfo>())
                .Verifiable(); // User logins 

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            manageController.ModelState.AddModelError("", ""); // Add error 

            // Act 
            var result = await manageController.Index(new IndexViewModel()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<IndexViewModel>(result.Model);
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.True(result.ViewData.ModelState.ErrorCount == 1);
        }

        [Fact(DisplayName = "Index post returns redirect action if user is updated")]
        public async Task IndexPost_RedirectAction_IfUserModified()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<IndexViewModel, ModifiedUserDto>(It.IsAny<IndexViewModel>()))
                .Returns(new ModifiedUserDto())
                .Verifiable();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // User found 

            mockUserManager.Setup(u => u.HasPasswordAsync(It.IsAny<User>()))
                .ReturnsAsync(true)
                .Verifiable(); // User has password 

            mockUserManager.Setup(u => u.GetLoginsAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<UserLoginInfo>())
                .Verifiable(); // User logins 

            mockUserService.Setup(u => u.ModifyUserAsync(It.IsAny<ModifiedUserDto>()))
                .ReturnsAsync(true)
                .Verifiable(); // Modified success 

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            // Act 
            var result = await manageController.Index(new IndexViewModel()) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.ChangeAccountSuccess, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "Index post returns view result with model if user not updated")]
        public async Task IndexPost_ViewResultWithModel_IfUserNotUpdated()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<IndexViewModel, ModifiedUserDto>(It.IsAny<IndexViewModel>()))
                .Returns(new ModifiedUserDto())
                .Verifiable();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // User found 

            mockUserManager.Setup(u => u.HasPasswordAsync(It.IsAny<User>()))
                .ReturnsAsync(true)
                .Verifiable(); // User has password 

            mockUserManager.Setup(u => u.GetLoginsAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<UserLoginInfo>())
                .Verifiable(); // User logins 

            mockUserService.Setup(u => u.ModifyUserAsync(It.IsAny<ModifiedUserDto>()))
                .ReturnsAsync(false)
                .Verifiable(); // Modified user error 

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);
            
            // Act 
            var result = await manageController.Index(new IndexViewModel()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<IndexViewModel>(result.Model);
        }

        [Fact(DisplayName = "RemoveLogin returns view error result when model state is invalid")]
        public async Task RemoveLogin_ReturnsViewErrorResult_WhenModelStateIsInvalid()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            manageController.ModelState.AddModelError("", "");

            // Act
            var result = await manageController.RemoveLogin(new RemoveLoginViewModel()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("/Error", result.ViewName);
        }

        [Fact(DisplayName = "RemoveLogin returns redirect action when user exists")]
        public async Task RemoveLogin_ReturnsRedirectAction_UserExists()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // Get user 

            mockUserManager.Setup(u => u.RemoveLoginAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable(); // Removed login success 

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            // Act
            var result = await manageController.RemoveLogin(new RemoveLoginViewModel()) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageLogins", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.RemoveLoginSuccess, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "RemoveLogin returns redirect action when user does not exist")]
        public async Task RemoveLogin_ReturnsRedirectAction_UserNotExists()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((User) null)
                .Verifiable(); // Get user empty 

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            // Act
            var result = await manageController.RemoveLogin(new RemoveLoginViewModel()) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageLogins", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.Error, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "ChangePassword returns a view result")]
        public void ChangePassword_ReturnsAviewResult()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            // Act
            var result = manageController.ChangePassword() as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact(DisplayName = "ChangePassword returns view result if invalid model")]
        public async Task ChangePassword_ViewResultWithModel_IfModelStateIsInvalid()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            manageController.ModelState.AddModelError("", "");

            // Act
            var result = await manageController.ChangePassword(new ChangePasswordViewModel()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<ChangePasswordViewModel>(result.Model);
            Assert.True(result.ViewData.ModelState.ErrorCount == 1);
            Assert.Equal(false, result.ViewData.ModelState.IsValid); 
        }

        [Fact(DisplayName = "ChangePassword returns redirect action if password changed")]
        public async Task ChangePassword_RedirectAction_IfPasswordChanged()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // Returns user 

            mockUserManager.Setup(u => u.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable(); // Password changed 

            mockSignInManager.Setup(s => s.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), null))
                .Returns(Task.FromResult(1))
                .Verifiable(); // Sign in 

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            // Act
            var result = await manageController.ChangePassword(new ChangePasswordViewModel()) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.ChangePasswordSuccess, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "ChangePassword returns view result with model if password changed error")]
        public async Task ChangePassword_ViewResultWithModel_IfPasswordChangeError()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // Returns user 

            mockUserManager.Setup(u => u.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()))
                .Verifiable(); // Password change error 

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            // Act
            var result = await manageController.ChangePassword(new ChangePasswordViewModel()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<ChangePasswordViewModel>(result.Model);
        }

        [Fact(DisplayName = "ChangePassword returns redirect action")]
        public async Task Changepassword_RedirectAction_Default()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((User) null)
                .Verifiable(); // Returns no user 

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            // Act
            var result = await manageController.ChangePassword(new ChangePasswordViewModel()) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.Error, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "SetPassword returns view result")]
        public void SetPassword_ReturnsViewResult()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            // Act
            var result = manageController.SetPassword() as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact(DisplayName = "SetPasswordPost returns view result with model if invalid model state")]
        public async Task SetPasswordPost_ReturnsViewResultWithModel_IfModelStateInvalid()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            manageController.ModelState.AddModelError("", ""); // Add error 

            // Act
            var result = await manageController.SetPassword(new SetPasswordViewModel()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<SetPasswordViewModel>(result.Model);
        }

        [Fact(DisplayName = "SetPasswordPost returns redirect action if password added")]
        public async Task SetPasswordPost_ReturnsRedirectAction_IfPasswordAdded()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // Returns user 

            mockUserManager.Setup(u => u.AddPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable(); // Password added 

            mockSignInManager.Setup(s => s.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), null))
                .Returns(Task.FromResult(1))
                .Verifiable(); // Sign in 

            // Act
            var result = await manageController.SetPassword(new SetPasswordViewModel()) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.SetPasswordSuccess, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "SetPasswordPost returns view result if password not added")]
        public async Task SetPasswordPost_ReturnsViewResult_IfPasswordNotAdded()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // Returns user 

            mockUserManager.Setup(u => u.AddPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()))
                .Verifiable(); // Password add error  

            mockSignInManager.Setup(s => s.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), null))
                .Returns(Task.FromResult(1))
                .Verifiable(); // Sign in 

            // Act
            var result = await manageController.SetPassword(new SetPasswordViewModel()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<SetPasswordViewModel>(result.Model); // Errors added in AddErrors 
        }

        [Fact(DisplayName = "SetPasswordPost returns redirect action if default")]
        public async Task SetPasswordPost_RedirectAction_IfDefault()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((User) null)
                .Verifiable(); // Returns no user 

            // Act
            var result = await manageController.SetPassword(new SetPasswordViewModel()) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.Error, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "ManageLogins returns view error if user does not exist")]
        public async Task ManageLogins_ReturnsViewError_IfUserNotExists()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((User)null)
                .Verifiable(); // Returns no user 

            // Act
            var result = await manageController.ManageLogins(message: null) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact(DisplayName = "ManageLogins returns view result")]
        public async Task ManageLogins_ReturnsViewResultWithModel()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // Returns user 

            var userLogins = new List<UserLoginInfo>();

             mockUserManager.Setup(s => s.GetLoginsAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<UserLoginInfo>())
                .Verifiable(); 

            mockSignInManager.Setup(s => s.GetExternalAuthenticationSchemes())
                .Returns(new List<AuthenticationDescription>())
                .Verifiable();

            // Act
            var result = await manageController.ManageLogins(message: null) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<ManageLoginsViewModel>(result.Model);
        }

        // TODO
        //[Fact(DisplayName = "LinkLogin returns challenge")]
        //public async Task LinkLogin_ReturnsChallengeResult()
        //{
        //    // Arrange 
        //    var context = new Mock<HttpContext>();
        //    var contextAccessor = new Mock<IHttpContextAccessor>();
        //    contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
        //    var mockUserManager = new Mock<FakeUserManager>();
        //    var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
        //    var mockIdentityCookieOptions = new FakeIOptions();
        //    var mockEmailSender = new Mock<IEmailSender>();
        //    var mockLoggerFactory = new Mock<ILoggerFactory>();
        //    var mockLogger = new Mock<ILogger>();
        //    mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
        //        .Returns(mockLogger.Object);
        //    var mockMapper = new Mock<IMapper>();
        //    var mockUserService = new Mock<IUserService>();
        //    var mockExportService = new Mock<IExportService>();
        //    var mockWorkoutService = new Mock<IWorkoutService>();
        //    var mockChallengeService = new Mock<IChallengeService>();

        //    var manageController = new ManageController(
        //        mockUserManager.Object,
        //        mockSignInManager.Object,
        //        mockIdentityCookieOptions,
        //        mockEmailSender.Object,
        //        mockLoggerFactory.Object,
        //        mockMapper.Object,
        //        mockUserService.Object,
        //        mockExportService.Object,
        //        mockWorkoutService.Object,
        //        mockChallengeService.Object);

        //    // Mock HttpContext sign out async TODO 

        //    var mockUrlHelper = new Mock<IUrlHelper>();
        //    manageController.Url = mockUrlHelper.Object;
        //    mockUrlHelper.Setup(u => u.Action(It.IsAny<string>(), It.IsAny<string>()))
        //        .Returns("returnUrl") // TODO cannot mock extension method 
        //        .Verifiable();

        //    mockSignInManager.Setup(s => s.ConfigureExternalAuthenticationProperties(It.IsAny<string>(), It.IsAny<string>(), null))
        //        .Returns(new AuthenticationProperties())
        //        .Verifiable();

        //    // Act
        //    var result = await manageController.LinkLogin("provider") as ChallengeResult;

        //    // Assert
        //    Assert.IsType<ChallengeResult>(result);
        //}

        [Fact(DisplayName = "LinkLoginCallback returns view error if user does not exist")]
        public async Task LinkLoginCallback_ReturnsViewError_IfUserNotExists()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((User) null)
                .Verifiable(); // Returns no user 

            // Act
            var result = await manageController.LinkLoginCallback() as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact(DisplayName = "LinkLoginCallback returns redirect action if empty external login info")]
        public async Task LinkLoginCallback_ReturnsRedirectAction_IfExternalLoginInfoIsEmpty()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // Returns user 

            mockSignInManager.Setup(s => s.GetExternalLoginInfoAsync(null))
                .ReturnsAsync((ExternalLoginInfo)null)
                .Verifiable(); // Info empty 

            // Act
            var result = await manageController.LinkLoginCallback() as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageLogins", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.Error, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "LinkLoginCallback returns redirect action")]
        public async Task LinkLoginCallback_ReturnsRedirectAction_Default()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // Returns user 

            mockSignInManager.Setup(s => s.GetExternalLoginInfoAsync(null))
                .ReturnsAsync(new ExternalLoginInfo(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable(); // Info empty 

            mockUserManager.Setup(u => u.AddLoginAsync(It.IsAny<User>(), It.IsAny<ExternalLoginInfo>()))
                .ReturnsAsync(IdentityResult.Failed())
                .Verifiable(); // Add login error 

            // Act
            var result = await manageController.LinkLoginCallback() as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageLogins", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.Error, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "LinkLoginCallback returns redirect action if login was added")]
        public async Task LinkLoginCallback_ReturnsRedirectAction_IfAddLoginSuccess()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // Returns user 

            mockSignInManager.Setup(s => s.GetExternalLoginInfoAsync(null))
                .ReturnsAsync(new ExternalLoginInfo(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable(); // Info empty 

            mockUserManager.Setup(u => u.AddLoginAsync(It.IsAny<User>(), It.IsAny<ExternalLoginInfo>()))
                .ReturnsAsync(IdentityResult.Failed())
                .Verifiable(); // Add login success 

            // Act
            var result = await manageController.LinkLoginCallback() as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageLogins", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.Error, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "DeleteUser returns redirect action if deleted")]
        public async Task DeleteUser_ReturnsRedirectAction_IfDeleted()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // Returns user 

            mockSignInManager.Setup(s => s.SignOutAsync())
                .Returns(Task.FromResult("Logged out"))
                .Verifiable(); // Await log out 

            mockUserManager.Setup(u => u.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable(); // Delete success 

            // Act
            var result = await manageController.DeleteUser() as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal("Account was successfully deleted.", result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "DeleteUser returns redirect action if not deleted")]
        public async Task DeleteUser_ReturnsRedirectAction_IfNotDeleted()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // Returns user 

            mockSignInManager.Setup(s => s.SignOutAsync())
                .Returns(Task.FromResult("Logged out"))
                .Verifiable(); // Await log out 

            mockUserManager.Setup(u => u.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()))
                .Verifiable(); // Delete error 

            // Act
            var result = await manageController.DeleteUser() as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.DeleteAccountError, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "ExportPdf returns file if success")]
        public async Task ExportPdf_ReturnsFileIfSuccess()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // Returns user 

            mockExportService.Setup(e => e.ExportToFileAsync(It.IsAny<int>()))
                .ReturnsAsync(new FileDto() { FileContent = new byte[0], FileExtention = "extension", FileName = "filename"})
                .Verifiable(); // Return new file  

            // Act
            var result = await manageController.ExportPdf() as FileContentResult;

            // Assert
            Assert.IsType<FileContentResult>(result);
        }

        [Fact(DisplayName = "ExportPdf returns view error if exception thrown")]
        public async Task ExportPDf_ReturnsViewError_ExceptionThrown()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User())
                .Verifiable(); // Returns user 

            // No file returned by export service setup

            // Act
            var result = await manageController.ExportPdf() as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("/Error", result.ViewName);
        }

        // TODO
        //[Fact(DisplayName = "GetExportLink returns redirect action if link is exported")]
        //public async Task GetExportLink_ReturnsRedirectAction_IfLinkExported()
        //{
        //    // Arrange 
        //    var context = new Mock<HttpContext>();
        //    var contextAccessor = new Mock<IHttpContextAccessor>();
        //    contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
        //    var mockUserManager = new Mock<FakeUserManager>();
        //    var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
        //    var mockIdentityCookieOptions = new FakeIOptions();
        //    var mockEmailSender = new Mock<IEmailSender>();
        //    var mockLoggerFactory = new Mock<ILoggerFactory>();
        //    var mockLogger = new Mock<ILogger>();
        //    mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
        //        .Returns(mockLogger.Object);
        //    var mockMapper = new Mock<IMapper>();
        //    var mockUserService = new Mock<IUserService>();
        //    var mockExportService = new Mock<IExportService>();
        //    var mockWorkoutService = new Mock<IWorkoutService>();
        //    var mockChallengeService = new Mock<IChallengeService>();

        //    var manageController = new ManageController(
        //        mockUserManager.Object,
        //        mockSignInManager.Object,
        //        mockIdentityCookieOptions,
        //        mockEmailSender.Object,
        //        mockLoggerFactory.Object,
        //        mockMapper.Object,
        //        mockUserService.Object,
        //        mockExportService.Object,
        //        mockWorkoutService.Object,
        //        mockChallengeService.Object);

        //    mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        //        .ReturnsAsync(new User())
        //        .Verifiable(); // Returns user 

        //    mockExportService.Setup(e => e.GetExportTokenAsync((It.IsAny<int>())))
        //        .ReturnsAsync("token")
        //        .Verifiable(); // Return token 

        //    // TODO mock HttpContext for Request.Host in controller

        //    // Act
        //    var result = await manageController.GetExportLink() as RedirectToActionResult;

        //    // Assert
        //    Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", result.ActionName);
        //    Assert.Equal("Home", result.ControllerName);
        //    Assert.True(result.RouteValues.Count == 1);
        //    // Assert on ParamsDto route value    
        //}

        [Fact(DisplayName = "ExportLink returns view error if exception thrown")]
        public async Task GetExportLink_ReturnsViewError_IfExceptionThrown()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            // Act
            var result = await manageController.GetExportLink() as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("/Error", result.ViewName);
        }

        [Fact(DisplayName = "DeleteAllWorkoutData returns redirect action when delete success")]
        public async Task DeleteAllWorkoutData_ReturnsRedirectAction_WhenDeleteSuccess()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
               .ReturnsAsync(new User())
               .Verifiable(); // Returns user 

            mockWorkoutService.Setup(w => w.DeleteAllWorkoutAsync(It.IsAny<int>()))
                .ReturnsAsync(true)
                .Verifiable(); // Delete success
            
            // Act
            var result = await manageController.DeleteAllWorkoutData() as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.DeleteAllWorkoutDataSuccess, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "DeleteAllWorkoutData returns redirect action when delete error")]
        public async Task DeleteAllWorkoutData_ReturnsRedirectAction_WhenDeleteError()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
               .ReturnsAsync(new User())
               .Verifiable(); // Returns user 

            mockWorkoutService.Setup(w => w.DeleteAllWorkoutAsync(It.IsAny<int>()))
                .ReturnsAsync(false)
                .Verifiable(); // Delete error

            // Act
            var result = await manageController.DeleteAllWorkoutData() as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.DeleteAllWorkoutDataError, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "DeleteAllChallengeData returns redirect action when delete success")]
        public async Task DeleteAllChallengeData_ReturnsRedirectAction_WhenDeleteSuccess()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
               .ReturnsAsync(new User())
               .Verifiable(); // Returns user 

            mockChallengeService.Setup(w => w.DeleteAllChallengesAsync(It.IsAny<int>()))
                .ReturnsAsync(true)
                .Verifiable(); // Delete success

            // Act
            var result = await manageController.DeleteAllChallengeData() as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.DeleteAllChallengeDataSuccess, result.RouteValues["Message"]);
        }

        [Fact(DisplayName = "DeleteAllChallengeData returns redirect action when delete success error")]
        public async Task DeleteAllChallengeData_ReturnsRedirectAction_WhenDeleteError()
        {
            // Arrange 
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
            var mockUserManager = new Mock<FakeUserManager>();
            var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
            var mockIdentityCookieOptions = new FakeIOptions();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockExportService = new Mock<IExportService>();
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockChallengeService = new Mock<IChallengeService>();

            var manageController = new ManageController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockIdentityCookieOptions,
                mockEmailSender.Object,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockUserService.Object,
                mockExportService.Object,
                mockWorkoutService.Object,
                mockChallengeService.Object);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
               .ReturnsAsync(new User())
               .Verifiable(); // Returns user 

            mockChallengeService.Setup(w => w.DeleteAllChallengesAsync(It.IsAny<int>()))
                .ReturnsAsync(false)
                .Verifiable(); // Delete error

            // Act
            var result = await manageController.DeleteAllChallengeData() as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", result.ActionName);
            Assert.True(result.RouteValues.Count == 1);
            Assert.Equal(ManageController.ManageMessageId.DeleteAllChallengeDataError, result.RouteValues["Message"]);
        }

        // TODO
        //[Fact(DisplayName = "UpdateConsentSettings returns redirect action when claims added")]
        //public async Task UpdateConsentSettings_ReturnsRedirectAction_WhenClaimsAddedSuccess()
        //{
        //    // Arrange 
        //    var context = new Mock<HttpContext>();
        //    var contextAccessor = new Mock<IHttpContextAccessor>();
        //    contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
        //    var mockUserManager = new Mock<FakeUserManager>();
        //    var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
        //    var mockIdentityCookieOptions = new FakeIOptions();
        //    var mockEmailSender = new Mock<IEmailSender>();
        //    var mockLoggerFactory = new Mock<ILoggerFactory>();
        //    var mockLogger = new Mock<ILogger>();
        //    mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
        //        .Returns(mockLogger.Object);
        //    var mockMapper = new Mock<IMapper>();
        //    var mockUserService = new Mock<IUserService>();
        //    var mockExportService = new Mock<IExportService>();
        //    var mockWorkoutService = new Mock<IWorkoutService>();
        //    var mockChallengeService = new Mock<IChallengeService>();

        //    var manageController = new ManageController(
        //        mockUserManager.Object,
        //        mockSignInManager.Object,
        //        mockIdentityCookieOptions,
        //        mockEmailSender.Object,
        //        mockLoggerFactory.Object,
        //        mockMapper.Object,
        //        mockUserService.Object,
        //        mockExportService.Object,
        //        mockWorkoutService.Object,
        //        mockChallengeService.Object);

        //    mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        //       .ReturnsAsync(new User())
        //       .Verifiable(); // Returns user 

        //    mockUserManager.Setup(u => u.AddClaimAsync(It.IsAny<User>(), It.IsAny<Claim>()))
        //        .ReturnsAsync(IdentityResult.Success)
        //        .Verifiable(); // Claims added success

        //    mockSignInManager.Setup(s => s.RefreshSignInAsync(It.IsAny<User>()))
        //        .Returns(Task.FromResult("Refresh sign in"))
        //        .Verifiable(); // Mock refresh 

        //    // TODO mock private method GetCurrentClaimsIdentity

        //    // Act
        //    var result = await manageController.UpdateConsentSettings("check", "check", "check", "check") as RedirectToActionResult;

        //    // Assert
        //    Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", result.ActionName);
        //    Assert.True(result.RouteValues.Count == 1);
        //    Assert.Equal("ConsentUpdateSuccess", result.RouteValues["Message"]);
        //}

        // TODO
        //[Fact(DisplayName = "UpdateConsentSettings returns redirect action when claims not added")]
        //public async Task UpdateConsentSettings_ReturnsRedirectAction_WhenClaimsNotAddedError()
        //{
        //    // Arrange 
        //    var context = new Mock<HttpContext>();
        //    var contextAccessor = new Mock<IHttpContextAccessor>();
        //    contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);
        //    var mockUserManager = new Mock<FakeUserManager>();
        //    var mockSignInManager = new Mock<FakeSignInManager>(contextAccessor.Object);
        //    var mockIdentityCookieOptions = new FakeIOptions();
        //    var mockEmailSender = new Mock<IEmailSender>();
        //    var mockLoggerFactory = new Mock<ILoggerFactory>();
        //    var mockLogger = new Mock<ILogger>();
        //    mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
        //        .Returns(mockLogger.Object);
        //    var mockMapper = new Mock<IMapper>();
        //    var mockUserService = new Mock<IUserService>();
        //    var mockExportService = new Mock<IExportService>();
        //    var mockWorkoutService = new Mock<IWorkoutService>();
        //    var mockChallengeService = new Mock<IChallengeService>();

        //    var manageController = new ManageController(
        //        mockUserManager.Object,
        //        mockSignInManager.Object,
        //        mockIdentityCookieOptions,
        //        mockEmailSender.Object,
        //        mockLoggerFactory.Object,
        //        mockMapper.Object,
        //        mockUserService.Object,
        //        mockExportService.Object,
        //        mockWorkoutService.Object,
        //        mockChallengeService.Object);

        //    mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        //       .ReturnsAsync(new User())
        //       .Verifiable(); // Returns user 

        //    mockUserManager.Setup(u => u.AddClaimAsync(It.IsAny<User>(), It.IsAny<Claim>()))
        //        .ReturnsAsync(IdentityResult.Failed())
        //        .Verifiable(); // Claims add error 

        //    mockSignInManager.Setup(s => s.RefreshSignInAsync(It.IsAny<User>()))
        //        .Returns(Task.FromResult("Refresh sign in"))
        //        .Verifiable(); // Mock refresh 

        //    // TODO mock private method GetCurrentClaimsIdentity

        //    // Act
        //    var result = await manageController.UpdateConsentSettings("check", "check", "check", "check") as RedirectToActionResult;

        //    // Assert
        //    Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", result.ActionName);
        //    Assert.True(result.RouteValues.Count == 1);
        //    Assert.Equal("ConsentUpdateError", result.RouteValues["Message"]);
        //}

    }
}
