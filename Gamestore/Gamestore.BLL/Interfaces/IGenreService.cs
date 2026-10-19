using Gamestore.BLL.DTOs.Genre;

namespace Gamestore.BLL.Interfaces;

public interface IGenreService
{
    Task<GenreDto?> GetByIdAsync(Guid id, string languageCode = "en", bool includeMongo = true);

    Task<IEnumerable<GenreDto>> GetAllAsync(string languageCode = "en", bool includeMongo = true);

    Task<IEnumerable<GenreDto>> GetByGameKeyAsync(string gameKey, string languageCode = "en");

    Task<IEnumerable<GenreDto>> GetByParentIdAsync(Guid parentId, string languageCode = "en");

    Task CreateAsync(CreateGenreRequestDto request, string languageCode = "en");

    Task UpdateAsync(UpdateGenreRequestDto request, string languageCode = "en");

    Task DeleteAsync(Guid id);
}