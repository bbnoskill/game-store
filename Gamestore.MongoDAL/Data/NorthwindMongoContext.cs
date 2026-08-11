using Gamestore.MongoDAL.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Gamestore.MongoDAL.Data;

public class NorthwindMongoContext
{
    public NorthwindMongoContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoConnection")
            ?? "mongodb://localhost:27017";
        var client = new MongoClient(connectionString);
        Database = client.GetDatabase("Northwind");
    }

    public IMongoDatabase Database { get; }

    public IMongoCollection<MongoProduct> Products => Database.GetCollection<MongoProduct>("products");

    public IMongoCollection<MongoCategory> Categories => Database.GetCollection<MongoCategory>("categories");

    public IMongoCollection<MongoSupplier> Suppliers => Database.GetCollection<MongoSupplier>("suppliers");

    public IMongoCollection<MongoOrder> Orders => Database.GetCollection<MongoOrder>("orders");

    public IMongoCollection<MongoOrderDetail> OrderDetails => Database.GetCollection<MongoOrderDetail>("order-details");

    public IMongoCollection<MongoShipper> Shippers => Database.GetCollection<MongoShipper>("shippers");

    public IMongoCollection<MongoChangeLog> ChangeLogs => Database.GetCollection<MongoChangeLog>("changeLogs");
}