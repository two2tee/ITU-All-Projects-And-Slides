// ChallengeRepository.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Threading.Tasks;
using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Data.Repositories
{
    public class ChallengeRepository : GenericRepository<Challenge>, IChallengeRepository
    {
        public ChallengeRepository(IContext context) : base(context)
        {
        }

        public async Task<Challenge> GetChallenge(int id)
        {
            var challenges = (await GetAllAsync())
                .Include("ChallengeeUser")
                .Include("ChallengerUser")
                .Include("Exercise");

            return await challenges.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}