// WorkoutDto.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Collections.Generic;

namespace LiftLog.Core.Dto
{
    public class WorkoutDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public DateTime CreationDate { get; set; }

        public ICollection<WorkoutEntryDto> WorkoutEntryDtos { get; set; }
    }
}