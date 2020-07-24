// ShareChallengeViewModel.cs is part of LiftLog and was created on 04/14/2017. 
// Last modified on 04/15/2017.

using System;

namespace LiftLog.Web.ViewModels.ChallengeViewModels
{
    /// <summary>
    ///     Represents viewmodel for sharing a challenge
    /// </summary>
    public class ShareChallengeViewModel
    {
        public DateTime CreationDate { get; set; }

        public string ExerciseName { get; set; }

        public int Weight { get; set; }

        public int Reps { get; set; }

        public int ResultReps { get; set; }

        public string Message { get; set; }
    }
}