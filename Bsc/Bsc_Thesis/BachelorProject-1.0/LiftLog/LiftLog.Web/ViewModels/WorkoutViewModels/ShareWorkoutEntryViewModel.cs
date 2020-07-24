// ShareWorkoutEntryViewModel.cs is part of LiftLog and was created on 04/14/2017. 
// Last modified on 04/15/2017.

namespace LiftLog.Web.ViewModels.WorkoutViewModels
{
    /// <summary>
    ///     Represents viewmodel when sharing a given workout entry.
    /// </summary>
    public class ShareWorkoutEntryViewModel
    {
        public int Id { get; set; }

        public int WorkoutId { get; set; }

        public string ExerciseName { get; set; }

        public int Set { get; set; }

        public int Weight { get; set; }

        public int Reps { get; set; }

        public string Message { get; set; }
    }
}