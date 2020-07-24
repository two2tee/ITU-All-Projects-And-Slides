// ChallengeResultDto.cs is part of LiftLog and was created on 04/14/2017. 
// Last modified on 04/15/2017.

namespace LiftLog.Core.Dto
{
    /// <summary>
    ///     Dto with challenge resuts for a given challenge
    /// </summary>
    public class ChallengeResultDto
    {
        public int UserId { get; set; }
        public int ChallengeId { get; set; }
        public int ResultReps { get; set; }
        public bool IsComplete { get; set; }
    }
}