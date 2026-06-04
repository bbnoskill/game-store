using Gamestore.BLL.DTOs.Genre;

namespace Gamestore.BLL.Interfaces;

public interface IGenreService
{
    Task<GenreDto?> GetByIdAsync(Guid id);

    Task<IEnumerable<GenreDto>> GetAllAsync();

    Task<IEnumerable<GenreDto>> GetByGameKeyAsync(string gameKey);

    Task<IEnumerable<GenreDto>> GetByParentIdAsync(Guid parentId);

    Task CreateAsync(CreateGenreRequestDto request);

    Task UpdateAsync(UpdateGenreRequestDto request);

    Task DeleteAsync(Guid id);
}