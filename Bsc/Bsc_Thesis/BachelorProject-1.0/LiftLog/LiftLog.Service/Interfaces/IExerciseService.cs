// IExerciseService.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Collections.Generic;
using System.Threading.Tasks;
using LiftLog.Core.Dto;

namespace LiftLog.Service.Interfaces
{
    /// <summary>
    ///     Interface for an exercise service
    ///     It is assumed that the system has already predefined the exercises
    /// </summary>
    public interface IExerciseService
    {
        /// <summary>
        ///     Get all exercises stored in database with their ides
        ///     If no exercise were found return an empty list
        /// </summary>
        /// <returns>Collection of exercises</returns>
        Task<ICollection<ExerciseDto>> GetAllExercisesAsync();
    }
}