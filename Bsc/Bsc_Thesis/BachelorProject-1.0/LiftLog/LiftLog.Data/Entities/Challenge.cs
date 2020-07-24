// Challenge.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using LiftLog.Data.Interfaces;

namespace LiftLog.Data.Entities
{
    /// <summary>
    ///     This class represents a challenge
    /// </summary>
    public class Challenge : IEntity
    {
        public int? ChallengerId { get; set; }

        public User ChallengerUser { get; set; }

        public int ChallengeeId { get; set; }

        public User ChallengeeUser { get; set; }

        public int ExerciseId { get; set; }

        public Exercise Exercise { get; set; }

        public DateTime CreationDate { get; set; }

        public int Reps { get; set; }

        public int Weight { get; set; }


        public bool IsComplete { get; set; }
        public int ResultReps { get; set; }
        public int Id { get; set; }
    }
}