using Gamestore.BLL.DTOs.Game;
using Gamestore.BLL.DTOs.Genre;
using Gamestore.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;
[ApiController]
[Route("genres")]
public class GenreController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IGenreService _genreService;

    public GenreController(IGameService gameService, IGenreService genreService)
    {
        _gameService = gameService;
        _genreService = genreService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreRequestDto request)
    {
        await _genreService.CreateAsync(request);
        return Ok();
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGenreById(Guid id)
    {
        var genre = await _genreService.GetByIdAsync(id);
        return Ok(genre);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGenres()
    {
        var genres = await _genreService.GetAllAsync();
        return Ok(genres);
    }

    [HttpGet("{id}/games")]
    public async Task<IActionResult> GetGamesByGenre(Guid id)
    {
        var games  = await _gameService.GetByGenreAsync(id);
        return Ok(games);
    }

    [HttpGet("{id}/genres")]
    public async Task<IActionResult> GetSubgenresByGenre(Guid id)
    {
        var subgenres = await _genreService.GetByParentIdAsync(id);
        return Ok(subgenres);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateGenre([FromBody] UpdateGenreRequestDto request)
    {
        await _genreService.UpdateAsync(request);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(Guid id)
    {
        await _genreService.DeleteAsync(id);
        return Ok();
    }
}