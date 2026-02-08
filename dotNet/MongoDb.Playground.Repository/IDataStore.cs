using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDb.Playground.Repository
{
    public interface ICollectionDocument
    {
        string Id { get; set; }
    }
    
    public interface IDataStore<T> where T : ICollectionDocument
    {
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task<T> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task InsertAsync(T document, CancellationToken cancellationToken);
        Task DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task DeleteAllAsync(CancellationToken cancellationToken);
        Task UpdateOneAsync(T document, CancellationToken cancellationToken);
    }
}