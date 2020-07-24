// IWorkoutEntryRepository.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Threading.Tasks;
using LiftLog.Data.Entities;

namespace LiftLog.Data.Interfaces
{
    /// <summary>
    ///     Workout entry repository with CRUD operations
    /// </summary>
    public interface IWorkoutEntryRepository : IRepository<WorkoutEntry>
    {
        /// <summary>
        /// Gets workout entry with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>entry entity</returns>
        Task<WorkoutEntry> GetWorkoutEntry(int id);
    }
}