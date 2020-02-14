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
        }

        private async Task Initialize()
        {
            var document = new Document
            {
                Id = "document-0",
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
            var document1 = new Document
            {
                Id = "document-1",
                Name = "adria",
                Number = 33,
                Properties = new Dictionary<string, string>
                {
                    ["eyes"] = "brown",
                    ["hair"] = "black",
                },
                Tags = new List<string> { "beer", "swim", "politics" },
                Timestamp = DateTime.UtcNow
            };

            await _repository.Insert(document);
            await _repository.Insert(document1);
        }

        private async Task Clean()
        {
            await _repository.DeleteAll();
        }

        [Fact]
        public async Task Test_FindAll_Success()
        {
            //Arrange
            await Initialize();

            //Act
            var result = await _repository.FindAll();

            //Assert
            Assert.True(result.Any());
            Assert.True(result.Count() == 2);

            await Clean();
        }

        [Theory]
        [InlineData("document-0")]
        public async Task Test_FindOne_Success(string id)
        {
            //Arrange
            await Initialize();

            //Act
            var result = await _repository.FindOne(d => d.Id == id);

            //Assert
            Assert.True(result.Id == id);
            Assert.IsType<Document>(result);

            await Clean();
        }

        [Fact]
        public async Task Test_Insert_Success()
        {
            //Arrange
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

            //Act
            await _repository.Insert(document);
            var result = await _repository.FindOne(d => d.Id == document.Id);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Id == document.Id);

            await Clean();
        }

        [Theory]
        [InlineData("document-0")]
        public async Task Test_Edit_Success(string id)
        {
            //Arrange
            await Initialize();
            var document = await _repository.FindOne(d => d.Id == id);
            document.Number = 60;
            //Act
            await _repository.UpdateOne(document);
            var result = await _repository.FindOne(d => d.Id == id);

            //Assert
            Assert.True(result.Number == document.Number);
            Assert.True(result.Id == document.Id);

            await Clean();
        }

        [Theory]
        [InlineData("document-0")]
        public async Task Test_Delete_Success(string id)
        {
            //Arrange
            await Initialize();

            //Act
            await _repository.Delete(d => d.Id == id);
            var result = await _repository.FindOne(d => d.Id == id);

            //Assert
            Assert.Null(result);

            await Clean();
        }
    }
}
