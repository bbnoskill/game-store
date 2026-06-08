using Gamestore.BLL.DTOs.Platform;
using Gamestore.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("platforms")]
public class PlatformController(IPlatformService platformService, IGameService gameService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePlatform([FromBody] CreatePlatformRequestDto request)
    {
        await platformService.CreateAsync(request);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPlatforms()
    {
        var platforms = await platformService.GetAllAsync();
        return Ok(platforms);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlatformById(Guid id)
    {
        var platform = await platformService.GetByIdAsync(id);
        return Ok(platform);
    }

    [HttpGet("{id}/games")]
    public async Task<IActionResult> GetPlatformGames(Guid id)
    {
        var games = await gameService.GetByPlatformAsync(id);
        return Ok(games);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePlatform([FromBody] UpdatePlatformRequestDto request)
    {
        await platformService.UpdateAsync(request);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlatform(Guid id)
    {
        await platformService.DeleteAsync(id);
        return Ok();
    }
}