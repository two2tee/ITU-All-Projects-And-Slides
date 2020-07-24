// IRankingService.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Collections.Generic;
using LiftLog.Core.Dto;

namespace LiftLog.Service.Interfaces
{
    /// <summary>
    /// Ranking Service that returns different lists of ranks based on 
    /// given categories such as country.
    /// </summary>
    [System.Obsolete("Optional feature for the future")]
    public interface IRankingService
    {
        ICollection<RankDto> GetRanks();
    }
}