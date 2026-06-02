using Gamestore.BLL.DTOs.Platform;
using Gamestore.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("platforms")]
public class PlatformController : ControllerBase
{
    private readonly IPlatformService  _platformService;
    private readonly IGameService _gameService;

    public PlatformController(IPlatformService platformService, IGameService gameService)
    {
        _platformService = platformService;
        _gameService = gameService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlatform([FromBody] CreatePlatformRequestDto request)
    {
        await _platformService.CreateAsync(request);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPlatforms()
    {
        var platforms = await _platformService.GetAllAsync();
        return Ok(platforms);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlatformById(Guid id)
    {
        var platform = await _platformService.GetByIdAsync(id);
        return Ok(platform);
    }

    [HttpGet("{id}/games")]
    public async Task<IActionResult> GetPlatformGames(Guid id)
    {
        var games = await _gameService.GetByPlatformAsync(id);
        return Ok(games);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePlatform([FromBody] UpdatePlatformRequestDto request)
    {
        await _platformService.UpdateAsync(request);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlatform(Guid id)
    {
        await _platformService.DeleteAsync(id);
        return Ok();
    }
}