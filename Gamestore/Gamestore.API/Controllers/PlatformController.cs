using Gamestore.BLL.DTOs.Platform;
using Gamestore.BLL.Interfaces;
using Gamestore.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("platforms")]
public class PlatformController(IPlatformService platformService, IGameService gameService) : ControllerBase
{
    private string LanguageCode => new LanguageService().NormalizeLanguageCode(HttpContext?.Request.Headers.AcceptLanguage.ToString());

    [HttpPost]
    public async Task<IActionResult> CreatePlatform([FromBody] CreatePlatformRequestDto request)
    {
        await platformService.CreateAsync(request, LanguageCode);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPlatforms()
    {
        var platforms = await platformService.GetAllAsync(LanguageCode);
        return Ok(platforms);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlatformById(Guid id)
    {
        var platform = await platformService.GetByIdAsync(id, LanguageCode);
        return Ok(platform);
    }

    [HttpGet("{id}/games")]
    public async Task<IActionResult> GetPlatformGames(Guid id)
    {
        var games = await gameService.GetByPlatformAsync(id, LanguageCode);
        return Ok(games);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePlatform([FromBody] UpdatePlatformRequestDto request)
    {
        await platformService.UpdateAsync(request, LanguageCode);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlatform(Guid id)
    {
        await platformService.DeleteAsync(id);
        return Ok();
    }
}