using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.MongoDAL.Entities;

[BsonIgnoreExtraElements]
public class MongoCategory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("CategoryID")]
    public int CategoryID { get; set; }

    [BsonElement("CategoryName")]
    public string CategoryName { get; set; } = string.Empty;

    [BsonElement("Description")]
    public string? Description { get; set; }

    [BsonElement("Picture")]
    [BsonIgnoreIfNull]
    public object? Picture { get; set; }
}
