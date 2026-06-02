using Gamestore.BLL.DTOs.Game;
using Gamestore.BLL.DTOs.Platform;
using Gamestore.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("[controller]")]
public class GamesController: ControllerBase
{
    private readonly IGameService  _gameService;
    private readonly IGenreService _genreService;

    public GamesController(IGameService gameService,  IGenreService genreService)
    {
        _gameService = gameService;
        _genreService = genreService;
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

    [HttpGet("{find/id}")]
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

    [HttpGet("{genreId:guid}")]
    public async Task<IActionResult> GetGameByGenre(Guid genreId)
    {
        var game = await _gameService.GetByGenreAsync(genreId);
        return Ok(game);
    }
    
}