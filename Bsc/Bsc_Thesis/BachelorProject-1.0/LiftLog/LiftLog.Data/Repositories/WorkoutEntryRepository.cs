// WorkoutEntryRepository.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Threading.Tasks;
using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Data.Repositories
{
    public class WorkoutEntryRepository : GenericRepository<WorkoutEntry>, IWorkoutEntryRepository
    {
        public WorkoutEntryRepository(IContext context) : base(context)
        {
        }

        /// <summary>
        ///     Returns a workout entry with id
        /// </summary>
        /// <param name="id"> uid of entry</param>
        /// <returns></returns>
        public async Task<WorkoutEntry> GetWorkoutEntry(int id)
        {
            var entries = await GetAllAsync();
            return await entries
                .Include("Exercise")
                .Include("Workout")
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}