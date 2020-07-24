// Ranking.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using LiftLog.Data.Interfaces;

namespace LiftLog.Data.Entities
{
    /// <summary>
    ///     A current ranking for a user based on an exercise
    ///     TODO Ranking was an optional feature that was used to compare the best users for a
    ///     TODO a particular exercise
    /// </summary>
    [System.Obsolete("Yet to be implemented")]
    public class Ranking : IEntity
    {
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public int ExerciseId { get; set; }

        public virtual Exercise Exercise { get; set; }

        public DateTime Year { get; set; }

        public long MaxLift { get; set; }
        public int Id { get; set; }
    }
}