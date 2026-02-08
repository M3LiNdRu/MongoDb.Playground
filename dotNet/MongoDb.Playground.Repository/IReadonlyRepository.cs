using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;

namespace MongoDb.Playground.Repository
{
    public interface IViewModel : ICollectionDocument { }

    public interface IReadonlyRepository<T> where T : IViewModel
    {
        Task<T> GetItemAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);
    }

    public abstract class ReadonlyRepository<T>(IDataStore<T> mongoStore) : IReadonlyRepository<T> where T : IViewModel
    {
        protected  readonly IDataStore<T> _mongoStore = mongoStore;

        public virtual Task<T> GetItemAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
        {
            return _mongoStore.FindOneAsync(filter, cancellationToken);
        }

        public virtual Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
        {
            return _mongoStore.FindAsync(filter, cancellationToken);
        }
    }
}
