// RankingRepository.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;

namespace LiftLog.Data.Repositories
{
    public class RankingRepository : GenericRepository<Ranking>, IRankingRepository
    {
        public RankingRepository(IContext context) : base(context)
        {
        }
    }
}