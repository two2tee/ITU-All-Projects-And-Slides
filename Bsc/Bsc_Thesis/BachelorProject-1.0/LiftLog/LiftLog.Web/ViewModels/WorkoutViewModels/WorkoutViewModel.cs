// WorkoutViewModel.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LiftLog.Core.Dto;

namespace LiftLog.Web.ViewModels.WorkoutViewModels
{
    /// <summary>
    ///     Represents data used to show workouts in workouts page.
    ///     A workout has a name, creation data and a list of entries (exercises).
    ///     The model is used by the WorkoutController.
    /// </summary>
    public class WorkoutViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CreationDate { get; set; }

        public IEnumerable<WorkoutEntryViewModel> WorkoutEntries { get; set; }

        // New Exercise information 

        public List<ExerciseDto> Exercises { get; set; }

        public int ExerciseId { get; set; }

        [Range(0, 100, ErrorMessage = "Please specify a valid amount of sets between 0 and 100.")]
        public int Set { get; set; }

        [Range(0, 1000, ErrorMessage = "Please specify a valid weight between 0 and 1000 kg.")]
        public int Weight { get; set; }

        [Range(0, 1000, ErrorMessage = "Please specify a valid amount of repititions between 0 and 100.")]
        public int Reps { get; set; }
    }
}