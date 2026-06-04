using Gamestore.BLL.DTOs.Genre;
using Gamestore.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("genres")]
public class GenreController(IGameService gameService, IGenreService genreService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreRequestDto request)
    {
        await genreService.CreateAsync(request);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGenreById(Guid id)
    {
        var genre = await genreService.GetByIdAsync(id);
        return Ok(genre);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGenres()
    {
        var genres = await genreService.GetAllAsync();
        return Ok(genres);
    }

    [HttpGet("{id}/games")]
    public async Task<IActionResult> GetGamesByGenre(Guid id)
    {
        var games = await gameService.GetByGenreAsync(id);
        return Ok(games);
    }

    [HttpGet("{id}/genres")]
    public async Task<IActionResult> GetSubgenresByGenre(Guid id)
    {
        var subgenres = await genreService.GetByParentIdAsync(id);
        return Ok(subgenres);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateGenre([FromBody] UpdateGenreRequestDto request)
    {
        await genreService.UpdateAsync(request);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(Guid id)
    {
        await genreService.DeleteAsync(id);
        return Ok();
    }
}