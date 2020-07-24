// PortableWorkoutDto.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Collections.Generic;

namespace LiftLog.Core.Dto.PortableDto
{
    /// <summary>
    /// Portable dto of a workout
    /// </summary>
    public class PortableWorkoutDto
    {
        public string Name { get; set; }

        public DateTime CreationDate { get; set; }

        public ICollection<PortableWorkoutEntryDto> WorkoutEntryDtos { get; set; }
    }
}