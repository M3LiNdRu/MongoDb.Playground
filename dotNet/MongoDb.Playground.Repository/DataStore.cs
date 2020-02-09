using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        public async Task<IEnumerable<T>> FindAll()
        {
            return await _db.GetCollection<T>(_collection).Find<T>(FilterDefinition<T>.Empty).ToListAsync<T>();
        }

        public async Task<T> FindOne(Expression<Func<T, bool>> predicate)
        {
            return await _db.GetCollection<T>(_collection).Find<T>(predicate).FirstOrDefaultAsync();
        }

        public async Task Insert(T document)
        {
            await _db.GetCollection<T>(_collection).InsertOneAsync(document);
        }
    }
}
 