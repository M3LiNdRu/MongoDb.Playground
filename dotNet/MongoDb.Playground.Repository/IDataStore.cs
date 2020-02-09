using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDb.Playground.Repository
{
    public interface IDataStore<T> where T : ICollectionDocument
    {
        Task<IEnumerable<T>> FindAll();
        Task<T> FindOne(Expression<Func<T, bool>> predicate);
        Task Insert(T document);
    }
}