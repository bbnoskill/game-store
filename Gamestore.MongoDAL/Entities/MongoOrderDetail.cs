using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.MongoDAL.Entities;

[BsonIgnoreExtraElements]
public class MongoOrderDetail
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("OrderID")]
    public int OrderID { get; set; }

    [BsonElement("ProductID")]
    public int ProductID { get; set; }

    [BsonElement("UnitPrice")]
    public decimal UnitPrice { get; set; }

    [BsonElement("Quantity")]
    public short Quantity { get; set; }

    [BsonElement("Discount")]
    public float Discount { get; set; }
}
