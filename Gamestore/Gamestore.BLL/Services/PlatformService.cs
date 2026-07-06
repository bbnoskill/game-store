using Gamestore.BLL.DTOs.Platform;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Gamestore.BLL.Services;

public class PlatformService(IUnitOfWork unitOfWork, ILogger<GameService> logger) : IPlatformService
{
    public async Task<PlatformDto?> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Fetching platform by id: {Id}", id);
        var platform = await unitOfWork.Platforms.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException($"Platform with id '{id}' was not found.");
        return new PlatformDto
        {
            Id = platform.Id,
            Type = platform.Type,
        };
    }

    public async Task<IEnumerable<PlatformDto>> GetAllAsync()
    {
        logger.LogInformation("Fetching all platforms.");
        var platforms = (await unitOfWork.Platforms.GetAllAsync()).ToList();
        logger.LogInformation("Fetched {Count} platforms.", platforms.Count);
        return platforms.Select(platform => new PlatformDto
        {
            Id = platform.Id,
            Type = platform.Type,
        });
    }

    public async Task<IEnumerable<PlatformDto>> GetByGameKeyAsync(string gameKey)
    {
        logger.LogInformation("Fetching all platforms by game key: {GameKey}.", gameKey);
        var platforms = (await unitOfWork.Platforms.GetByGameKeyAsync(gameKey)).ToList();
        logger.LogInformation("Fetched {Count} platforms.", platforms.Count);
        return platforms.Select(platform => new PlatformDto
        {
            Id = platform.Id,
            Type = platform.Type,
        });
    }

    public async Task CreateAsync(CreatePlatformRequestDto request)
    {
        logger.LogInformation("Creating platform: {Type}", request.Platform.Type);
        var platform = new Platform
        {
            Id = Guid.NewGuid(),
            Type = request.Platform.Type,
        };

        await unitOfWork.Platforms.AddAsync(platform);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Created platform: {Type}", platform.Type);
    }

    public async Task UpdateAsync(UpdatePlatformRequestDto request)
    {
        logger.LogInformation("Updating platform: {Id}", request.Platform.Id);
        var platform = await unitOfWork.Platforms.GetByIdAsync(request.Platform.Id)
            ?? throw new KeyNotFoundException($"Platform with id {request.Platform.Id} not found");
        platform.Type = request.Platform.Type;

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