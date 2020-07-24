// IUserService.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Threading.Tasks;
using LiftLog.Core.Dto;

namespace LiftLog.Service.Interfaces
{
    /// <summary>
    ///     A user service handles all related operations concerning users
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        ///     Creates a new user - If unsuccessfull the integer -1 will be returned
        /// </summary>
        /// <param name="userDto">dto with user information</param>
        /// <returns>UID of new user || -1 if failure</returns>
        Task<int> SaveNewUserAsync(NewUserDto userDto);

        /// <summary>
        ///     Deletes an existing user
        /// </summary>
        /// <param name="id"></param>
        /// <returns>if deletion was successfull</returns>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        ///     Modify an existing user with new/change information
        /// </summary>
        /// <param name="modifiedUserDto">dto with information to change</param>
        /// <returns>if update was successfull</returns>
        Task<bool> ModifyUserAsync(ModifiedUserDto modifiedUserDto);

        /// <summary>
        ///     Gets a specific user with its UID
        /// </summary>
        /// <param name="id">UID of user</param>
        /// <returns>user dto</returns>
        Task<UserDto> GetUserAsync(int id);
    }
}