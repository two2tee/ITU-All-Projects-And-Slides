// WorkoutEntry.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using LiftLog.Data.Interfaces;

namespace LiftLog.Data.Entities
{
    /// <summary>
    ///     A workout entry
    /// </summary>
    public class WorkoutEntry : IEntity
    {
        public int WorkoutId { get; set; }

        public Workout Workout { get; set; }

        public int ExerciseId { get; set; }

        public virtual Exercise Exercise { get; set; }

        public long Set { get; set; }

        public long Weight { get; set; }

        public long Reps { get; set; }
        public int Id { get; set; }
    }
}