// NewChallengeDto.cs is part of LiftLog and was created on 04/14/2017. 
// Last modified on 04/15/2017.

namespace LiftLog.Core.Dto
{
    public class NewChallengeDto
    {
        public int ChallengerId { get; set; }

        public int ChallengeeId { get; set; }

        public int ExerciseId { get; set; }

        public int Weight { get; set; }

        public int Reps { get; set; }
    }
}