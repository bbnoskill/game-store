using Gamestore.BLL.DTOs.Game;
using Gamestore.BLL.DTOs.Game.Filtering;

namespace Gamestore.BLL.Interfaces;

public interface IGameService
{
    Task<GameDto?> GetByKeyAsync(string key, string languageCode = "en", bool includeMongo = true);

    Task<GameDto?> GetByIdAsync(Guid id, string languageCode = "en");

    Task<IEnumerable<GameDto>> GetAllAsync(string languageCode = "en", bool includeMongo = true);

    Task<IEnumerable<GameDto>> GetByGenreAsync(Guid genreId, string languageCode = "en");

    Task<IEnumerable<GameDto>> GetByPlatformAsync(Guid platformId, string languageCode = "en");

    Task<IEnumerable<GameDto>> GetByPublisherAsync(string companyName, string languageCode = "en");

    Task CreateAsync(CreateGameRequestDto request, string languageCode = "en");

    Task UpdateAsync(UpdateGameRequestDto request, string languageCode = "en");

    Task DeleteAsync(string key);

    Task<int> GetTotalCountAsync();

    Task<GameFilterResultDto> GetFilteredAsync(GameFilterDto filter, string languageCode = "en", bool includeMongo = true);
}