// PortableWorkoutEntryDto.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

namespace LiftLog.Core.Dto.PortableDto
{
    /// <summary>
    /// Portable dto of a workout entry
    /// </summary>
    public class PortableWorkoutEntryDto
    {
        public string ExerciseName { get; set; }

        public long Set { get; set; }

        public long Weight { get; set; }

        public long Reps { get; set; }
    }
}