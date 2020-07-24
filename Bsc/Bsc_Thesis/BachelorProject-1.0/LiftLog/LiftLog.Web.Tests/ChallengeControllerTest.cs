using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Service.Interfaces;
using LiftLog.Web.Controllers.Web;
using LiftLog.Web.ViewModels.ChallengeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LiftLog.Web.Tests
{

    /// <summary>
    /// This class tests the controller logic of the ChallengeController and tests how it behaves based on valid and invalid inputs when creating challenges.
    /// This includes the following controller responsibilities:
    /// 1) Verify ModelState.IsValid
    /// 2) Return error response if Modelstate is invalid
    /// 3) Retrieve a business entity from persistence 
    /// 4) Perform action on business entity
    /// 5) Save business entity to persistence
    /// 6) Return appropriate IActionResult
    /// Note that 3-5 are tested in the service layer. 
    /// </summary>
    public class ChallengeControllerTest
    {

        [Fact(DisplayName = "Challenges returns view result with challenges")]
        public async Task Challenges_ReturnsChallenges_WhenNotEmpty()
        {
            // Arrange
            var mockUserManager = new FakeUserManager();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var challenges = new List<ChallengeDto>();
            var mockChallengeService = new Mock<IChallengeService>();
            mockChallengeService.Setup(c => c.GetReceivedChallengesAsync(It.IsAny<int>()))
                .ReturnsAsync(challenges)
                .Verifiable();
            mockChallengeService.Setup(c => c.GetGivenChallengesAsync(It.IsAny<int>()))
                .ReturnsAsync(challenges)
                .Verifiable();

            var challengeController = new ChallengeController(
                mockUserManager,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockChallengeService.Object
                );

            // Act 
            var result = await challengeController.Challenges() as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ChallengeViewModel>(result.Model);
            Assert.Equal(challenges, model.GivenChallenges);
            Assert.Equal(challenges, model.ReceivedChallenges);

        }

        [Fact(DisplayName = "Challenges returns status code 500 for internal error when empty")]
        public async Task Challenges_ReturnsStatusCode500_WhenEmpty()
        {
            // Arrange
            var mockUserManager = new FakeUserManager();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockChallengeService = new Mock<IChallengeService>();

            var challengeController = new ChallengeController(
                mockUserManager,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockChallengeService.Object
                );
            
            // Act 
            var result = await challengeController.Challenges() as StatusCodeResult;

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact(DisplayName = "PostResults returns view result again to retry when model state is invalid")]
        public async Task PostResults_ReturnsViewResultAgain_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockUserManager = new FakeUserManager();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockChallengeService = new Mock<IChallengeService>();

            var challengeResultViewModel = new ChallengeResultViewModel();

            var challengeController = new ChallengeController(
                mockUserManager,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockChallengeService.Object
                );

            challengeController.ModelState.AddModelError("", "");

            // Act 
            var result = await challengeController.PostResults(challengeResultViewModel) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ChallengeResultViewModel>(result.Model);
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
        }

        [Fact(DisplayName = "PostResults returns redirect action when model state is valid")]
        public async Task PostResults_RedirectToAction_WhenModelStateIsValid()
        {
            // Arrange
            var mockUserManager = new FakeUserManager();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockChallengeService = new Mock<IChallengeService>();
            mockChallengeService.Setup(c => c.SetResultAsync(It.IsAny<ChallengeResultDto>()))
                .ReturnsAsync(true)
                .Verifiable();
            var challengeResultViewModel = new ChallengeResultViewModel();

            var challengeController = new ChallengeController(
                mockUserManager,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockChallengeService.Object
                );

            // Act 
            var result = await challengeController.PostResults(challengeResultViewModel) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Challenges", result.ActionName);
        }

        [Fact(DisplayName = "PostResults returns redirect action when challenge is set in challenge service")]
        public async Task PostResults_RedirectToAction_WhenDatabaseSuccess()
        {
            // Arrange
            var mockUserManager = new FakeUserManager();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<ChallengeResultDto>(It.IsAny<ChallengeResultViewModel>()))
                .Returns(new ChallengeResultDto())
                .Verifiable();
            var mockChallengeService = new Mock<IChallengeService>();
            mockChallengeService.Setup(c => c.SetResultAsync(It.IsAny<ChallengeResultDto>()))
                .ReturnsAsync(true)
                .Verifiable();
            var challengeResultViewModel = new ChallengeResultViewModel();

            var challengeController = new ChallengeController(
                mockUserManager,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockChallengeService.Object
                );

            // Act 
            var result = await challengeController.PostResults(challengeResultViewModel) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Challenges", result.ActionName);
        }

        [Fact(DisplayName = "PostResults returns status code 500 error when challenge is not set in service")]
        public async Task PostResults_ReturnsStatusCode500_WhenServiceError()
        {
            // Arrange
            var mockUserManager = new FakeUserManager();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockChallengeService = new Mock<IChallengeService>();
            mockChallengeService.Setup(c => c.SetResultAsync(It.IsAny<ChallengeResultDto>()))
                .ReturnsAsync(false)
                .Verifiable();
            var challengeResultViewModel = new ChallengeResultViewModel();

            var challengeController = new ChallengeController(
                mockUserManager,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockChallengeService.Object
                );

            // Act 
            var result = await challengeController.PostResults(challengeResultViewModel) as StatusCodeResult;

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact(DisplayName = "PostResults returns view result with model if challenge exists")]
        public async Task PostResults_ReturnsViewResult_IfChallengeExists()
        {
            // Arrange
            var mockUserManager = new FakeUserManager();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var challengeResultViewModel = new ChallengeResultViewModel();
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<ChallengeResultViewModel>(It.IsAny<ChallengeDto>()))
                .Returns(challengeResultViewModel)
                .Verifiable();
            var mockChallengeService = new Mock<IChallengeService>();
            mockChallengeService.Setup(c => c.GetChallengeAsync(It.IsAny<int>()))
                .ReturnsAsync(new ChallengeDto())
                .Verifiable();
            

            var challengeController = new ChallengeController(
                mockUserManager,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockChallengeService.Object
                );

            // Act 
            var result = await challengeController.PostResults(It.IsAny<int>()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ChallengeResultViewModel>(result.Model);
            // Assert.Equal(0, model.Id); TODO test model properties 
        }

        [Fact(DisplayName = "PostResults returns status code 500 if challenge is empty")]
        public async Task PostResults_ReturnsStatusCode500_IfChallengeEmpty()
        {
            // Arrange
            var mockUserManager = new FakeUserManager();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockChallengeService = new Mock<IChallengeService>();
            var challengeResultViewModel = new ChallengeResultViewModel();

            var challengeController = new ChallengeController(
                mockUserManager,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockChallengeService.Object
                );

            // Act 
            var result = await challengeController.PostResults(It.IsAny<int>()) as StatusCodeResult;

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact(DisplayName = "DeleteChallenge returns redirect action if challenge was deleted")]
        public async Task DeleteChallenge_ReturnsRedirectAction_IfChallengeDeleted()
        {
            // Arrange
            var mockUserManager = new FakeUserManager();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockChallengeService = new Mock<IChallengeService>();
            mockChallengeService.Setup(c => c.DeleteChallengeAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true) // Delete challenge success 
                .Verifiable(); 

            var challengeController = new ChallengeController(
                mockUserManager,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockChallengeService.Object
                );

            // Act 
            var result = await challengeController.DeleteChallenge(It.IsAny<int>()) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Challenges", result.ActionName);
        }

        [Fact(DisplayName = "DeleteChallenge returns status code 500 if challenge was not deleted")]
        public async Task DeleteChallenge_ReturnsStatusCode500_IfChallengeNotDeleted()
        {
            // Arrange
            var mockUserManager = new FakeUserManager();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockChallengeService = new Mock<IChallengeService>();
            mockChallengeService.Setup(c => c.DeleteChallengeAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false) // Delete challenge error 
                .Verifiable();

            var challengeController = new ChallengeController(
                mockUserManager,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockChallengeService.Object
                );

            // Act 
            var result = await challengeController.DeleteChallenge(It.IsAny<int>()) as StatusCodeResult;

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact(DisplayName = "ShareResult returns view result with model if challenge exists")]
        public async Task ShareResult_ReturnsViewResultWithModel_IfChallengeExists()
        {
            // Arrange
            var mockUserManager = new FakeUserManager();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<ShareChallengeViewModel>(It.IsAny<ChallengeDto>()))
                .Returns(new ShareChallengeViewModel())
                .Verifiable();
            var mockChallengeService = new Mock<IChallengeService>();
            mockChallengeService.Setup(c => c.GetChallengeAsync(It.IsAny<int>()))
                .ReturnsAsync(new ChallengeDto()) // Challenge exists  
                .Verifiable();

            var challengeController = new ChallengeController(
                mockUserManager,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockChallengeService.Object
                );

            // Act 
            var result = await challengeController.ShareResult(It.IsAny<int>()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ShareChallengeViewModel>(result.Model);
            // Assert.Equal(0, model.Reps); // TODO assert on all properties 
        }

        [Fact(DisplayName = "ShareResult returns status code 500 if challenge does not exist")]
        public async Task ShareResult_ReturnsStatusCode500_IfChallengeNotExists()
        {
            // Arrange
            var mockUserManager = new FakeUserManager();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            var mockMapper = new Mock<IMapper>();
            var mockChallengeService = new Mock<IChallengeService>();
            mockChallengeService.Setup(c => c.GetChallengeAsync(It.IsAny<int>()))
                .ReturnsAsync((ChallengeDto)null) // Challenge does not exist  
                .Verifiable();

            var challengeController = new ChallengeController(
                mockUserManager,
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockChallengeService.Object
                );

            // Act 
            var result = await challengeController.ShareResult(It.IsAny<int>()) as StatusCodeResult;

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, result.StatusCode);
        }

        //[Fact(DisplayName = "PostResults throws KeyNotFoundException when Temp Data is empty")]
        //public async Task PostResults_ThrowsException_WhenTempIsEmpty()
        //{
        //    // Arrange
        //    var mockUserManager = new FakeUserManager();
        //    var mockLoggerFactory = new Mock<ILoggerFactory>();
        //    var mockLogger = new Mock<ILogger>();
        //    mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
        //        .Returns(mockLogger.Object);
        //    var mockMapper = new Mock<IMapper>();
        //    var mockChallengeService = new Mock<IChallengeService>();
        //    var challengeResultViewModel = new ChallengeResultViewModel();

        //    var challengeController = new ChallengeController(
        //        mockUserManager,
        //        mockLoggerFactory.Object,
        //        mockMapper.Object,
        //        mockChallengeService.Object
        //        );

        //    // Act and Assert
        //    var result = await Assert.ThrowsAsync<KeyNotFoundException>(() => challengeController.PostResults(challengeResultViewModel));
        //}

    }

}
