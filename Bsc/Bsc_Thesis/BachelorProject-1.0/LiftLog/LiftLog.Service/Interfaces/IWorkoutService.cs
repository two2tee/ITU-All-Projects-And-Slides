// IWorkoutService.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiftLog.Core.Dto;

namespace LiftLog.Service.Interfaces
{
    public interface IWorkoutService
    {
        /// <summary>
        ///     Creates a new workout
        /// </summary>
        /// <param name="dto">The DTO with workout information</param>
        /// <returns>UID of created workout || -1 if failure </returns>
        Task<int> AddWorkoutAsync(NewWorkoutDto dto);

        /// <summary>
        ///     Deletes existing workout
        /// </summary>
        /// <param name="userId">used for verification</param>
        /// <param name="workoutId"></param>
        /// <returns>if deletion was successfull</returns>
        Task<bool> DeleteWorkoutAsync(int userId, int workoutId);

        /// <summary>
        ///     Deletes all workouts for a user
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <returns>if success</returns>
        Task<bool> DeleteAllWorkoutAsync(int userId);

        /// <summary>
        ///     Get all workouts for a user. This can be used to
        ///     show all the workouts a user had added.
        /// </summary>
        /// <param name="userId">UID of user</param>
        /// <returns>A collection of all workouts for a user</returns>
        Task<ICollection<WorkoutDto>> GetAllWorkoutsAsync(int userId);

        /// <summary>
        ///     Returns a specific workout and its entries
        /// </summary>
        /// <param name="workoutId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ICollection<WorkoutEntryDto>> GetWorkoutEntriesAsync(int workoutId);

        /// <summary>
        ///     Retrieves a set of workout based on a given date for a given user.
        /// </summary>
        /// <param name="userId">UID of user</param>
        /// <param name="requestDate">date to lookup</param>
        /// <returns></returns>
        Task<ICollection<WorkoutDto>> GetWorkoutOnDateAsync(int userId, DateTime requestDate);

        /// <summary>
        ///     Add entries to an existing workout
        /// </summary>
        /// <param name="workoutDto">Dto</param>
        /// <returns>Number of added workout entries</returns>
        Task<int> AddWorkoutEntryAsync(AddWorkoutEntryToWorkoutDto workoutDto);

        /// <summary>
        ///     Deletes a workout entry for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="entryId">id of entry</param>
        /// <returns>if success</returns>
        Task<bool> DeleteWorkoutEntryAsync(int userId, int entryId);


        /// <summary>
        ///     Get a specific workout with entries
        /// </summary>
        /// <param name="workoutId">uid of workout</param>
        /// <returns>Workout with entries</returns>
        Task<WorkoutDto> GetWorkoutAsync(int workoutId);

        /// <summary>
        ///     Get a specific workout entry
        /// </summary>
        /// <param name="entryId">uid of entry</param>
        /// <returns>workout entry</returns>
        Task<WorkoutEntryDto> GetWorkoutEntryAsync(int entryId);
    }
}