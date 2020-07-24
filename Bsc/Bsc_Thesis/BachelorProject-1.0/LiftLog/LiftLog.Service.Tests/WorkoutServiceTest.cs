// WorkoutServiceTest.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using LiftLog.Service.UserServices;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LiftLog.Service.Tests
{
    /// <summary>
    ///     Tests for workout service. The test suit will test CRUD operations
    ///     of the workout service as well as negative testing with invalid data
    /// </summary>
    public class WorkoutServiceTest
    {
        [Fact(DisplayName = "AddWorkoutEntryAsync - success - Returns 2 ")]
        public async Task AddWorkoutEntryReturnsTrue()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);


            var exercise = new Exercise
            {
                Id = 1,
                Name = "Benchpress"
            };

            var workout = new Workout
            {
                Id = 1,
                CreationDate = DateTime.Now,
                UserId = 0,
                WorkoutEntries = new List<WorkoutEntry>()
            };
            var user = new User
            {
                Id = 1,
                Workouts = new List<Workout> {workout}
            };

            var dto = new AddWorkoutEntryToWorkoutDto
            {
                UserId = 1,
                WorkoutId = 1,
                WorkoutEntryDtos = new List<NewWorkoutEntryDto>
                {
                    new NewWorkoutEntryDto
                    {
                        ExerciseId = 1,
                        Set = 1,
                        Reps = 10,
                        Weight = 35
                    },
                    new NewWorkoutEntryDto
                    {
                        ExerciseId = 2,
                        Set = 1,
                        Reps = 15,
                        Weight = 70
                    }
                }
            };

            userRepoMock.Setup(r => r.GetUser(It.IsAny<int>())).ReturnsAsync(user);
            workoutRepoMock.Setup(r => r.GetWorkoutWithEntries(It.IsAny<int>())).ReturnsAsync(workout);
            userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);
            exerciseRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(exercise);
            mockMapper.Setup(m => m.Map<WorkoutEntry>(It.IsAny<WorkoutEntryDto>())).Returns(new WorkoutEntry());


            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.AddWorkoutEntryAsync(dto);

            //Asert
            Assert.True(result == 2); //Two entries are added to the workout
        }


        [Fact(DisplayName = "DeleteWorkoutAsync with  existing user DTO and workout - Databases Called once")]
        public async Task DeleteWorkoutDatabaseCalledOnce()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            const int userId = 1;
            const int workoutid = 1;

            userRepoMock.Setup(r => r.GetUser(It.IsAny<int>())).ReturnsAsync(new User());
            workoutRepoMock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);


            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.DeleteWorkoutAsync(userId, workoutid);

            //Asert
            userRepoMock.Verify(r => r.GetUser(It.IsAny<int>()));
            workoutRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "DeleteWorkoutAsync with  None user - return false")]
        public async Task DeleteWorkoutDatabaseReturnFalse()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            const int userId = 1;
            const int workoutid = 1;

            userRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync((User) null);
            workoutRepoMock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);


            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.DeleteWorkoutAsync(userId, workoutid);

            //Asert
            Assert.False(result);
        }

        [Fact(DisplayName = "DeleteWorkoutEntryAsync - valid entryId and userID - Databases called once")]
        public async Task DeleteWorkoutEntryDatabaseCalledOnce()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            const int userId = 1;
            const int entryId = 1;

            userRepoMock.Setup(r => r.GetUser(It.IsAny<int>())).ReturnsAsync(new User());
            workoutEntryRepoMock.Setup(r => r.DeleteAsync(entryId)).ReturnsAsync(true);

            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.DeleteWorkoutEntryAsync(userId, entryId);

            //Asert
            userRepoMock.Verify(r => r.GetUser(It.IsAny<int>()), Times.Once);
            workoutEntryRepoMock.Verify(r => r.DeleteAsync(entryId), Times.Once);
        }


        [Fact(DisplayName = "DeleteWorkoutEntryAsync - invalid entryId and valid userID- Returns false")]
        public async Task DeleteWorkoutEntryInvalidEntryIdReturnFalse()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            const int userId = 1;
            const int entryId = -1;

            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.DeleteWorkoutEntryAsync(userId, entryId);

            //Asert
            Assert.False(result);
        }


        [Fact(DisplayName = "DeleteWorkoutEntryAsync - valid entryId and invalid userID- Returns false")]
        public async Task DeleteWorkoutEntryInvalidUserIdReturnFalse()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            const int userId = -1;
            const int entryId = 1;

            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.DeleteWorkoutEntryAsync(userId, entryId);

            //Asert
            Assert.False(result);
        }

        [Fact(DisplayName = "DeleteWorkoutEntryAsync - valid entryId and userID- Returns true")]
        public async Task DeleteWorkoutEntryReturnTrue()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            const int userId = 1;
            const int entryId = 1;

            userRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(new User());
            workoutEntryRepoMock.Setup(r => r.DeleteAsync(entryId)).ReturnsAsync(true);

            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.DeleteWorkoutEntryAsync(userId, entryId);

            //Asert
            Assert.True(result);
        }

        [Fact(DisplayName = "DeleteWorkoutAsync with  existing user DTO and workout - Return true")]
        public async Task DeleteWorkoutReturnsTrue()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            const int userId = 1;
            const int workoutid = 1;

            userRepoMock.Setup(r => r.GetUser(It.IsAny<int>())).ReturnsAsync(new User());
            workoutRepoMock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);


            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.DeleteWorkoutAsync(userId, workoutid);

            //Asert
            Assert.True(result);
        }

        [Fact(DisplayName = "DeleteWorkoutAsync with  None user - user database called once, others never")]
        public async Task DeleteWorkoutUserDatabaseCalledOnceOthersNever()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            const int userId = 1;
            const int workoutid = 1;

            userRepoMock.Setup(r => r.GetUser(It.IsAny<int>())).ReturnsAsync((User) null);
            workoutRepoMock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);


            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.DeleteWorkoutAsync(userId, workoutid);

            //Asert
            userRepoMock.Verify(r => r.GetUser(It.IsAny<int>()));
            Times.Once();
            workoutRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }


        [Fact(DisplayName = "GetAllWorkoutsAsync - Existing user - Returns list of workouts")]
        public async Task GetAllWorkoutExistingUserReturnsLListOfworkouts()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();

            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
            mockMapper.Setup(m => m.Map<WorkoutEntryDto>(It.IsAny<WorkoutEntry>())).Returns(new WorkoutEntryDto());
            mockMapper.Setup(m => m.Map<WorkoutDto>(It.IsAny<Workout>())).Returns(new WorkoutDto());


            var exercise = new Exercise
            {
                Id = 1,
                Name = "Benchpress"
            };

            var workouts = new List<Workout>
            {
                new Workout
                {
                    Id = 1,
                    CreationDate = DateTime.Now,
                    UserId = 0,
                    User = new User(),
                    WorkoutEntries = new List<WorkoutEntry>
                    {
                        new WorkoutEntry
                        {
                            Id = 0,
                            Exercise = exercise,
                            ExerciseId = exercise.Id,
                            Reps = 10,
                            Set = 1,
                            Weight = 10
                        },
                        new WorkoutEntry
                        {
                            Id = 0,
                            Exercise = exercise,
                            ExerciseId = exercise.Id,
                            Reps = 11,
                            Set = 2,
                            Weight = 12
                        }
                    }
                }
            };

            var user = new User
            {
                Id = 1,
                Workouts = workouts
            };

            userRepoMock.Setup(r => r.GetUser(It.IsAny<int>())).ReturnsAsync(user);


            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.GetAllWorkoutsAsync(user.Id);

            //Asert
            Assert.True(result.Count == 1);
        }

        [Fact(DisplayName = "GetAllWorkoutsAsync - invalid userId - Returns null")]
        public async Task GetAllWorkoutInvalidUserIdReturnsNull()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            var userId = -1;

            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.GetAllWorkoutsAsync(userId);

            //Asert
            Assert.Null(result);
        }

        [Fact(DisplayName = "GetAllWorkoutsAsync - None Existing user - Returns null")]
        public async Task GetAllWorkoutNoneExistingUserReturnsNull()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            var userId = 1;

            userRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync((User) null);


            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.GetAllWorkoutsAsync(userId);

            //Asert
            Assert.Null(result);
        }


        [Fact(DisplayName = "GetWorkoutOnDateAsync - Existing user - Returns list of workouts")]
        public async Task GetWorkoutOnDateUserReturnsLListOfworkouts()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
            mockMapper.Setup(m => m.Map<WorkoutEntryDto>(It.IsAny<WorkoutEntry>())).Returns(new WorkoutEntryDto());
            mockMapper.Setup(m => m.Map<WorkoutDto>(It.IsAny<Workout>())).Returns(new WorkoutDto());

            var searchDate = DateTime.Now;

            var exercise = new Exercise
            {
                Id = 1,
                Name = "Benchpress"
            };

            var workouts = new List<Workout>
            {
                new Workout
                {
                    Id = 1,
                    CreationDate = searchDate,
                    UserId = 1,
                    User = new User(),
                    WorkoutEntries = new List<WorkoutEntry>
                    {
                        new WorkoutEntry
                        {
                            Id = 1,
                            Exercise = exercise,
                            ExerciseId = exercise.Id,
                            Reps = 10,
                            Set = 1,
                            Weight = 10
                        },
                        new WorkoutEntry
                        {
                            Id = 1,
                            Exercise = exercise,
                            ExerciseId = exercise.Id,
                            Reps = 11,
                            Set = 2,
                            Weight = 12
                        }
                    }
                },
                new Workout
                {
                    Id = 1,
                    CreationDate = DateTime.Now.AddYears(-1),
                    UserId = 1,
                    User = new User(),
                    WorkoutEntries = new List<WorkoutEntry>
                    {
                        new WorkoutEntry
                        {
                            Id = 1,
                            Exercise = exercise,
                            ExerciseId = exercise.Id,
                            Reps = 10,
                            Set = 1,
                            Weight = 10
                        },
                        new WorkoutEntry
                        {
                            Id = 1,
                            Exercise = exercise,
                            ExerciseId = exercise.Id,
                            Reps = 11,
                            Set = 2,
                            Weight = 12
                        }
                    }
                }
            };

            var user = new User
            {
                Id = 1,
                Workouts = workouts
            };

            userRepoMock.Setup(r => r.GetUser(It.IsAny<int>())).ReturnsAsync(user);
            workoutRepoMock.Setup(r => r.GetWorkoutWithEntries(It.IsAny<int>())).ReturnsAsync(new Workout
            {
                WorkoutEntries = new List<WorkoutEntry>()
            });


            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.GetWorkoutOnDateAsync(user.Id, searchDate);

            //Asert
            Assert.True(result.Count == 1);
        }


        [Fact(DisplayName = "AddWorkoutAsync with none existing user DTO - Return -1")]
        public async Task SaveNewWorkouNoUserReturnMinusOne()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            var workoutEntries = new List<NewWorkoutEntryDto>
            {
                new NewWorkoutEntryDto
                {
                    ExerciseId = 1,
                    Set = 1,
                    Reps = 10,
                    Weight = 20
                }
            };

            var dto = new NewWorkoutDto
            {
                Name = "Name",
                UserId = 1
            };

            mockMapper.Setup(m => m.Map<WorkoutEntry>(It.IsAny<NewWorkoutEntryDto>())).Returns(new WorkoutEntry());
            userRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync((User) null);


            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.AddWorkoutAsync(dto);

            //Asert
            Assert.True(result == -1);
        }

        [Fact(DisplayName = "AddWorkoutAsync with none existing user DTO - Userdatabase called once, others never")]
        public async Task SaveNewWorkouNoUserReturnOnceUserDatabaseNeverOtherDatabases()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);


            var workoutEntries = new List<NewWorkoutEntryDto>
            {
                new NewWorkoutEntryDto
                {
                    ExerciseId = 1,
                    Set = 1,
                    Reps = 10,
                    Weight = 20
                }
            };

            var dto = new NewWorkoutDto
            {
                Name = "Name",
                UserId = 1
            };

            mockMapper.Setup(m => m.Map<WorkoutEntry>(It.IsAny<NewWorkoutEntryDto>())).Returns(new WorkoutEntry());
            userRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync((User) null);


            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.AddWorkoutAsync(dto);

            //Asert
            userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
            workoutRepoMock.Verify(r => r.CreateAsync(It.IsAny<Workout>()), Times.Never);
            exerciseRepoMock.Verify(r => r.FindAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact(DisplayName = "AddWorkoutAsync with null DTO - Database has not been called")]
        public async Task SaveNewWorkoutNullDatabasesNotCalled()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);


            mockMapper.Setup(m => m.Map<WorkoutEntry>(It.IsAny<NewWorkoutEntryDto>())).Returns(new WorkoutEntry());

            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.AddWorkoutAsync(null);

            //Asert
            userRepoMock.Verify(r => r.FindAsync(It.IsAny<int>()), Times.Never);
            userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
            workoutRepoMock.Verify(r => r.CreateAsync(It.IsAny<Workout>()), Times.Never);
            exerciseRepoMock.Verify(r => r.FindAsync(It.IsAny<int>()), Times.Never);
        }


        [Fact(DisplayName = "AddWorkoutAsync with null DTO - Return -1")]
        public async Task SaveNewWorkoutNullReturnMinusOne()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            mockMapper.Setup(m => m.Map<WorkoutEntry>(It.IsAny<NewWorkoutEntryDto>())).Returns(new WorkoutEntry());

            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.AddWorkoutAsync(null);

            //Asert
            Assert.True(result == -1);
        }

        [Fact(DisplayName = "AddWorkoutAsync with valid DTO - Return int ID of workout")]
        public async Task SaveNewWorkoutValidDtoReturnsId()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var workoutEntryRepoMock = new Mock<IWorkoutEntryRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            var user = new User
            {
                Workouts = new List<Workout>()
            };

            var workoutEntries = new List<NewWorkoutEntryDto>
            {
                new NewWorkoutEntryDto
                {
                    ExerciseId = 1,
                    Set = 1,
                    Reps = 10,
                    Weight = 20
                }
            };

            var dto = new NewWorkoutDto
            {
                UserId = 1,
                Name = "name"
            };

            userRepoMock.Setup(r => r.GetUser(It.IsAny<int>())).ReturnsAsync(user);
            workoutRepoMock.Setup(r => r.CreateAsync(It.IsAny<Workout>())).ReturnsAsync(1);
            exerciseRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(new Exercise());

            mockMapper.Setup(m => m.Map<WorkoutEntry>(It.IsAny<NewWorkoutEntryDto>())).Returns(new WorkoutEntry());

            var SUT = new WorkoutService(loggerMock.Object, mockMapper.Object, userRepoMock.Object,
                workoutRepoMock.Object,
                exerciseRepoMock.Object, workoutEntryRepoMock.Object);

            //Act
            var result = await SUT.AddWorkoutAsync(dto);

            //Asert
            Assert.True(result > 0);
        }
    }
}