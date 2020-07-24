// IWorkoutRepository.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Threading.Tasks;
using LiftLog.Data.Entities;

namespace LiftLog.Data.Interfaces
{
    /// <summary>
    ///     Workout repository with CRUD operations
    /// </summary>
    public interface IWorkoutRepository : IRepository<Workout>
    {

        /// <summary>
        /// Gets workout with entries
        /// </summary>
        /// <param name="workoutId"></param>
        /// <returns></returns>
        Task<Workout> GetWorkoutWithEntries(int workoutId);
    }
}