using Gamestore.BLL.DTOs.Publisher;
using Gamestore.BLL.Interfaces;
using Gamestore.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("publishers")]
public class PublisherController(IPublisherService publisherService, IGameService gameService) : ControllerBase
{
    private string LanguageCode => new LanguageService().NormalizeLanguageCode(HttpContext?.Request.Headers.AcceptLanguage.ToString());

    [HttpPost]
    public async Task<IActionResult> CreatePublisher([FromBody] CreatePublisherRequestDto publisher)
    {
        await publisherService.CreateAsync(publisher, LanguageCode);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPublishers([FromQuery] bool includeMongo = true)
    {
        var publishers = await publisherService.GetAllAsync(LanguageCode, includeMongo);
        return Ok(publishers);
    }

    [HttpGet("{companyName}")]
    public async Task<IActionResult> GetPublisherByCompanyName(string companyName, [FromQuery] bool includeMongo = true)
    {
        var publisher = await publisherService.GetByNameAsync(companyName, LanguageCode, includeMongo);
        return Ok(publisher);
    }

    [HttpGet("{companyName}/games")]
    public async Task<IActionResult> GetGamesByCompanyName(string companyName)
    {
        var games = await gameService.GetByPublisherAsync(companyName, LanguageCode);
        return Ok(games);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePublisher([FromBody] UpdatePublisherRequestDto publisher)
    {
        await publisherService.UpdateAsync(publisher, LanguageCode);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePublisher(Guid id)
    {
        await publisherService.DeleteAsync(id);
        return Ok();
    }
}