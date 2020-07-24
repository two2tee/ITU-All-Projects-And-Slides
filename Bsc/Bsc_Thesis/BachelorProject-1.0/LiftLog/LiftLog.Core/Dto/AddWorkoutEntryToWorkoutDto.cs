// AddWorkoutEntryToWorkoutDto.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Collections.Generic;

namespace LiftLog.Core.Dto
{
    /// <summary>
    ///     DTO used to add a workout entry to a workout dto
    /// </summary>
    public class AddWorkoutEntryToWorkoutDto
    {
        public int UserId { get; set; }

        public int WorkoutId { get; set; }

        public ICollection<NewWorkoutEntryDto> WorkoutEntryDtos { get; set; }
    }
}