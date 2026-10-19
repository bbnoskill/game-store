using Gamestore.BLL.DTOs.Platform;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Gamestore.BLL.Services;

public class PlatformService(IUnitOfWork unitOfWork, ILogger<GameService> logger) : IPlatformService
{
    public async Task<PlatformDto?> GetByIdAsync(Guid id, string languageCode = "en")
    {
        logger.LogInformation("Fetching platform by id: {Id}", id);
        var platform = await unitOfWork.Platforms.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException($"Platform with id '{id}' was not found.");
        return new PlatformDto
        {
            Id = platform.Id,
            Type = platform.Localizations.FirstOrDefault(l => l.LanguageCode == languageCode)?.Type
                ?? platform.Localizations.FirstOrDefault(l => l.LanguageCode == "en")?.Type
                ?? platform.Type,
        };
    }

    public async Task<IEnumerable<PlatformDto>> GetAllAsync(string languageCode = "en")
    {
        logger.LogInformation("Fetching all platforms.");
        var platforms = (await unitOfWork.Platforms.GetAllAsync()).ToList();
        logger.LogInformation("Fetched {Count} platforms.", platforms.Count);
        return platforms.Select(platform => new PlatformDto
        {
            Id = platform.Id,
            Type = platform.Localizations.FirstOrDefault(l => l.LanguageCode == languageCode)?.Type
                ?? platform.Localizations.FirstOrDefault(l => l.LanguageCode == "en")?.Type
                ?? platform.Type,
        });
    }

    public async Task<IEnumerable<PlatformDto>> GetByGameKeyAsync(string gameKey, string languageCode = "en")
    {
        logger.LogInformation("Fetching all platforms by game key: {GameKey}.", gameKey);
        var platforms = (await unitOfWork.Platforms.GetByGameKeyAsync(gameKey)).ToList();
        logger.LogInformation("Fetched {Count} platforms.", platforms.Count);
        return platforms.Select(platform => new PlatformDto
        {
            Id = platform.Id,
            Type = platform.Localizations.FirstOrDefault(l => l.LanguageCode == languageCode)?.Type
                ?? platform.Localizations.FirstOrDefault(l => l.LanguageCode == "en")?.Type
                ?? platform.Type,
        });
    }

    public async Task CreateAsync(CreatePlatformRequestDto request, string languageCode = "en")
    {
        logger.LogInformation("Creating platform: {Type}", request.Platform.Type);
        var platform = new Platform
        {
            Id = Guid.NewGuid(),
            Type = request.Platform.Type,
        };

        EntityLocalization.Update(platform, languageCode, request.Platform.Type);

        await unitOfWork.Platforms.AddAsync(platform);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Created platform: {Type}", platform.Type);
    }

    public async Task UpdateAsync(UpdatePlatformRequestDto request, string languageCode = "en")
    {
        logger.LogInformation("Updating platform: {Id}", request.Platform.Id);
        var platform = await unitOfWork.Platforms.GetByIdAsync(request.Platform.Id)
            ?? throw new KeyNotFoundException($"Platform with id {request.Platform.Id} not found");
        EntityLocalization.Update(platform, languageCode, request.Platform.Type);

        unitOfWork.Platforms.Update(platform);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Updated platform: {Id}", platform.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("Deleting platform with id: {Id}", id);
        var platform = await unitOfWork.Platforms.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException($"Platform with id {id} not found");
        unitOfWork.Platforms.Delete(platform);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Deleted platform with id: {Id}", id);
    }
}