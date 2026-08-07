using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.MongoDAL.Entities;

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
    public int Quantity { get; set; }

    [BsonElement("Discount")]
    public double Discount { get; set; }
}
