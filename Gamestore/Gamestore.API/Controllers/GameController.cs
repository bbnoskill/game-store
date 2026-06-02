using System.Text;
using System.Text.Json;
using Gamestore.BLL.DTOs.Game;
using Gamestore.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("games")]
public class GameController: ControllerBase
{
    private readonly IGameService  _gameService;
    private readonly IGenreService _genreService;
    private readonly IPlatformService _platformService;

    public GameController(IGameService gameService,  IGenreService genreService, IPlatformService platformService)
    {
        _gameService = gameService;
        _genreService = genreService;
        _platformService = platformService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequestDto request)
    {
        await _gameService.CreateAsync(request);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGames()
    {
        var games = await _gameService.GetAllAsync();
        return Ok(games);
    }

    [HttpGet("find/{id}")]
    public async Task<IActionResult> GetGameById(Guid id)
    {
        var game = await _gameService.GetByIdAsync(id);
        return Ok(game);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetGameByKey(string key)
    {
        var game = await _gameService.GetByKeyAsync(key);
        return Ok(game);
    }

    [HttpGet("{key}/genres")]
    public async Task<IActionResult> GetGenresByGameKey(string key)
    {
        var genres = await _genreService.GetByGameKeyAsync(key);
        return Ok(genres);
    }
    
    [HttpGet("{key}/platforms")]
    public async Task<IActionResult> GetPlatformsByGameKey(string key)
    {
        var platforms = await _platformService.GetByGameKeyAsync(key);
        return Ok(platforms);
    }
    
    [HttpGet("{key}/file")]
    public async Task<IActionResult> DownloadGameFile(string key)
    {
        var game = await _gameService.GetByKeyAsync(key);
        if (game is null) return NotFound();

        var content = JsonSerializer.Serialize(game);
        var fileName = $"{game.Name}_{DateTime.UtcNow:yyyyMMddHHmmss}.txt";
        var bytes = Encoding.UTF8.GetBytes(content);

        return File(bytes, "text/plain", fileName);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateGame([FromBody] UpdateGameRequestDto request)
    {
        await _gameService.UpdateAsync(request);
        return Ok();
    }

    [HttpDelete("{key}")]
    public async Task<IActionResult> DeleteGame(string key)
    {
        await _gameService.DeleteAsync(key);
        return Ok();
    }
    
}