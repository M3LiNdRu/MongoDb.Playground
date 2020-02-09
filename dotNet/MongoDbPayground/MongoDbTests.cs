using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDb.Playground.Repository;
using Xunit;

namespace MongoDbPayground
{
    public class MongoDbTests
    {
        private readonly IDataStore<Document> _repository;

        public MongoDbTests()
        {
            var connectionString = "mongodb://localhost:27017";
            var database = "mongolab";
            _repository = new DataStore<Document>(new OptionsMonitorDbConfiguration(connectionString, database), "Documents");
            Initialize();
        }

        public async Task Initialize()
        {
            var document = new Document
            {
                Id = Guid.NewGuid().ToString(),
                Name = "roger",
                Number = 30,
                Properties = new Dictionary<string, string>
                {
                    ["eyes"] = "brown",
                    ["hair"] = "black",
                },
                Tags = new List<string> { "waterpolo", "climbing", "developing" },
                Timestamp = DateTime.UtcNow
            };

            await _repository.Insert(document);
        }

        [Fact]
        public async Task Test_FindAll_Success()
        {
            //Arrange

            //Act
            var result = await _repository.FindAll();

            //Assert
            Assert.True(result.Any());
        }
    }
}
