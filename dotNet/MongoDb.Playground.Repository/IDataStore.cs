using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDb.Playground.Repository
{
    public interface IDataStore<T> where T : ICollectionDocument
    {
        Task<IEnumerable<T>> FindAll();
        Task<IEnumerable<T>> FindAllAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task<T> FindOne(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task Insert(T document);
        Task Delete(Expression<Func<T, bool>> predicate);
        Task DeleteAll();
        Task UpdateOne(T document);
    }
}