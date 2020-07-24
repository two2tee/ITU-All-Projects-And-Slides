// ChallengeResultViewModel.cs is part of LiftLog and was created on 04/14/2017. 
// Last modified on 04/15/2017.

using System;
using System.ComponentModel.DataAnnotations;

namespace LiftLog.Web.ViewModels.ChallengeViewModels
{
    /// <summary>
    ///     Represents viewmodel of challenge results
    /// </summary>
    public class ChallengeResultViewModel
    {
        public int Id { get; set; }

        public string ChallengerDisplayName { get; set; }

        public int ChallengeeId { get; set; }

        public DateTime CreationDate { get; set; }

        public string ExerciseName { get; set; }
        public int Weight { get; set; }
        public int Reps { get; set; }

        [Range(0, 999, ErrorMessage = "Please specify a valid amount of reps between 0 and 999.")]
        public int ResultReps { get; set; }

        public bool IsComplete { get; set; }
    }
}