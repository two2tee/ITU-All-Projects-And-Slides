// PdfProviderTest.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Collections.Generic;
using LiftLog.Core.Dto.PortableDto;
using LiftLog.Service.FileProviders;
using Microsoft.AspNetCore.NodeServices;
using Moq;
using Xunit;

namespace LiftLog.Service.Tests
{
    /// <summary>
    ///     Test suite for the PDF-FileProvider.. It tests if a 
    ///     portable object is converted to a pdf within a file DTO
    /// </summary>
    public class PdfProviderTest
    {
        [Fact(DisplayName = "MakeFileAsync - Returns not null but returns a pdf fileDto")]
        public void GetContentTest()
        {
            //Arrange
            var ns = new Mock<INodeServices>();

            var portableUser = new PortableUserDto
            {
                Name = "Dennis",
                BirthDay = DateTime.Now,
                CountryName = "Denmark",
                SexType = "Sex",
                BodyWeight = 65,
                Email = "abc@cde.com",
                RetrievalDate = DateTime.Now,
                Height = 178,
                ChallengeGiven = 1,
                ChallengeReceived = 2,
                CreationDate = DateTime.Now,
                DisplayName = "LeetUser",
                Workouts = new List<PortableWorkoutDto>
                {
                    new PortableWorkoutDto
                    {
                        CreationDate = DateTime.Now,
                        Name = "First ever",
                        WorkoutEntryDtos = new List<PortableWorkoutEntryDto>
                        {
                            new PortableWorkoutEntryDto
                            {
                                ExerciseName = "Benchpress",
                                Reps = 12,
                                Set = 2,
                                Weight = 145
                            }
                        }
                    },
                    new PortableWorkoutDto
                    {
                        CreationDate = DateTime.Now,
                        Name = "Second",
                        WorkoutEntryDtos = new List<PortableWorkoutEntryDto>
                        {
                            new PortableWorkoutEntryDto
                            {
                                ExerciseName = "Squad",
                                Reps = 12,
                                Set = 1,
                                Weight = 200
                            }
                        }
                    }
                }
            };

            ns.Setup(s => s.InvokeAsync<byte[]>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new byte[] {0, 1, 0, 1, 1, 0});

            var SUT = new PdfProvider(ns.Object);
            //Act
            var result = SUT.MakeFileAsync(portableUser).Result;

            //Assert
            Assert.NotNull(result);
        }
    }
}