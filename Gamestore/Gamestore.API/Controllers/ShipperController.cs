using Gamestore.MongoDAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("shippers")]
public class ShipperController(IMongoShipperRepository shipperRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetShippersAsync()
    {
        var shippers = await shipperRepository.GetAllAsync();
        return Ok(shippers);
    }
}