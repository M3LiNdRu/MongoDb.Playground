using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;

namespace MongoDb.Playground.Repository
{
    public interface IViewModel { }

    public interface IReadonlyRepository<T> where T : IViewModel
    {
        Task<T> GetItemAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);
    }

    public abstract class ReadonlyRepository<T> : IReadonlyRepository<T> where T : IViewModel
    {

        public ReadonlyRepository(IDataStore<T> mongoStore, string collection)
        {
            _mongoStore = mongoStore.Create(collection);
        }

        public virtual Task<T> GetItemAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
        {
            return _mongoStore.ReadItemAsync(filter, cancellationToken);
        }

        public virtual Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
        {
            return _mongoStore.QueryItemsAsync(filter, cancellationToken);
        }
    }
}
