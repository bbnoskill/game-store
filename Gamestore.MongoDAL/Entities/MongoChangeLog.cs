using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.MongoDAL.Entities;

[BsonIgnoreExtraElements]
public class MongoChangeLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("DateTime")]
    public DateTime DateTime { get; set; }

    [BsonElement("Action")]
    public string Action { get; set; } = string.Empty;

    [BsonElement("EntityType")]
    public string EntityType { get; set; } = string.Empty;

    [BsonElement("OldVersion")]
    [BsonIgnoreIfNull]
    public BsonDocument? OldVersion { get; set; }

    [BsonElement("NewVersion")]
    [BsonIgnoreIfNull]
    public BsonDocument? NewVersion { get; set; }
}
