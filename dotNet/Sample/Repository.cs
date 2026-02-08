using MongoDb.Playground.Repository;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;

namespace Sample;

[BsonKnownTypes(typeof(TopSection), typeof(FooterSection))]
public record Section(string Type);

[BsonDiscriminator("TopSection")]
public record TopSection(string Type, string Title, string Description) : Section(Type);

[BsonDiscriminator("FooterSection")]
public record FooterSection(string Type, string Title, string Link) : Section(Type);

[BsonDiscriminator("MiddleSection")]
public record MiddleSection(string Type, string Title, int Views) : Section(Type);

public record Page : IViewModel
{
    [BsonId]
    public required string Id { get; set; }

    public string Language { get; init; }

    public IEnumerable<Section> Sections { get; init; } = [];
}

public interface IReadonlyPageRepository : IReadonlyRepository<Page> { }

public class Repository : ReadonlyRepository<Page>, IReadonlyPageRepository
{
    public Repository(IDataStore<Page> store) : base(store)
    {
        BsonSerializer.RegisterDiscriminatorConvention(typeof(Section), new ScalarDiscriminatorConvention("type"));
    }
}