
namespace Sample;

[BsonKnownTypes(typeof(TopSection), typeof(FooterSection))]
public record Section(string Type);

[BsonDiscriminator("TopSection")]
public record TopSection(string Type, string Title, string Description) : Section(Type);

[BsonDiscriminator("FooterSection")]
public record FooterSection(string Type, string Title, string Link) : Section(Type);


public class Repository 
{
    public Repository()
    {
        //BsonSerializer.RegisterDiscriminatorConvention()
    }
}