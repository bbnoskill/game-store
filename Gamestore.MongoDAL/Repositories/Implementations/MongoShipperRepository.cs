using Gamestore.MongoDAL.Data;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;

namespace Gamestore.MongoDAL.Repositories.Implementations;

public class MongoShipperRepository(NorthwindMongoContext context) : MongoRepository<MongoShipper>(context, "shippers"), IMongoShipperRepository
{
}
