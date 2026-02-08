using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDb.Playground.Repository.Configuration;
using MongoDB.Driver;

namespace MongoDb.Playground.Repository
{
    public class DataStore<T> : IDataStore<T> where T : ICollectionDocument
    {
        private readonly IMongoDatabase _db;
        private readonly string _collection;

        public DataStore(IOptionsMonitor<DbConfigurationSettings> options, string collection) 
        {
            var mongo = new MongoClient(options.CurrentValue.ConnectionString);
            _db = mongo.GetDatabase(options.CurrentValue.Database);
            _collection = collection;
        }


        public Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            return _db.GetCollection<T>(_collection).DeleteManyAsync(FilterDefinition<T>.Empty, cancellationToken);
        }

        public Task DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return _db.GetCollection<T>(_collection).DeleteOneAsync(predicate, cancellationToken);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _db.GetCollection<T>(_collection).Find<T>(FilterDefinition<T>.Empty).ToListAsync<T>(cancellationToken);
        }

        public async Task<T> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _db.GetCollection<T>(_collection).Find<T>(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        public Task InsertAsync(T document, CancellationToken cancellationToken)
        {
            var options = new InsertOneOptions { BypassDocumentValidation = false };
            return _db.GetCollection<T>(_collection).InsertOneAsync(document, options, cancellationToken);
        }

        public Task UpdateOneAsync(T document, CancellationToken cancellationToken)
        {
            var filter = Builders<T>.Filter.Eq(s => s.Id, document.Id);
            return _db.GetCollection<T>(_collection).ReplaceOneAsync(filter, document, cancellationToken: cancellationToken);
        }
    }
}
 