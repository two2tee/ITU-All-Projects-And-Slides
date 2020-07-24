// GenericRepository.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Linq;
using System.Threading.Tasks;
using LiftLog.Data.Interfaces;

namespace LiftLog.Data
{
    /// <summary>
    ///     Represents generic repository used to perform CRUD operations in DAL, implemented by all repositories as an
    ///     abstract class.
    ///     Virtual is used to indicate that default signature in interface is overriden so that CRUD operations are async.
    /// </summary>
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        protected readonly IContext Context;

        // Database context is injected into repository 
        protected GenericRepository(IContext context)
        {
            Context = context;
        }

        /// <summary>
        ///     Dispose the database context.
        /// </summary>
        public void Dispose()
        {
            Context.Dispose();
        }

        /// <summary>
        ///     Creates a given entity asynchronously.
        /// </summary>
        /// <param name="entity"> Entity to create. </param>
        /// <returns> Id of newly created entity. </returns>
        public virtual async Task<int> CreateAsync(T entity)
        {
            var result = await Context.Set<T>().AddAsync(entity);
            if (await Context.SaveChangesAsync() > 1)
                return -1;
            return result.Entity.Id;
        }

        /// <summary>
        ///     Finds the entity with the given id and returns it.
        /// </summary>
        /// <param name="id"> Id of the entity searched for. </param>
        /// <returns> Entity with the given id.</returns>
        public virtual async Task<T> FindAsync(int id)
        {
            return await Context.Set<T>().FindAsync(id);
        }

        /// <summary>
        ///     Updates the entity with a matching id, to the given entity.
        /// </summary>
        /// <param name="entity">The new version of the entity to update. </param>
        /// <returns> Boolean indicating if the operation was successful, true if success. </returns>
        public virtual async Task<bool> UpdateAsync(T entity)
        {
            if (entity == null) return false;
            Context.Set<T>().Update(entity);
            return await Context.SaveChangesAsync() > 0;
        }

        /// <summary>
        ///     Deletes the entity with the given id if it exists.
        /// </summary>
        /// <param name="id"> Id of the entity to delete. </param>
        /// <returns> Boolean indicating if the operation was successful, true if success. </returns>
        public virtual async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = Context.Set<T>().First(e => e.Id == id);
                Context.Set<T>().Remove(entity);
                return await Context.SaveChangesAsync() == 1;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e); //TODO LOG this
                return false;
            }
        }

        /// <summary>
        /// Returns a queryable collection of entities
        /// </summary>
        /// <returns>quryable collection</returns>
        public virtual async Task<IQueryable<T>> GetAllAsync()
        {
            return Context.Set<T>().AsQueryable();
        }
    }
}