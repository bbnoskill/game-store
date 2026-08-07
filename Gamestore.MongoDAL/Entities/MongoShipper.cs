using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.MongoDAL.Entities;

public class MongoShipper
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("ShipperID")]
    public int ShipperID { get; set; }

    [BsonElement("CompanyName")]
    public string CompanyName { get; set; } = string.Empty;

    [BsonElement("Phone")]
    public string? Phone { get; set; }
}
