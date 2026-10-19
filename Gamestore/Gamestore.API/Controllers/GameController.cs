using System.Text;
using System.Text.Json;
using Gamestore.BLL.DTOs.Comment;
using Gamestore.BLL.DTOs.Game;
using Gamestore.BLL.DTOs.Game.Filtering;
using Gamestore.BLL.Interfaces;
using Gamestore.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("games")]
public class GameController(IGameService gameService, IGenreService genreService, IPlatformService platformService,
    IPublisherService publisherService, IOrderService orderService, ICommentService commentService) : ControllerBase
{
    private static readonly string[] PaginationOptions = ["10", "20", "50", "100", "all"];
    private static readonly string[] SortingOptions = ["Most popular", "Most commented", "Price ASC", "Price DESC", "New"];
    private static readonly string[] PublishDateOptions = ["last week", "last month", "last year", "2 years", "3 years"];

    private string LanguageCode => new LanguageService().NormalizeLanguageCode(HttpContext?.Request.Headers.AcceptLanguage.ToString());

    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequestDto request)
    {
        await gameService.CreateAsync(request, LanguageCode);
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
    public async Task<IActionResult> GetAllGames([FromQuery] GameFilterDto filter)
    {
        var games = await gameService.GetFilteredAsync(filter, LanguageCode);
        return Ok(games);
    }

    [HttpGet("find/{id}")]
    public async Task<IActionResult> GetGameById(Guid id)
    {
        var game = await gameService.GetByIdAsync(id, LanguageCode);
        return Ok(game);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetGameByKey(string key)
    {
        var game = await gameService.GetByKeyAsync(key, LanguageCode);
        return Ok(game);
    }

    [HttpGet("{key}/genres")]
    public async Task<IActionResult> GetGenresByGameKey(string key)
    {
        var genres = await genreService.GetByGameKeyAsync(key, LanguageCode);
        return Ok(genres);
    }

    [HttpGet("{key}/platforms")]
    public async Task<IActionResult> GetPlatformsByGameKey(string key)
    {
        var platforms = await platformService.GetByGameKeyAsync(key, LanguageCode);
        return Ok(platforms);
    }

    [HttpGet("{key}/publisher")]
    public async Task<IActionResult> GetPublisherByGameKey(string key)
    {
        var publisher = await publisherService.GetByGameKeyAsync(key, LanguageCode);
        return Ok(publisher);
    }

    [HttpGet("{key}/file")]
    public async Task<IActionResult> DownloadGameFile(string key)
    {
        var game = await gameService.GetByKeyAsync(key, LanguageCode);
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

    [HttpGet("pagination-options")]
    public IActionResult GetPaginationOptions()
        => Ok(PaginationOptions);

    [HttpGet("sorting-options")]
    public IActionResult GetSortingOptions()
        => Ok(SortingOptions);

    [HttpGet("publish-date-options")]
    public IActionResult GetPublishDateOptions()
        => Ok(PublishDateOptions);

    [HttpPut]
    public async Task<IActionResult> UpdateGame([FromBody] UpdateGameRequestDto request)
    {
        await gameService.UpdateAsync(request, LanguageCode);
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