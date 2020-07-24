// IChallengeRepository.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Threading.Tasks;
using LiftLog.Data.Entities;

namespace LiftLog.Data.Interfaces
{
    /// <summary>
    ///     challenge repository with CRUD operations
    /// </summary>
    public interface IChallengeRepository : IRepository<Challenge>
    {
        Task<Challenge> GetChallenge(int id);
    }
}