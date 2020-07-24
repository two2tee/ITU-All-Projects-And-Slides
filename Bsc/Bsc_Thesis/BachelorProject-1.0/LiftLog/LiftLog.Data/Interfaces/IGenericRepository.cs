// IGenericRepository.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

namespace LiftLog.Data.Interfaces
{
    public interface IGenericRepository<T> : IRepository<T> where T : class, IEntity
    {
    }
}