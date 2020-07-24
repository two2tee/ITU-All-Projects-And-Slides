// Exercise.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Collections.Generic;
using LiftLog.Data.Interfaces;

namespace LiftLog.Data.Entities
{
    /// <summary>
    ///     Represents an exercise. It contains a name and description.
    ///     An exercise can be part of some workouts made by users or
    ///     programs
    /// </summary>
    public class Exercise : IEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<WorkoutEntry> WorkoutEntries { get; set; }

        public virtual ICollection<Challenge> Challenges { get; set; }
        public int Id { get; set; }
    }
}