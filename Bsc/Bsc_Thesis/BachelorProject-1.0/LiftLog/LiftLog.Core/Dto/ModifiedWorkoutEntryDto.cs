// ModifiedWorkoutEntryDto.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

namespace LiftLog.Core.Dto
{
    public class ModifiedWorkoutEntryDto
    {
        public int UserId { get; set; }

        public int WorkoutEntryId { get; set; }

        public int Weight { get; set; }
        public int Reps { get; set; }
        public int Set { get; set; }
    }
}