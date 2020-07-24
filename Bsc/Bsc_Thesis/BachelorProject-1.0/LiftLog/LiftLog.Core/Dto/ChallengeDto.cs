// ChallengeDto.cs is part of LiftLog and was created on 04/14/2017. 
// Last modified on 04/15/2017.

using System;

namespace LiftLog.Core.Dto
{
    /// <summary>
    ///     Dto for a challenge
    /// </summary>
    public class ChallengeDto
    {
        public int Id { get; set; }

        public bool IsShareAble { get; set; }

        public string ChallengerDisplayName { get; set; }
        public int ChallengerId { get; set; }

        public string ChallengeeDisplayName { get; set; }
        public int ChallengeeId { get; set; }

        public DateTime CreationDate { get; set; }

        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }

        public int Weight { get; set; }
        public int Reps { get; set; }

        public int ResultReps { get; set; }

        public bool IsComplete { get; set; }
    }
}