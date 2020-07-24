// ExerciseServiceTest.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Collections.Generic;
using System.Linq;
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
    ///     Test suite for exercise service.
    /// </summary>
    public class ExerciseServiceTest
    {
        [Fact(DisplayName = "GetAllExercisesAsync returns empty list if none found")]
        public async Task GetAllExercisesReturnsEmpty()
        {
            //Arrage
            var repositoryMock = new Mock<IExerciseRepository>();
            var loggerMock = new Mock<ILoggerFactory>();
            var mapperMock = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);


            repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync((IQueryable<Exercise>) null);

            var SUT = new ExerciseService(loggerMock.Object, mapperMock.Object, repositoryMock.Object);

            //Act
            var result = await SUT.GetAllExercisesAsync();


            //Assert
            Assert.True(result.Count == 0);
        }

        [Fact(DisplayName = "GetAllExercisesAsync returns list of exerciseDtos")]
        public async Task GetAllExercisesReturnsExerciseDtos()
        {
            //Arrage
            var repositoryMock = new Mock<IExerciseRepository>();
            var loggerMock = new Mock<ILoggerFactory>();
            var mapperMock = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            var foundExercises = new List<Exercise>
            {
                new Exercise
                {
                    Id = 0,
                    Name = "Bench",
                    Description = "Compound",
                    WorkoutEntries = new List<WorkoutEntry>()
                },
                new Exercise
                {
                    Id = 1,
                    Name = "Squad",
                    Description = "Compound",
                    WorkoutEntries = new List<WorkoutEntry>()
                }
            };

            var dto = new ExerciseDto
            {
                Id = 0,
                Name = "Bench",
                Description = "Compound"
            };


            mapperMock.Setup(m => m.Map<ExerciseDto>(It.IsAny<Exercise>())).Returns(dto);
            repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(foundExercises.AsQueryable);

            var SUT = new ExerciseService(loggerMock.Object, mapperMock.Object, repositoryMock.Object);

            //Act
            var result = await SUT.GetAllExercisesAsync();


            //Assert
            Assert.True(result.Count == 2);
        }
    }
}