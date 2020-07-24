// IUserRepository.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Threading.Tasks;
using LiftLog.Data.Entities;

namespace LiftLog.Data.Interfaces
{
    /// <summary>
    ///     User repository with CRUD operations
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Retrieves a specific user with userId
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <returns>User entity</returns>
        Task<User> GetUser(int userId);

        /// <summary>
        /// Retrieves a specific user with user token
        /// </summary>
        /// <param name="token">token of user</param>
        /// <returns>User entity</returns>
        Task<User> GetUserWithToken(Guid token);
    }
}