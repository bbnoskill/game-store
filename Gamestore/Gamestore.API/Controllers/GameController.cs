using System.Text;
using System.Text.Json;
using Gamestore.BLL.DTOs.Comment;
using Gamestore.BLL.DTOs.Game;
using Gamestore.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("games")]
public class GameController(IGameService gameService, IGenreService genreService, IPlatformService platformService,
    IPublisherService publisherService, IOrderService orderService, ICommentService commentService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequestDto request)
    {
        await gameService.CreateAsync(request);
        return Ok();
    }

    [HttpPost("{key}/buy")]
    public async Task<IActionResult> AddGameToCart(string key)
    {
        await orderService.AddToCartAsync(key);
        return Ok();
    }

    [HttpPost("{key}/comments")]
    public async Task<IActionResult> AddComment(string key, [FromBody] CreateCommentRequestDto request)
    {
        await commentService.CreateAsync(key, request);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGames()
    {
        var games = await gameService.GetAllAsync();
        return Ok(games);
    }

    [HttpGet("find/{id}")]
    public async Task<IActionResult> GetGameById(Guid id)
    {
        var game = await gameService.GetByIdAsync(id);
        return Ok(game);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetGameByKey(string key)
    {
        var game = await gameService.GetByKeyAsync(key);
        return Ok(game);
    }

    [HttpGet("{key}/genres")]
    public async Task<IActionResult> GetGenresByGameKey(string key)
    {
        var genres = await genreService.GetByGameKeyAsync(key);
        return Ok(genres);
    }

    [HttpGet("{key}/platforms")]
    public async Task<IActionResult> GetPlatformsByGameKey(string key)
    {
        var platforms = await platformService.GetByGameKeyAsync(key);
        return Ok(platforms);
    }

    [HttpGet("{key}/publisher")]
    public async Task<IActionResult> GetPublisherByGameKey(string key)
    {
        var publisher = await publisherService.GetByGameKeyAsync(key);
        return Ok(publisher);
    }

    [HttpGet("{key}/file")]
    public async Task<IActionResult> DownloadGameFile(string key)
    {
        var game = await gameService.GetByKeyAsync(key);
        if (game is null)
        {
            return NotFound();
        }

        var content = JsonSerializer.Serialize(game);
        var fileName = $"{game.Name}_{DateTime.UtcNow:yyyyMMddHHmmss}.txt";
        var bytes = Encoding.UTF8.GetBytes(content);

        return File(bytes, "text/plain", fileName);
    }

    [HttpGet("{key}/comments")]
    public async Task<IActionResult> GetCommentsByGameKey(string key)
    {
        var comments = await commentService.GetByGameKeyAsync(key);
        return Ok(comments);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateGame([FromBody] UpdateGameRequestDto request)
    {
        await gameService.UpdateAsync(request);
        return Ok();
    }

    [HttpDelete("{key}")]
    public async Task<IActionResult> DeleteGame(string key)
    {
        await gameService.DeleteAsync(key);
        return Ok();
    }

    [HttpDelete("{key}/comments/{id}")]
    public async Task<IActionResult> DeleteComment(string key, Guid id)
    {
        await commentService.DeleteAsync(key, id);
        return Ok();
    }
}