using Gamestore.BLL.DTOs.Genre;
using Gamestore.BLL.Interfaces;
using Gamestore.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("genres")]
public class GenreController(IGameService gameService, IGenreService genreService) : ControllerBase
{
    private string LanguageCode => new LanguageService().NormalizeLanguageCode(HttpContext?.Request.Headers.AcceptLanguage.ToString());

    [HttpPost]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreRequestDto request)
    {
        await genreService.CreateAsync(request, LanguageCode);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGenreById(Guid id)
    {
        var genre = await genreService.GetByIdAsync(id, LanguageCode);
        return Ok(genre);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGenres()
    {
        var genres = await genreService.GetAllAsync(LanguageCode);
        return Ok(genres);
    }

    [HttpGet("{id}/games")]
    public async Task<IActionResult> GetGamesByGenre(Guid id)
    {
        var games = await gameService.GetByGenreAsync(id, LanguageCode);
        return Ok(games);
    }

    [HttpGet("{id}/genres")]
    public async Task<IActionResult> GetSubgenresByGenre(Guid id)
    {
        var subgenres = await genreService.GetByParentIdAsync(id, LanguageCode);
        return Ok(subgenres);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateGenre([FromBody] UpdateGenreRequestDto request)
    {
        await genreService.UpdateAsync(request, LanguageCode);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(Guid id)
    {
        await genreService.DeleteAsync(id);
        return Ok();
    }
}