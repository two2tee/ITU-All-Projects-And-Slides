// UserRepository.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Linq;
using System.Threading.Tasks;
using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Data.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(IContext context) : base(context)
        {
        }

        /// <summary>
        ///     Retrieves a specific user with userId
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <returns>User entity</returns>
        public async Task<User> GetUser(int userId)
        {
            var users =
                (await GetAllAsync()).Include("Workouts")
                .Include("GivenChallenges")
                .Include("ReceivedChallenges")
                .Include("Claims");
            return users.FirstOrDefault(u => u.Id == userId);
        }

        /// <summary>
        ///     Retrieves a specific user with user token
        /// </summary>
        /// <param name="token">token of user</param>
        /// <returns>User entity</returns>
        public async Task<User> GetUserWithToken(Guid token)
        {
            var users =
                (await GetAllAsync()).Include("Workouts")
                .Include("GivenChallenges")
                .Include("ReceivedChallenges")
                .Include("Claims");
            return users.FirstOrDefault(u => u.ExportToken == token);
        }
    }
}