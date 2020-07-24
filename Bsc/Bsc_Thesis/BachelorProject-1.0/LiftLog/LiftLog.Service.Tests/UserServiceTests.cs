// UserServiceTests.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Core.Enums;
using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using LiftLog.Service.UserServices;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LiftLog.Service.Tests
{
    /// <summary>
    ///     Test suite for user service. It tests the CRUD operations within the service
    ///     as well as performing negative tests.
    /// </summary>
    public class UserServiceTests
    {
        [Fact(DisplayName = "Delete User with Id User Database called once, true returned")]
        public void Delete_Existing_User_true_returned_databaseOnce()
        {
            //Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var mockMapper = new Mock<IMapper>();
            var loggerMock = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);


            userRepoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var SUT = new UserService(loggerMock.Object, mockMapper.Object, userRepoMock.Object);

            //Act
            var result = SUT.DeleteUserAsync(1).Result;

            //Asert
            Assert.True(result);
            userRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Once());
        }

        [Fact(DisplayName = "Do not Delete None-Existing User with Id - User Database called once, false returned")]
        public void Delete_Non_Existing_User_false_returned_databaseOnce()
        {
            //Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var mockMapper = new Mock<IMapper>();
            var loggerMock = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);


            userRepoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(false);

            var SUT = new UserService(loggerMock.Object, mockMapper.Object, userRepoMock.Object);

            //Act
            var result = SUT.DeleteUserAsync(1).Result;

            //Asert
            Assert.False(result);
            userRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Once());
        }

        [Fact(DisplayName = "Get Existing User with Id _ dto is returned")]
        public void GetExistingUser_ReturnUserDTO()
        {
            //Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILoggerFactory>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
            var user = new User
            {
                Id = 1,
                Name = "Dennis",
                DisplayName = "Beast",
                BirthDate = DateTime.Today,
                Height = 172,
                BodyWeight = 65,
                Country = Country.Denmark,
                Email = "abc@live.com",
                Sex = Sex.Male,
                EmailConfirmed = true,
                CreationDate = DateTime.Now
            };

            var dto = new UserDto
            {
                Name = "Dennis",
                DisplayName = "Beast",
                BirthDay = DateTime.Today,
                Height = 172,
                BodyWeight = 65,
                Country = Country.Denmark,
                Email = "abc@live.com",
                Sex = Sex.Male
            };

            mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(dto);
            userRepoMock.Setup(r => r.FindAsync(1)).ReturnsAsync(user);

            var SUT = new UserService(loggerMock.Object, mockMapper.Object, userRepoMock.Object);

            //Act
            var result = SUT.GetUserAsync(1).Result;

            //Asert
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Get Existing User with Id User Database called once")]
        public void GetExistingUser_ReturnUserDTO_Database_called_once()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
            var user = new User
            {
                Id = 1,
                Name = "Dennis",
                DisplayName = "Beast",
                BirthDate = DateTime.Today,
                Height = 172,
                BodyWeight = 65,
                Country = Country.Denmark,
                Email = "abc@live.com",
                Sex = Sex.Male,
                EmailConfirmed = true,
                CreationDate = DateTime.Now
            };

            var dto = new UserDto
            {
                Name = "Dennis",
                DisplayName = "Beast",
                BirthDay = DateTime.Today,
                Height = 172,
                BodyWeight = 65,
                Country = Country.Denmark,
                Email = "abc@live.com",
                Sex = Sex.Male
            };

            mockMapper.Setup(m => m.Map<User, UserDto>(It.IsAny<User>())).Returns(dto);
            userRepoMock.Setup(r => r.FindAsync(1)).ReturnsAsync(user);

            var SUT = new UserService(loggerMock.Object, mockMapper.Object, userRepoMock.Object);

            //Act
            var result = SUT.GetUserAsync(1).Result;

            //Asert
            userRepoMock.Verify(r => r.FindAsync(It.IsAny<int>()), Times.Once());
        }


        [Fact(DisplayName = "Get NonExisting User with Id User Database called once, null returned")]
        public void GetNonExistingUser_ReturnUserDTO_Database_called_once_null_Returned()
        {
            //Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILoggerFactory>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
            var user = new User
            {
                Id = 1,
                Name = "Dennis",
                DisplayName = "Beast",
                BirthDate = DateTime.Today,
                Height = 172,
                BodyWeight = 65,
                Country = Country.Denmark,
                Email = "abc@live.com",
                Sex = Sex.Male,
                EmailConfirmed = true,
                CreationDate = DateTime.Now
            };

            var dto = new UserDto
            {
                Name = "Dennis",
                DisplayName = "Beast",
                BirthDay = DateTime.Today,
                Height = 172,
                BodyWeight = 65,
                Country = Country.Denmark,
                Email = "abc@live.com",
                Sex = Sex.Male
            };

            mockMapper.Setup(m => m.Map<User, UserDto>(It.IsAny<User>())).Returns(dto);
            userRepoMock.Setup(r => r.FindAsync(1)).ReturnsAsync((User) null);

            var SUT = new UserService(loggerMock.Object, mockMapper.Object, userRepoMock.Object);

            //Act
            var result = SUT.GetUserAsync(1).Result;

            //Asert
            Assert.Null(result);
            userRepoMock.Verify(r => r.FindAsync(It.IsAny<int>()), Times.Once());
        }


        [Fact(DisplayName = "Save new user with valid DTO - Databases Called once")]
        public void SaveNewUserValidNewCountryDatabasesCalledOnce()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
            var dto = new NewUserDto
            {
                Name = "Dennis",
                DisplayName = "Beast",
                BirthDate = DateTime.Today,
                Height = 172,
                BodyWeight = 65,
                Country = Country.Denmark,
                Sex = Sex.Male
            };

            var user = new User
            {
                Id = 0
            };

            mockMapper.Setup(m => m.Map<NewUserDto, User>(It.IsAny<NewUserDto>())).Returns(new User());
            userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);
            userRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(user);


            var SUT = new UserService(loggerMock.Object, mockMapper.Object, userRepoMock.Object);

            //Act
            var result = SUT.SaveNewUserAsync(dto).Result;

            //Asert
            userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once());
            userRepoMock.Verify(r => r.FindAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Save new user with valid DTO - Return int ID")]
        public void SaveNewUserValidNewCountryReturnInt()
        {
            //Arrange
            var loggerMock = new Mock<ILoggerFactory>();
            var userRepoMock = new Mock<IUserRepository>();
            var mockMapper = new Mock<IMapper>();
            var logger = new Mock<ILogger>();
            loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
            var dto = new NewUserDto
            {
                UserId = 0,
                Name = "Dennis",
                DisplayName = "Beast",
                BirthDate = DateTime.Today,
                Height = 172,
                BodyWeight = 65,
                Country = Country.Denmark,
                Sex = Sex.Male
            };

            var user = new User
            {
                Id = 0
            };

            mockMapper.Setup(m => m.Map<NewUserDto, User>(It.IsAny<NewUserDto>())).Returns(new User());
            userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);
            userRepoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(user);

            var SUT = new UserService(loggerMock.Object, mockMapper.Object, userRepoMock.Object);

            //Act
            var result = SUT.SaveNewUserAsync(dto).Result;

            //Asert
            Assert.True(result == 0);
        }
    }
}