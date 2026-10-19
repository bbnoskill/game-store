using Gamestore.BLL.DTOs.Platform;

namespace Gamestore.BLL.Interfaces;

public interface IPlatformService
{
    Task<PlatformDto?> GetByIdAsync(Guid id, string languageCode = "en");

    Task<IEnumerable<PlatformDto>> GetAllAsync(string languageCode = "en");

    Task<IEnumerable<PlatformDto>> GetByGameKeyAsync(string gameKey, string languageCode = "en");

    Task CreateAsync(CreatePlatformRequestDto request, string languageCode = "en");

    Task UpdateAsync(UpdatePlatformRequestDto request, string languageCode = "en");

    Task DeleteAsync(Guid id);
}