using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Core.Dto.PortableDto;
using LiftLog.Data.Interfaces;
using LiftLog.Service.EmailService;
using LiftLog.Service.Interfaces;
using LiftLog.Service.UserServices;
using LiftLog.Web.Controllers.Web;
using LiftLog.Web.ViewModels.WorkoutViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Org.BouncyCastle.Asn1.Cms.Ecc;
using Xunit;
using Xunit.Sdk;

namespace LiftLog.Web.Tests
{

    /// <summary>
    /// This class tests the controller logic of the WorkoutController and tests how it behaves based on valid and invalid inputs when creating workouts etc.
    /// This includes the following controller responsibilities:
    /// 1) Verify ModelState.IsValid
    /// 2) Return error response if Modelstate is invalid
    /// 3) Retrieve a business entity from persistence 
    /// 4) Perform action on business entity
    /// 5) Save business entity to persistence
    /// 6) Return appropriate IActionResult
    /// Note that 3-5 are tested in the service layer. 
    /// </summary>
    public class WorkoutControllerTest
    {

        [Fact(DisplayName = "Workouts returns view result with a list of workouts")]
        public async Task Workouts_ReturnsAViewResult_WithAListofWorkouts()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var workouts = new List<WorkoutDto> {new WorkoutDto {}};
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.GetAllWorkoutsAsync(It.IsAny<int>()))
                .ReturnsAsync(workouts)
                .Verifiable();
            var mockExerciseService = new Mock<IExerciseService>();
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();
            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);


            // Act
            var viewResult = await workoutController.Workouts() as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(viewResult);
            var model = Assert.IsAssignableFrom<IEnumerable<WorkoutViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(0, model.Count());

        }

        [Fact(DisplayName = "Workouts returns view error result for empty list of workouts")]
        public async Task Workouts_ReturnsAViewErrorResult_WithEmptyListofWorkouts()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.GetAllWorkoutsAsync(It.IsAny<int>()))
                .ReturnsAsync((List<WorkoutDto>) null)
                .Verifiable(); // Return empty list of workouts
            var mockExerciseService = new Mock<IExerciseService>();
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();
            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            // Act
            var viewResult = await workoutController.Workouts() as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(viewResult);
            Assert.Equal("/Error", viewResult.ViewName);

        }

        [Fact(DisplayName = "EditWorkout returns a view result with a workout")]
        public async Task EditWorkout_ReturnsAView_WithWorkout()
        {
            // Arrange 
            var workoutDto = new WorkoutDto() { Name = "Workout", CreationDate = DateTime.MinValue, Id = 1, UserId = 1,
                WorkoutEntryDtos = new List<WorkoutEntryDto> { new WorkoutEntryDto { ExerciseId = 1, ExerciseName = "Squat", Id = 1, Reps = 10, Set = 10, Weight = 10 }}};
            var exercises = new List<ExerciseDto>
            {
                new ExerciseDto { Description = "Description", Id = 1, Name = "Squat" }
            };
            var workoutEntries = new List<WorkoutEntryViewModel>
            {
                new WorkoutEntryViewModel {ExerciseName = "Squat", Id = 1, Reps = 10, Set = 10, Weight = 10}
            };
            var workoutViewModel = new WorkoutViewModel() { CreationDate = DateTime.MinValue, ExerciseId = 1, Name = "Workout", Exercises = exercises, Id = 1,
                WorkoutEntries = workoutEntries, Reps = 10, Weight = 10, Set = 10 };

            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<WorkoutViewModel>(workoutDto))
                .Returns(workoutViewModel)
                .Verifiable();
            mockMapper.Setup(m => m.Map<ICollection<WorkoutEntryDto>, IEnumerable<WorkoutEntryViewModel>>(It.IsAny<ICollection<WorkoutEntryDto>>()))
                .Returns(workoutEntries)
                .Verifiable();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.GetWorkoutAsync(It.IsAny<int>()))
                .ReturnsAsync(workoutDto)
                .Verifiable(); // Return workout
            var mockExerciseService = new Mock<IExerciseService>();
            mockExerciseService.Setup(e => e.GetAllExercisesAsync())
                .ReturnsAsync(exercises)
                .Verifiable(); // Returns exercises 
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();
            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            // Act 
            var result = await workoutController.EditWorkout(It.IsAny<int>()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            //Assert.Equal("EditWorkout", result.ViewName);
            var model = Assert.IsType<WorkoutViewModel>(result.Model);
            Assert.Equal(DateTime.MinValue, model.CreationDate);
            Assert.Equal(1, model.ExerciseId);
            Assert.Equal("Workout", model.Name);
            Assert.Equal(exercises, model.Exercises);
            Assert.Equal(1, model.Id);
            Assert.Equal(workoutEntries, model.WorkoutEntries);
            Assert.Equal(10, model.Reps);
            Assert.Equal(10, model.Weight);
            Assert.Equal(10, model.Set);
        }

        [Fact(DisplayName = "EditWorkout returns a view error for empty workout or exercise")]
        public async Task EditWorkout_ReturnsError_EmptyWorkout()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.GetWorkoutAsync(It.IsAny<int>()))
                .ReturnsAsync((WorkoutDto) null)
                .Verifiable(); // Return workout
            var mockExerciseService = new Mock<IExerciseService>();
            mockExerciseService.Setup(e => e.GetAllExercisesAsync())
                .ReturnsAsync((ICollection<ExerciseDto>)null)
                .Verifiable(); // Returns exercises 
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();
            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            // Act 
            var result = await workoutController.EditWorkout(It.IsAny<int>()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("/Error", result.ViewName);
        }

        [Fact(DisplayName = "EditWorkout POST request returns a view error for empty workout model")]
        public async Task EditWorkoutPost_ReturnsError_EmptyModel()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.GetWorkoutAsync(It.IsAny<int>()))
                .ReturnsAsync((WorkoutDto)null)
                .Verifiable(); // Return workout
            var mockExerciseService = new Mock<IExerciseService>();
            mockExerciseService.Setup(e => e.GetAllExercisesAsync())
                .ReturnsAsync((ICollection<ExerciseDto>)null)
                .Verifiable(); // Returns exercises 
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();
            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            // Act 
            var result = await workoutController.EditWorkout((WorkoutViewModel)null) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("/Error", result.ViewName);
        }

        [Fact(DisplayName = "EditWorkout POST returns a view error when database could not add workout entry")]
        public async Task EditWorkoutPost_ReturnsError_WorkoutResultNotAdded()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.AddWorkoutEntryAsync(It.IsAny<AddWorkoutEntryToWorkoutDto>()))
                .ReturnsAsync(0) // Workout result not added 
                .Verifiable(); 
            var mockExerciseService = new Mock<IExerciseService>();
            //mockExerciseService.Setup(e => e.GetAllExercisesAsync())
            //    .ReturnsAsync((ICollection<ExerciseDto>)null)
            //    .Verifiable(); // Returns exercises 
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();
            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            // Act 
            var result = await workoutController.EditWorkout(It.IsAny<WorkoutViewModel>()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("/Error", result.ViewName);
        }

        // TODO integration test of update ?
        //[Fact(DisplayName = "EditWorkout POST returns a view result with an updated workout model")]
        //public async Task EditWorkoutPost_ReturnsViewResult_UpdatedWorkout()
        //{
        //    // Arrange 
        //    var mockUserManager = new FakeUserManager();
        //    var mockMapper = new Mock<IMapper>();

        //    var workoutViewModel = new WorkoutViewModel() { Set = 10, Weight = 10, Reps = 10 };

        //    mockMapper.Setup(m => m.Map<WorkoutViewModel>(It.IsAny<WorkoutDto>()))
        //        .Returns(new WorkoutViewModel())
        //        .Verifiable();
        //    mockMapper.Setup(m => m.Map<ICollection<WorkoutEntryDto>, IEnumerable<WorkoutEntryViewModel>>(new List<WorkoutEntryDto>()))
        //        .Returns(new List<WorkoutEntryViewModel>())
        //        .Verifiable();
        //    var mockLoggerFactory = new Mock<ILoggerFactory>();
        //    var mockLogger = new Mock<ILogger>();
        //    mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
        //    var mockWorkoutService = new Mock<IWorkoutService>();
        //    mockWorkoutService.Setup(w => w.GetWorkoutAsync(It.IsAny<int>()))
        //        .ReturnsAsync((WorkoutDto)null)
        //        .Verifiable(); // Return workout
        //    mockWorkoutService.Setup(w => w.AddWorkoutEntryAsync(It.IsAny<AddWorkoutEntryToWorkoutDto>()))
        //        .ReturnsAsync(1)
        //        .Verifiable();
        //    var mockExerciseService = new Mock<IExerciseService>();
        //    mockExerciseService.Setup(e => e.GetAllExercisesAsync())
        //        .ReturnsAsync((ICollection<ExerciseDto>)null)
        //        .Verifiable(); // Returns exercises 
        //    var mockChallengeService = new Mock<IChallengeService>();
        //    var mockWorkoutRepository = new Mock<IWorkoutRepository>();
        //    var workoutController = new WorkoutController(
        //        mockLoggerFactory.Object,
        //        mockMapper.Object,
        //        mockWorkoutService.Object,
        //        mockExerciseService.Object,
        //        mockUserManager,
        //        mockWorkoutRepository.Object,
        //        mockChallengeService.Object);

        //    // Act 
        //    var result = await workoutController.EditWorkout(workoutViewModel) as ViewResult;

        //    // Assert
        //    Assert.IsType<ViewResult>(result);
        //    Assert.IsType<WorkoutViewModel>(result.Model);
        //    Assert.Equal(true, result.ViewData.ModelState.IsValid);
        //}

        [Fact(DisplayName = "EditWorkout POST returns view result when model state is valid")]
        public async Task EditWorkoutPost_ReturnsViewResult_WhenModelStateIsValid()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<WorkoutViewModel>(It.IsAny<WorkoutDto>()))
                .Returns(new WorkoutViewModel())
                .Verifiable();
            mockMapper.Setup(m => m.Map<ICollection<WorkoutEntryDto>, IEnumerable<WorkoutEntryViewModel>>(new List<WorkoutEntryDto>()))
                .Returns(new List<WorkoutEntryViewModel>())
                .Verifiable();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.GetWorkoutAsync(It.IsAny<int>()))
                .ReturnsAsync(new WorkoutDto())
                .Verifiable(); // Return workout
            mockWorkoutService.Setup(w => w.AddWorkoutEntryAsync(It.IsAny<AddWorkoutEntryToWorkoutDto>()))
                .ReturnsAsync(1)
                .Verifiable();
            var mockExerciseService = new Mock<IExerciseService>();
            mockExerciseService.Setup(e => e.GetAllExercisesAsync())
                .ReturnsAsync(new List<ExerciseDto>())
                .Verifiable(); // Returns exercises 
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();
            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);
            var invalidWorkout = new WorkoutViewModel() { Set = -10, Weight = -10, Reps = -10 };

            // Act 
            var result = await workoutController.EditWorkout(invalidWorkout) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<WorkoutViewModel>(result.Model);
            Assert.Equal(true, result.ViewData.ModelState.IsValid);
        }

        [Fact(DisplayName = "EditWorkout POST returns a view result to retry when model state is invalid")]
        public async Task EditWorkoutPost_ReturnsError_WhenModelStateIsInValid()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.GetWorkoutAsync(It.IsAny<int>()))
                .ReturnsAsync((WorkoutDto)null)
                .Verifiable(); // Return workout
            var mockExerciseService = new Mock<IExerciseService>();
            mockExerciseService.Setup(e => e.GetAllExercisesAsync())
                .ReturnsAsync((ICollection<ExerciseDto>)null)
                .Verifiable(); // Returns exercises 
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();
            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);
            var invalidWorkout = new WorkoutViewModel() { Set = -10, Weight = -10, Reps = -10}; 

            workoutController.ModelState.AddModelError("", "");

            // Act 
            var result = await workoutController.EditWorkout(invalidWorkout) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<WorkoutViewModel>(result.Model);
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
        }

        [Fact(DisplayName = "AddWorkout returns a view error when empty workout or exercise")]
        public async Task AddWorkout_ReturnsError_WhenEmptyWorkout()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.GetAllWorkoutsAsync(It.IsAny<int>()))
                .ReturnsAsync((List<WorkoutDto>) null)
                .Verifiable(); // Return null
            var mockExerciseService = new Mock<IExerciseService>();
            mockExerciseService.Setup(e => e.GetAllExercisesAsync())
                .ReturnsAsync((List<ExerciseDto>) null)
                .Verifiable();
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();
            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            // Act
            var result = await workoutController.AddWorkout() as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("/Error", result.ViewName);
        }

        [Fact(DisplayName = "AddWorkout returns a view error when database could not add workout")]
        public async Task AddWorkout_ReturnsViewError_Database()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.GetAllWorkoutsAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<WorkoutDto> { new WorkoutDto { } })
                .Verifiable(); // Return list of workouts
            mockWorkoutService.Setup(w => w.AddWorkoutAsync(It.IsAny<NewWorkoutDto>()))
                .ReturnsAsync(0) // Error in database 
                .Verifiable();
            mockWorkoutService.Setup(w => w.GetWorkoutAsync(It.IsAny<int>()))
                .ReturnsAsync(new WorkoutDto { WorkoutEntryDtos = new List<WorkoutEntryDto>() })
                .Verifiable();
            var mockExerciseService = new Mock<IExerciseService>();
            mockExerciseService.Setup(e => e.GetAllExercisesAsync())
                .ReturnsAsync(new List<ExerciseDto>())
                .Verifiable();
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();
            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            // Act
            var result = await workoutController.AddWorkout() as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("/Error", result.ViewName);
            
        }

        [Fact(DisplayName = "AddWorkout returns a view result with workout in redirect action")]
        public async Task AddWorkout_ReturnsEditRedirectViewResult_WithWorkoutModel()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var workoutEntries = new List<WorkoutEntryViewModel>();
            var exercises = new List<ExerciseDto>();
            var workoutViewModel = new WorkoutViewModel() { CreationDate = DateTime.MinValue, ExerciseId = 0, Name = "Workout",
                Exercises = exercises, Id = 0, Reps = 10, Weight = 10, Set = 10, WorkoutEntries = workoutEntries };
            mockMapper.Setup(m => m.Map<WorkoutDto, WorkoutViewModel>(It.IsAny<WorkoutDto>()))
                .Returns(workoutViewModel)
                .Verifiable();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.GetAllWorkoutsAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<WorkoutDto> { new WorkoutDto { } })
                .Verifiable(); // Return list of workouts
            mockWorkoutService.Setup(w => w.AddWorkoutAsync(It.IsAny<NewWorkoutDto>()))
                .ReturnsAsync(1)
                .Verifiable();
            mockWorkoutService.Setup(w => w.GetWorkoutAsync(It.IsAny<int>()))
                .ReturnsAsync(new WorkoutDto { WorkoutEntryDtos = new List<WorkoutEntryDto>() })
                .Verifiable();
            var mockExerciseService = new Mock<IExerciseService>();
            mockExerciseService.Setup(e => e.GetAllExercisesAsync())
                .ReturnsAsync(exercises);
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();
            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            //var expectedRedirectValues = new RouteValueDictionary()
            //{
            //    // TODO 
            //    { "Id", 0 },
            //    { "Name", "Workout" },
            //    { "CreationDate", DateTime.MinValue },
            //    //{ "WorkoutEntries", workoutEntries },
            //    //{ "Exercises", exercises },
            //    { "ExerciseId", 0 },
            //    { "Set", 10 },
            //    { "Weight", 10 },
            //    { "Reps", 10 }
            //};

            // Act
            var result = await workoutController.AddWorkout() as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("EditWorkout", result.ActionName);
            //Assert.Equal(expectedRedirectValues, result.RouteValues);
        }

        [Fact(DisplayName = "DeleteWorkout redirects to workouts upon deletion")]
        public async Task DeleteWorkout_RedirectToWorkouts_IfDeleted()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.DeleteWorkoutAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true)
                .Verifiable();
            var mockExerciseService = new Mock<IExerciseService>();
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();

            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            // Act 
            var result = await workoutController.DeleteWorkout(It.IsAny<int>()) as RedirectToActionResult;

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Workouts", result.ActionName);
        }

        [Fact(DisplayName = "DeleteWorkout returns view error if workout not deleted")]
        public async Task DeleteWorkout_ReturnsViewError_WhenNoDelete()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.DeleteWorkoutAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false)
                .Verifiable();
            var mockExerciseService = new Mock<IExerciseService>();
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();

            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            // Act 
            var result = await workoutController.DeleteWorkout(It.IsAny<int>()) as ViewResult;

            // Assert 
            Assert.IsType<ViewResult>(result);
            Assert.Equal("/Error", result.ViewName);
        }

        [Fact(DisplayName = "DeleteEntry redirects to specific edit page for workout upon deletion")]
        public async Task DeleteEntry_RedirectToEditWorkout_IfDeleted()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.DeleteWorkoutEntryAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true)
                .Verifiable();
            var mockExerciseService = new Mock<IExerciseService>();
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();

            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            var expectedRedirectValues = new RouteValueDictionary()
            {
                {"controller", "Workout"},
                {"action", "EditWorkout"},
                {"id", 0}
            };

            // Act 
            var result = await workoutController.DeleteEntry(It.IsAny<int>(), It.IsAny<int>()) as RedirectToActionResult;

            // Assert 
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("EditWorkout", result.ActionName);
            Assert.Equal(expectedRedirectValues, result.RouteValues);
        }

        [Fact(DisplayName = "DeleteEntry returns view error if workout entry not deleted")]
        public async Task DeleteEntry_ReturnsViewError_WhenNoDelete()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.DeleteWorkoutEntryAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false)
                .Verifiable();
            var mockExerciseService = new Mock<IExerciseService>();
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();

            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            // Act 
            var result = await workoutController.DeleteEntry(It.IsAny<int>(), It.IsAny<int>()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("/Error", result.ViewName);
        }

        [Fact(DisplayName = "Challenge returns view error if empty user")]
        public async Task Challenge_ReturnsViewError_IfEmptyUser()
        {
            // Arrange 
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            mockWorkoutService.Setup(w => w.GetWorkoutEntryAsync(It.IsAny<int>()))
                .ReturnsAsync((WorkoutEntryDto)null)
                .Verifiable();
            var mockExerciseService = new Mock<IExerciseService>();
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();

            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            // Act
            var result = await workoutController.Challenge(It.IsAny<int>(), It.IsAny<int>()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("/Error", result.ViewName);

        }

        [Fact(DisplayName = "Challenge returns view result with model")]
        public async Task Challenge_ReturnsViewResult_WithModel()
        {
            // Arrange 
            var workoutEntryDto = new WorkoutEntryDto() { Id = 1, ExerciseId = 1, ExerciseName = "Squat", Reps = 10, Set = 10, Weight = 10 };
            var challengeableUserDtos = new List<ChallengeAbleUserDto>()
            {
                new ChallengeAbleUserDto {DisplayName = "Test", Id = 1}
            };
            var challengeViewModel = new NewChallengeViewModel()
            {
                Id = 1,
                ExerciseId = 1,
                ChallengeAbleUserDtos = challengeableUserDtos,
                ChallengeeId = challengeableUserDtos.First().Id,
                ExerciseName = "Squat",
                Reps = 10,
                Set = 10, 
                Weight = 10
            };

            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<NewChallengeViewModel>(It.IsAny<WorkoutEntryDto>()))
                .Returns(challengeViewModel)
                .Verifiable();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            
            mockWorkoutService.Setup(w => w.GetWorkoutEntryAsync(It.IsAny<int>()))
                .ReturnsAsync(workoutEntryDto)
                .Verifiable();
            var mockExerciseService = new Mock<IExerciseService>();
            var mockChallengeService = new Mock<IChallengeService>();
            mockChallengeService.Setup(c => c.GetChallengeAbleUsersAsync(It.IsAny<int>()))
                .ReturnsAsync(challengeableUserDtos)
                .Verifiable();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();

            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            // Act
            var result = await workoutController.Challenge(It.IsAny<int>(), It.IsAny<int>()) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<NewChallengeViewModel>(result.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal(1, model.ExerciseId);
            Assert.Equal(challengeableUserDtos, model.ChallengeAbleUserDtos);
            Assert.Equal(1, model.ChallengeeId);
            Assert.Equal(10, model.Reps);
            Assert.Equal(10, model.Set);
            Assert.Equal(10, model.Weight);
            //Assert.Equal("Challenge", result);
        }

        // TODO 
        //[Fact(DisplayName = "Challenge Post request redirects to challenge view result upon creation")]
        //public async Task ChallengePost_ReturnsViewResult_IfChallengeCreated()
        //{
        //    // Arrange 
        //    var newChallengeDto = new NewChallengeDto() { Weight = 10, ChallengeeId = 1, ChallengerId = 2, ExerciseId = 1, Reps = 10 };
        //    var challengeableUserDtos = new List<ChallengeAbleUserDto>()
        //    {
        //        new ChallengeAbleUserDto {DisplayName = "Test", Id = 1}
        //    };

        //    var challengeViewModel = new NewChallengeViewModel()
        //    {
        //        Id = 1,
        //        ExerciseId = 1,
        //        ChallengeAbleUserDtos = challengeableUserDtos,
        //        ChallengeeId = challengeableUserDtos.First().Id,
        //        ExerciseName = "Squat",
        //        Reps = 10,
        //        Set = 10,
        //        Weight = 10
        //    };

        //    var mockUserManager = new FakeUserManager();
        //    var mockMapper = new Mock<IMapper>();
        //    mockMapper.Setup(m => m.Map<NewChallengeDto>(It.IsAny<NewChallengeViewModel>()))
        //        .Returns(newChallengeDto)
        //        .Verifiable();
        //    var mockLoggerFactory = new Mock<ILoggerFactory>();
        //    var mockLogger = new Mock<ILogger>();
        //    mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
        //    var mockWorkoutService = new Mock<IWorkoutService>();
        //    var mockExerciseService = new Mock<IExerciseService>();
        //    var mockChallengeService = new Mock<IChallengeService>();
        //    mockChallengeService.Setup(c => c.CreateChallengeAsync(It.IsAny<NewChallengeDto>()))
        //        .ReturnsAsync(1) // Result challenge created 
        //        .Verifiable();
        //    var mockWorkoutRepository = new Mock<IWorkoutRepository>();

        //    var workoutController = new WorkoutController(
        //        mockLoggerFactory.Object,
        //        mockMapper.Object,
        //        mockWorkoutService.Object,
        //        mockExerciseService.Object,
        //        mockUserManager,
        //        mockWorkoutRepository.Object,
        //        mockChallengeService.Object);

        //    // TODO mock Tempdata dictionary or set in constructor

        //    // Act 
        //    var result = await workoutController.Challenge(challengeViewModel) as RedirectToActionResult;

        //    // Assert 
        //    Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Challenges", result.ActionName);
        //    Assert.Equal("Challenge", result.ControllerName);
        //}

        [Fact(DisplayName = "Challenge post returns view error if challenge not created")]
        public async Task ChallengePost_ReturnsViewError_ChallengeNotCreated()
        {
            // Arrange 
            var newChallengeDto = new NewChallengeDto() { Weight = 10, ChallengeeId = 1, ChallengerId = 2, ExerciseId = 1, Reps = 10 };
            var challengeableUserDtos = new List<ChallengeAbleUserDto>()
            {
                new ChallengeAbleUserDto {DisplayName = "Test", Id = 1}
            };

            var challengeViewModel = new NewChallengeViewModel()
            {
                Id = 1,
                ExerciseId = 1,
                ChallengeAbleUserDtos = challengeableUserDtos,
                ChallengeeId = challengeableUserDtos.First().Id,
                ExerciseName = "Squat",
                Reps = 10,
                Set = 10,
                Weight = 10
            };

            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<NewChallengeDto>(It.IsAny<NewChallengeViewModel>()))
                .Returns(newChallengeDto)
                .Verifiable();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockExerciseService = new Mock<IExerciseService>();
            var mockChallengeService = new Mock<IChallengeService>();
            mockChallengeService.Setup(c => c.CreateChallengeAsync(It.IsAny<NewChallengeDto>()))
                .ReturnsAsync(0) // Result challenge not created 
                .Verifiable();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();

            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            // Act 
            var result = await workoutController.Challenge(challengeViewModel) as ViewResult;

            // Assert 
            Assert.IsType<ViewResult>(result);
            Assert.Equal("/Error", result.ViewName);
        }

        [Fact(DisplayName = "Challenge post returns bad request result if model state is invalid")]
        public async Task ChallengePost_ReturnsBadRequestResult_WhenModelStateIsInvalid()
        {
            // Arrange 
            var challengeViewModel = new NewChallengeViewModel()
            {
                Id = 1,
                ExerciseId = 1,
                ExerciseName = "Squat",
                Reps = -10,
                Set = -10,
                Weight = -10 
            };
            var mockUserManager = new FakeUserManager();
            var mockMapper = new Mock<IMapper>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var mockWorkoutService = new Mock<IWorkoutService>();
            var mockExerciseService = new Mock<IExerciseService>();
            var mockChallengeService = new Mock<IChallengeService>();
            var mockWorkoutRepository = new Mock<IWorkoutRepository>();

            var workoutController = new WorkoutController(
                mockLoggerFactory.Object,
                mockMapper.Object,
                mockWorkoutService.Object,
                mockExerciseService.Object,
                mockUserManager,
                mockWorkoutRepository.Object,
                mockChallengeService.Object);

            workoutController.ModelState.AddModelError("ExerciseName", "Required");

            // Act 
            var result = await workoutController.Challenge(challengeViewModel) as ViewResult;

            // Assert 
            Assert.IsType<ViewResult>(result);
            Assert.IsType<NewChallengeViewModel>(result.Model);
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
        }

        // TODO
        //[Fact(DisplayName = "Challenge post returns view result when model state is valid")]
        //public async Task ChallengePost_ReturnsViewResult_WhenModelStateIsValid()
        //{
        //    // Arrange 
        //    var challengeViewModel = new NewChallengeViewModel()
        //    {
        //        Id = 1,
        //        ExerciseId = 1,
        //        ExerciseName = "Squat",
        //        Reps = 10,
        //        Set = 10,
        //        Weight = 10
        //    };
        //    var mockUserManager = new FakeUserManager();
        //    var mockMapper = new Mock<IMapper>();
        //    var mockLoggerFactory = new Mock<ILoggerFactory>();
        //    var mockLogger = new Mock<ILogger>();
        //    mockLoggerFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
        //    var mockWorkoutService = new Mock<IWorkoutService>();
        //    var mockExerciseService = new Mock<IExerciseService>();
        //    var mockChallengeService = new Mock<IChallengeService>();
        //    var mockWorkoutRepository = new Mock<IWorkoutRepository>();

        //    var workoutController = new WorkoutController(
        //        mockLoggerFactory.Object,
        //        mockMapper.Object,
        //        mockWorkoutService.Object,
        //        mockExerciseService.Object,
        //        mockUserManager,
        //        mockWorkoutRepository.Object,
        //        mockChallengeService.Object);

        //    // TODO mock Tempdata dictionary or set in constructor

        //    // Act 
        //    var result = await workoutController.Challenge(challengeViewModel) as ViewResult;

        //    // Assert 
        //    Assert.IsType<ViewResult>(result);
        //    Assert.IsType<NewChallengeViewModel>(result.Model);
        //    Assert.Equal(false, result.ViewData.ModelState.IsValid);
        //}

        // TODO depends on private static Temp dictionary 
        //[Fact(DisplayName = "Challenge post returns view error if temp data is null")]
        //public async Task ChallengePost_ReturnsViewError_IfTempEmpty()
        //{
        //    throw new NotImplementedException();
        //}

    }
}
