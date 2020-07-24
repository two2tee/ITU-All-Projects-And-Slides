// ChallengeViewModel.cs is part of LiftLog and was created on 04/14/2017. 
// Last modified on 04/15/2017.

using System.Collections.Generic;
using LiftLog.Core.Dto;

namespace LiftLog.Web.ViewModels.ChallengeViewModels
{
    /// <summary>
    ///     Viewmodel of received and given challenges for a user
    /// </summary>
    public class ChallengeViewModel
    {
        public IEnumerable<ChallengeDto> ReceivedChallenges { get; set; }
        public IEnumerable<ChallengeDto> GivenChallenges { get; set; }
    }
}