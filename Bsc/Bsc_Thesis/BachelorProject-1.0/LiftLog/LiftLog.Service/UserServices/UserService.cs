// UserService.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Threading.Tasks;
using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using LiftLog.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace LiftLog.Service.UserServices
{
    /// <summary>
    ///     This class is responsible for handling every user related actions, such as CRUD of users
    /// </summary>
    public class UserService : BaseService, IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserService(ILoggerFactory logger, IMapper mapper, IUserRepository repository)
        {
            Logger = logger.CreateLogger<UserService>();
            _userRepository = repository;
            _mapper = mapper;
        }


        /// <summary>
        ///     Stores a new user in the system
        ///     If something happened and user wasn't created  successfully the integer
        ///     -1 will be returned
        /// </summary>
        /// <param name="userDto">User DTO</param>
        /// <returns>id of new user</returns>
        public async Task<int> SaveNewUserAsync(NewUserDto userDto)
        {
            try
            {
                if (userDto == null || userDto.UserId < 0)
                {
                    LogError("SaveNewUserAsync", "Dto was invalid. Aborted user creation");
                    return -1;
                }

                   //Getting existing user
                var user = await _userRepository.FindAsync(userDto.UserId);
            
                if (user == null)
                {
                    LogNonExistingUserError("SaveNewUserAsync", userDto.UserId);
                    return -1;
                }

             
                //Adding extra information
                user.ExportToken = Guid.NewGuid();
                user.BirthDate = new DateTime();

                var isSuccess = await _userRepository.UpdateAsync(user);

                if (isSuccess)
                {
                    LogCritical("SaveNewUserAsync", $"Created user with id {userDto.UserId}");
                    return user.Id;
                }
                LogError("SaveNewUserAsync", $"Failed to create user with id {userDto.UserId}");
                return -1;
            }
            catch (Exception e)
            {
                LogError("SaveNewUserAsync", $"Exception was trown - Failed to create user with id {userDto.UserId}", e);
                return -1;
            }
        }

        /// <summary>
        ///     Deletes a given user with id
        /// </summary>
        /// <param name="id">id of user</param>
        /// <returns>bool if deletion is successfull</returns>
        public async Task<bool> DeleteUserAsync(int id)
        {
            if (id < 0)
            {
                LogInvalidIdError("DeleteUserAsync", id);
                return false;
            }

            var isSuccess = await _userRepository.DeleteAsync(id);
            if (isSuccess)
                LogCritical("DeleteUserAsync", $"Deleted all information of a user with id {id}");
            else
                LogError("DeleteUserAsync", $"Failed to delete user with id {id}");
            return isSuccess;
        }

        /// <summary>
        ///     Modifies a user. If a property in the DTO isn't empty it will be considered to be used as overwrite
        ///     data
        /// </summary>
        /// <param name="modifiedUserDto">Data that has been modified for a user</param>
        /// <returns>if update success </returns>
        public async Task<bool> ModifyUserAsync(ModifiedUserDto modifiedUserDto)
        {
            if (modifiedUserDto.Id < 1)
            {
                LogInvalidIdError("ModifyUserAsync", modifiedUserDto.Id);
                return false;
            }
            var user = await _userRepository.FindAsync(modifiedUserDto.Id);

            if (user == null)
            {
                LogNonExistingUserError("ModifyUserAsync", modifiedUserDto.Id);
                return false;
            }

            if (modifiedUserDto.Name != null) user.Name = modifiedUserDto.Name;
            if (modifiedUserDto.DisplayName != null) user.DisplayName = modifiedUserDto.DisplayName;
            if (modifiedUserDto.Country != user.Country) user.Country = modifiedUserDto.Country;
            if (IsAgeChanged(modifiedUserDto.BirthDate)) user.BirthDate = modifiedUserDto.BirthDate;
            if (modifiedUserDto.BodyWeight != user.BodyWeight) user.BodyWeight = modifiedUserDto.BodyWeight;
            if (modifiedUserDto.Height != user.Height) user.Height = modifiedUserDto.Height;
            if (modifiedUserDto.Sex != user.Sex) user.Sex = modifiedUserDto.Sex;

            var isSuccess = await _userRepository.UpdateAsync(user);


            if (isSuccess)
                LogCritical("ModifyUserAsync", $"information of user with id {modifiedUserDto.Id} has been modified");
            else
                LogError("ModifyUserAsync", $"Failed to modify user with id {modifiedUserDto.Id}");
            return isSuccess;
        }

        /// <summary>
        ///     Get a user dto based on its id
        /// </summary>
        /// <param name="id">id of user</param>
        /// <returns>User DTO or null if not exist</returns>
        public async Task<UserDto> GetUserAsync(int id)
        {
            if (id < 1)
            {
                LogInvalidIdError("GetUserAsync", id);
                return null;
            }
            var user = await _userRepository.FindAsync(id);

            if (user == null)
            {
                LogNonExistingUserError("GetUserAsync", id);
                return null;
            }

            LogCritical("GetUserAsync", $"Retrived a user with id {id}");
            return _mapper.Map<UserDto>(user);
        }

        private bool IsAgeChanged(DateTime modifiedAge)
        {
            return modifiedAge != new DateTime();
        }

  
    }
}