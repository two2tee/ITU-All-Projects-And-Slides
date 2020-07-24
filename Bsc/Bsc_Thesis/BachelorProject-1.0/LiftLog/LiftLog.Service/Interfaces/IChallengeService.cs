// IChallengeService.cs is part of LiftLog and was created on 04/14/2017. 
// Last modified on 04/15/2017.

using System.Collections.Generic;
using System.Threading.Tasks;
using LiftLog.Core.Dto;
using LiftLog.Core.Dto.PortableDto;

namespace LiftLog.Service.Interfaces
{
    /// <summary>
    ///     Challenge service is responsible for challenge requests
    /// </summary>
    public interface IChallengeService
    {
        /// <summary>
        ///     Sets the results for a given challenge
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>if success</returns>
        Task<bool> SetResultAsync(ChallengeResultDto dto);

        /// <summary>
        ///     Returns a specific Challenge
        /// </summary>
        /// <param name="challengeId">uid of challenge</param>
        /// <returns>Challenge dto</returns>
        Task<ChallengeDto> GetChallengeAsync(int challengeId);

        /// <summary>
        ///     Returns a list of received challenges for a user
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <returns>Received challenges</returns>
        Task<ICollection<ChallengeDto>> GetReceivedChallengesAsync(int userId);

        /// <summary>
        ///     Returns a list of given challenges for a user
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <returns>Given Challenges</returns>
        Task<ICollection<ChallengeDto>> GetGivenChallengesAsync(int userId);

        /// <summary>
        ///     Returns all challengeable users
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <returns>List of users to be challenged</returns>
        Task<ICollection<ChallengeAbleUserDto>> GetChallengeAbleUsersAsync(int userId);

        /// <summary>
        ///     Deletes a challenge
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <param name="challengeId">challengeId of challenge</param>
        /// <returns>if success</returns>
        Task<bool> DeleteChallengeAsync(int userId, int challengeId);

        /// <summary>
        ///     Delets all challenges for a given user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> DeleteAllChallengesAsync(int userId);

        /// <summary>
        ///     Creates a new challenge between two users
        /// </summary>
        /// <param name="dto">dto with challenge details</param>
        /// <returns>created challenge Id</returns>
        Task<int> CreateChallengeAsync(NewChallengeDto dto);
    }
}