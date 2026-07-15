using Gamestore.BLL.DTOs.Publisher;
using Gamestore.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("publishers")]
public class PublisherController(IPublisherService publisherService, IGameService gameService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePublisher([FromBody] CreatePublisherRequestDto publisher)
    {
        await publisherService.CreateAsync(publisher);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPublishers()
    {
        var publishers = await publisherService.GetAllAsync();
        return Ok(publishers);
    }

    [HttpGet("{companyName}")]
    public async Task<IActionResult> GetPublisherByCompanyName(string companyName)
    {
        var publisher = await publisherService.GetByNameAsync(companyName);
        return Ok(publisher);
    }

    [HttpGet("{companyName}/games")]
    public async Task<IActionResult> GetGamesByCompanyName(string companyName)
    {
        var games = await gameService.GetByPublisherAsync(companyName);
        return Ok(games);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePublisher([FromBody] UpdatePublisherRequestDto publisher)
    {
        await publisherService.UpdateAsync(publisher);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePublisher(Guid id)
    {
        await publisherService.DeleteAsync(id);
        return Ok();
    }
}