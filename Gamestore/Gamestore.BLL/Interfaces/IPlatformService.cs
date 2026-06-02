using Gamestore.BLL.DTOs.Platform;

namespace Gamestore.BLL.Interfaces;

public interface IPlatformService
{
    Task<PlatformDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<PlatformDto>> GetAllAsync();
    Task<IEnumerable<PlatformDto>> GetByGameKeyAsync(string gameKey);
    Task CreateAsync(CreatePlatformRequestDto request);
    Task UpdateAsync(UpdatePlatformRequestDto platform);
    Task DeleteAsync(Guid id);
}