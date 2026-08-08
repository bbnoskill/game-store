using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.MongoDAL.Entities;

public class MongoProduct
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("ProductID")]
    public int ProductID { get; set; }

    [BsonElement("ProductName")]
    public string ProductName { get; set; } = string.Empty;

    [BsonElement("SupplierID")]
    public int? SupplierID { get; set; }

    [BsonElement("CategoryID")]
    public int? CategoryID { get; set; }

    [BsonElement("QuantityPerUnit")]
    public string? QuantityPerUnit { get; set; }

    [BsonElement("UnitPrice")]
    public decimal? UnitPrice { get; set; }

    [BsonElement("UnitsInStock")]
    public int? UnitsInStock { get; set; }

    [BsonElement("UnitsOnOrder")]
    public int? UnitsOnOrder { get; set; }

    [BsonElement("ReorderLevel")]
    public int? ReorderLevel { get; set; }

    [BsonElement("Discontinued")]
    public bool Discontinued { get; set; }

    [BsonElement("GameStoreKey")]
    [BsonIgnoreIfNull]
    public string? GameStoreKey { get; set; }

    [BsonElement("ViewsCount")]
    public int ViewsCount { get; set; }
}
