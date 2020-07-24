// Workout.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Collections.Generic;
using LiftLog.Data.Interfaces;

namespace LiftLog.Data.Entities
{
    /// <summary>
    ///     A workout is a session created by a single user. It contains what exercise
    ///     has been performed, the current set, the number of reps and the weight used during the
    ///     exercise.
    ///     A workout can also be considered as a log entry for a single user.
    /// </summary>
    public class Workout : IEntity
    {
        public Workout()
        {
            WorkoutEntries = new List<WorkoutEntry>();
        }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public string Name { get; set; }

        public DateTime CreationDate { get; set; }

        public virtual ICollection<WorkoutEntry> WorkoutEntries { get; set; }

        public int Id { get; set; }
    }
}