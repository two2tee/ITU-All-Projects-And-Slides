// IExerciseRepository.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using LiftLog.Data.Entities;

namespace LiftLog.Data.Interfaces
{
    /// <summary>
    ///     Exercise repository with CRUD operations
    /// </summary>
    public interface IExerciseRepository : IRepository<Exercise>
    {
    }
}