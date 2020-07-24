using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LiftLog.Core.Dto.PortableDto;

namespace LiftLog.Web.ViewModels.WorkoutViewModels
{

    /// <summary>
    /// This view model exposes a new challenge where the properties are read only. 
    /// </summary>
    public class NewChallengeViewModel
    {
        public int Id { get; set; }

        public int ChallengeeId { get; set; }

        public int ExerciseId { get; set; }

        public string ExerciseName { get; set; }

        public int Set { get; set; }

        public int Weight { get; set; }

        public int Reps { get; set; }

        public IEnumerable<ChallengeAbleUserDto> ChallengeAbleUserDtos { get; set; }
    }
}
