// IRepository.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Linq;
using System.Threading.Tasks;

namespace LiftLog.Data.Interfaces
{
    /// <summary>
    ///     repository with CRUD operations
    /// </summary>
    public interface IRepository<T> : IDisposable
    {
        /// <summary>
        ///     Creates a new entity in database
        /// </summary>
        /// <param name="entity">The entity to be added</param>
        /// <returns>The UID of the created entity</returns>
        Task<int> CreateAsync(T entity);

        /// <summary>
        ///     Search for a specific entity based on a uid.
        /// </summary>
        /// <param name="id">uid of entity</param>
        /// <returns>Thee entity</returns>
        Task<T> FindAsync(int id);

        /// <summary>
        ///     Updates an existing entity
        /// </summary>
        /// <param name="entity">entity to be modified</param>
        /// <returns>If update was successfull</returns>
        Task<bool> UpdateAsync(T entity);

        /// <summary>
        ///     Delets an existing entity based on its UID
        /// </summary>
        /// <param name="id">UID of entity</param>
        /// <returns>of deletion was successfull</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        ///     Retrieves all entities
        /// </summary>
        /// <returns>All entities in queryable format</returns>
        Task<IQueryable<T>> GetAllAsync();
    }
}