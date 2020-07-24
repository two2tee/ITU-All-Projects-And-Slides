// ExportServiceTests.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Collections.Generic;
using AutoMapper;
using LiftLog.Core.Dto.PortableDto;
using LiftLog.Core.Enums;
using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using LiftLog.Service.Interfaces;
using LiftLog.Service.UserServices;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LiftLog.Service.Tests
{
    /// <summary>
    /// Test suite for export service. The test will test if 
    /// portableobjects are created if upon valid request.
    /// It will also perform negative tests of invalid file creations and invalid tokens
    /// </summary>
    public class ExportServiceTests
    {
        [Fact(DisplayName = "ExportToFileAsync - file content array is empty returns null")]
        public void EExportToFileEmptyContentReturnsNull()
        {
            //Arrage
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();

            var mapperMock = new Mock<IMapper>();
            var fileProviderMock = new Mock<IFIleProvider<PortableUserDto>>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            var user = new User
            {
                Name = "Dennis",
                Sex = Sex.Male,
                BodyWeight = 100,
                Height = 190,
                BirthDate = DateTime.Now,
                Country = Country.Denmark,
                CreationDate = DateTime.Now,
                DisplayName = "Leet",
                Email = "abc@test.com",
                Id = 1,
                //GivenChallenges = new List<Challenge>(),
                //ReceivedChallenges = new List<Challenge>(),
                Workouts = new List<Workout>
                {
                    new Workout
                    {
                        Id = 0,
                        CreationDate = DateTime.Now,
                        Name = "first",
                        User = new User(),
                        UserId = 1,
                        WorkoutEntries = new List<WorkoutEntry>
                        {
                            new WorkoutEntry
                            {
                                Id = 0,
                                ExerciseId = 0,
                                Reps = 12,
                                Set = 1,
                                Weight = 50,
                                Exercise = new Exercise
                                {
                                    //Challenges = new List<Challenge>(),
                                    Description = "Compound",
                                    Name = "Bench",
                                    Id = 0
                                }
                            }
                        }
                    }
                }
            };

            userRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
            mapperMock.Setup(m => m.Map<PortableUserDto>(It.IsAny<User>())).Returns(new PortableUserDto());
            mapperMock.Setup(m => m.Map<PortableWorkoutDto>(It.IsAny<Workout>())).Returns(new PortableWorkoutDto());
            mapperMock.Setup(m => m.Map<PortableWorkoutEntryDto>(It.IsAny<WorkoutEntry>()))
                .Returns(new PortableWorkoutEntryDto());
            fileProviderMock.Setup(f => f.MakeFileAsync(It.IsAny<PortableUserDto>())).ReturnsAsync(new FileDto
            {
                FileContent = new byte[] {}
            });

            var SUT = new ExportService(loggerMock.Object, userRepoMock.Object, mapperMock.Object,
                fileProviderMock.Object, workoutRepoMock.Object, exerciseRepoMock.Object);
            var userid = 1;
            //Act

            Assert.Null(SUT.ExportToFileAsync(userid).Result);
        }


        [Fact(DisplayName = "ExportToFileAsync - file content is null returns null")]
        public void EExportToFileNullContentReturnsNull()
        {
            //Arrage
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();

            var fileProviderMock = new Mock<IFIleProvider<PortableUserDto>>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            var user = new User
            {
                Name = "Dennis",
                Sex = Sex.Male,
                BodyWeight = 100,
                Height = 190,
                BirthDate = DateTime.Now,
                Country = Country.Denmark,
                CreationDate = DateTime.Now,
                DisplayName = "Leet",
                Email = "abc@test.com",
                Id = 1,
                //GivenChallenges = new List<Challenge>(),
                //ReceivedChallenges = new List<Challenge>(),
                Workouts = new List<Workout>
                {
                    new Workout
                    {
                        Id = 0,
                        CreationDate = DateTime.Now,
                        Name = "first",
                        User = new User(),
                        UserId = 1,
                        WorkoutEntries = new List<WorkoutEntry>
                        {
                            new WorkoutEntry
                            {
                                Id = 0,
                                ExerciseId = 0,
                                Reps = 12,
                                Set = 1,
                                Weight = 50,
                                Exercise = new Exercise
                                {
                                    //Challenges = new List<Challenge>(),
                                    Description = "Compound",
                                    Name = "Bench",
                                    Id = 0
                                }
                            }
                        }
                    }
                }
            };

            userRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
            mapperMock.Setup(m => m.Map<PortableUserDto>(It.IsAny<User>())).Returns(new PortableUserDto());
            mapperMock.Setup(m => m.Map<PortableWorkoutDto>(It.IsAny<Workout>())).Returns(new PortableWorkoutDto());
            mapperMock.Setup(m => m.Map<PortableWorkoutEntryDto>(It.IsAny<WorkoutEntry>()))
                .Returns(new PortableWorkoutEntryDto());

            fileProviderMock.Setup(f => f.MakeFileAsync(It.IsAny<PortableUserDto>())).ReturnsAsync(new FileDto
            {
                FileContent = null
            });

            var SUT = new ExportService(loggerMock.Object, userRepoMock.Object, mapperMock.Object,
                fileProviderMock.Object, workoutRepoMock.Object, exerciseRepoMock.Object);
            var userid = 1;
            //Act

            Assert.Null(SUT.ExportToFileAsync(userid).Result);
        }


        [Fact(DisplayName = "ExportToFileAsync - file is null returns null")]
        public void EExportToFileNullReturnsNull()
        {
            //Arrage
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();

            var mapperMock = new Mock<IMapper>();
            var fileProviderMock = new Mock<IFIleProvider<PortableUserDto>>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            var user = new User
            {
                Name = "Dennis",
                Sex = Sex.Male,
                BodyWeight = 100,
                Height = 190,
                BirthDate = DateTime.Now,
                Country = Country.Denmark,
                CreationDate = DateTime.Now,
                DisplayName = "Leet",
                Email = "abc@test.com",
                Id = 1,
                //GivenChallenges = new List<Challenge>(),
                //ReceivedChallenges = new List<Challenge>(),
                Workouts = new List<Workout>
                {
                    new Workout
                    {
                        Id = 0,
                        CreationDate = DateTime.Now,
                        Name = "first",
                        User = new User(),
                        UserId = 1,
                        WorkoutEntries = new List<WorkoutEntry>
                        {
                            new WorkoutEntry
                            {
                                Id = 0,
                                ExerciseId = 0,
                                Reps = 12,
                                Set = 1,
                                Weight = 50,
                                Exercise = new Exercise
                                {
                                    //Challenges = new List<Challenge>(),
                                    Description = "Compound",
                                    Name = "Bench",
                                    Id = 0
                                }
                            }
                        }
                    }
                }
            };

            userRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
            mapperMock.Setup(m => m.Map<PortableUserDto>(It.IsAny<User>())).Returns(new PortableUserDto());
            mapperMock.Setup(m => m.Map<PortableWorkoutDto>(It.IsAny<Workout>())).Returns(new PortableWorkoutDto());
            mapperMock.Setup(m => m.Map<PortableWorkoutEntryDto>(It.IsAny<WorkoutEntry>()))
                .Returns(new PortableWorkoutEntryDto());
            fileProviderMock.Setup(f => f.MakeFileAsync(It.IsAny<PortableUserDto>())).ReturnsAsync((FileDto) null);

            var SUT = new ExportService(loggerMock.Object, userRepoMock.Object, mapperMock.Object,
                fileProviderMock.Object, workoutRepoMock.Object, exerciseRepoMock.Object);
            var userid = 1;
            //Act

            Assert.Null(SUT.ExportToFileAsync(userid).Result);
        }

        [Fact(DisplayName = "ExportToFileAsync - Valid user Id returns FileDto")]
        public void EExportToFileReturnsFileDto()
        {
            //Arrage
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();
            var fileProviderMock = new Mock<IFIleProvider<PortableUserDto>>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();

            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            var user = new User
            {
                Name = "Dennis",
                Sex = Sex.Male,
                BodyWeight = 100,
                Height = 190,
                BirthDate = DateTime.Now,
                Country = Country.Denmark,
                CreationDate = DateTime.Now,
                DisplayName = "Leet",
                Email = "abc@test.com",
                Id = 1,
                //GivenChallenges = new List<Challenge>(),
                //ReceivedChallenges = new List<Challenge>(),
                Workouts = new List<Workout>

                {
                    new Workout
                    {
                        Id = 1,
                        CreationDate = DateTime.Now,
                        Name = "first",
                        User = new User(),
                        UserId = 1,
                        WorkoutEntries = new List<WorkoutEntry>
                        {
                            new WorkoutEntry
                            {
                                Id = 1,
                                ExerciseId = 1,
                                Reps = 12,
                                Set = 1,
                                Weight = 50,
                                Exercise = new Exercise
                                {
                                    //Challenges = new List<Challenge>(),
                                    Description = "Compound",
                                    Name = "Bench",
                                    Id = 1
                                }
                            }
                        }
                    }
                }
            };

            userRepoMock.Setup(r => r.GetUser(It.IsAny<int>())).ReturnsAsync(user);
            workoutRepoMock.Setup(r => r.GetWorkoutWithEntries(It.IsAny<int>())).ReturnsAsync(new Workout
            {
                WorkoutEntries = new List<WorkoutEntry>()
            });
            mapperMock.Setup(m => m.Map<PortableUserDto>(It.IsAny<User>())).Returns(new PortableUserDto());
            mapperMock.Setup(m => m.Map<PortableWorkoutDto>(It.IsAny<Workout>())).Returns(new PortableWorkoutDto
            {
                Name = "",
                WorkoutEntryDtos = new List<PortableWorkoutEntryDto>()
            });
            mapperMock.Setup(m => m.Map<PortableWorkoutEntryDto>(It.IsAny<WorkoutEntry>()))
                .Returns(new PortableWorkoutEntryDto());

            fileProviderMock.Setup(f => f.MakeFileAsync(It.IsAny<PortableUserDto>())).ReturnsAsync(new FileDto
            {
                FileContent = new byte[] {0, 1, 0, 1}
            });

            var SUT = new ExportService(loggerMock.Object, userRepoMock.Object, mapperMock.Object,
                fileProviderMock.Object, workoutRepoMock.Object, exerciseRepoMock.Object);
            var userid = 1;
            //Act
            var result = SUT.ExportToFileAsync(userid).Result;

            //Assert
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "ExportToFileAsync - invalid user Id returns null")]
        public void EExportToFileReturnsNull()
        {
            //Arrage
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();
            var mapperMock = new Mock<IMapper>();
            var fileProviderMock = new Mock<IFIleProvider<PortableUserDto>>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            mapperMock.Setup(m => m.Map<PortableUserDto>(It.IsAny<User>())).Returns((PortableUserDto) null);

            userRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync((User) null);

            var SUT = new ExportService(loggerMock.Object, userRepoMock.Object, mapperMock.Object,
                fileProviderMock.Object, workoutRepoMock.Object, exerciseRepoMock.Object);
            var userid = -1;

            //Act
            var result = SUT.ExportToFileAsync(userid).Result;

            //Asser

            Assert.Null(result);
        }


        [Fact(DisplayName = "ExportToPortableObjectAsync - No existing user returns null")]
        public void ExportToPortableObjectReturnsNullNoUser()
        {
            //Arrage
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();

            var mapperMock = new Mock<IMapper>();
            var fileProviderMock = new Mock<IFIleProvider<PortableUserDto>>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);


            userRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync((User) null);

            var SUT = new ExportService(loggerMock.Object, userRepoMock.Object, mapperMock.Object,
                fileProviderMock.Object, workoutRepoMock.Object, exerciseRepoMock.Object);
            var userid = Guid.NewGuid();
            //Act

            var result = SUT.ExportToPortableObjectAsync(userid).Result;

            Assert.Null(result);
        }

        [Fact(DisplayName = "ExportToPortableObjectAsync - Valid user Id returns PortableUserDto")]
        public void ExportToPortableObjectReturnsPortableUserDto()
        {
            //Arrage
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var workoutRepoMock = new Mock<IWorkoutRepository>();
            var exerciseRepoMock = new Mock<IExerciseRepository>();

            var mapperMock = new Mock<IMapper>();
            var fileProviderMock = new Mock<IFIleProvider<PortableUserDto>>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            var user = new User
            {
                Name = "Dennis",
                Sex = Sex.Male,
                BodyWeight = 100,
                Height = 190,
                BirthDate = DateTime.Now,
                Country = Country.Denmark,
                CreationDate = DateTime.Now,
                DisplayName = "Leet",
                Email = "abc@test.com",
                Id = 1,
                //GivenChallenges = new List<Challenge>(),
                //ReceivedChallenges = new List<Challenge>(),
                Workouts = new List<Workout>
                {
                    new Workout
                    {
                        Id = 0,
                        CreationDate = DateTime.Now,
                        Name = "first",
                        User = new User(),
                        UserId = 1,
                        WorkoutEntries = new List<WorkoutEntry>
                        {
                            new WorkoutEntry
                            {
                                Id = 0,
                                ExerciseId = 0,
                                Reps = 12,
                                Set = 1,
                                Weight = 50,
                                Exercise = new Exercise
                                {
                                    //Challenges = new List<Challenge>(),
                                    Description = "Compound",
                                    Name = "Bench",
                                    Id = 0
                                }
                            }
                        }
                    }
                }
            };

            userRepoMock.Setup(r => r.GetUserWithToken(It.IsAny<Guid>())).ReturnsAsync(user);
            workoutRepoMock.Setup(r => r.GetWorkoutWithEntries(It.IsAny<int>())).ReturnsAsync(new Workout
            {
                WorkoutEntries = new List<WorkoutEntry>()
            });
            mapperMock.Setup(m => m.Map<PortableUserDto>(It.IsAny<User>())).Returns(new PortableUserDto());
            mapperMock.Setup(m => m.Map<PortableWorkoutDto>(It.IsAny<Workout>())).Returns(new PortableWorkoutDto
            {
                Name = "",
                WorkoutEntryDtos = new List<PortableWorkoutEntryDto>()
            });
            mapperMock.Setup(m => m.Map<PortableWorkoutEntryDto>(It.IsAny<WorkoutEntry>()))
                .Returns(new PortableWorkoutEntryDto());

            fileProviderMock.Setup(f => f.MakeFileAsync(It.IsAny<PortableUserDto>())).ReturnsAsync(new FileDto
            {
                FileContent = new byte[] {0, 1, 0, 1}
            });
            var SUT = new ExportService(loggerMock.Object, userRepoMock.Object, mapperMock.Object,
                fileProviderMock.Object, workoutRepoMock.Object, exerciseRepoMock.Object);
            var userid = Guid.NewGuid();
            //Act

            Assert.NotNull(SUT.ExportToPortableObjectAsync(userid).Result);
        }
    }
}