using Gamestore.BLL.DTOs.Game;

namespace Gamestore.BLL.Interfaces;

public interface IGameService
{
    Task<GameDto?> GetByKeyAsync(string key);

    Task<GameDto?> GetByIdAsync(Guid id);

    Task<IEnumerable<GameDto>> GetAllAsync();

    Task<IEnumerable<GameDto>> GetByGenreAsync(Guid genreId);

    Task<IEnumerable<GameDto>> GetByPlatformAsync(Guid platformId);

    Task<IEnumerable<GameDto>> GetByPublisherAsync(string companyName);

    Task CreateAsync(CreateGameRequestDto request);

    Task UpdateAsync(UpdateGameRequestDto request);

    Task DeleteAsync(string key);

    Task<int> GetTotalCountAsync();
}