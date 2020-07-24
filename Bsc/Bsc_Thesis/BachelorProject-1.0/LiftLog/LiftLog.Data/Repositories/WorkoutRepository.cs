// WorkoutRepository.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Linq;
using System.Threading.Tasks;
using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Data.Repositories
{
    public class WorkoutRepository : GenericRepository<Workout>, IWorkoutRepository
    {
        public WorkoutRepository(IContext context) : base(context)
        {
        }

        /// <summary>
        ///     Gets workout with workout entries
        /// </summary>
        /// <param name="workoutId">uid of workout</param>
        /// <returns>workout</returns>
        public async Task<Workout> GetWorkoutWithEntries(int workoutId)
        {
            var entities = await GetAllAsync();
            var result = entities.Include("WorkoutEntries");
            return result.FirstOrDefault(workout => workout.Id == workoutId);
        }
    }
}