// WorkoutEntryViewModel.cs is part of LiftLog and was created on 04/10/2017. 
// Last modified on 04/15/2017.

namespace LiftLog.Web.ViewModels.WorkoutViewModels
{
    /// <summary>
    ///     Represents exercises as part of a workout in the workout page.
    ///     An exercise has a name and user may log how many sets, weight and reps have been performed per exercise.
    ///     The model is used by the WorkoutController.
    /// </summary>
    public class WorkoutEntryViewModel
    {
        public int Id { get; set; }

        public string ExerciseName { get; set; }

        public int Set { get; set; }

        public int Weight { get; set; }

        public int Reps { get; set; }
    }
}