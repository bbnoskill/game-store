using Gamestore.BLL.DTOs.Platform;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;

namespace Gamestore.BLL.Services;

public class PlatformService(IUnitOfWork unitOfWork) : IPlatformService
{
    public async Task<PlatformDto?> GetByIdAsync(Guid id)
    {
        var platform = await unitOfWork.Platforms.GetByIdAsync(id);
        return platform == null ? null : new PlatformDto
        {
            Id = platform.Id,
            Type = platform.Type,
        };
    }

    public async Task<IEnumerable<PlatformDto>> GetAllAsync()
    {
        var platforms = await unitOfWork.Platforms.GetAllAsync();
        return platforms.Select(platform => new PlatformDto
        {
            Id = platform.Id,
            Type = platform.Type,
        });
    }

    public async Task<IEnumerable<PlatformDto>> GetByGameKeyAsync(string gameKey)
    {
        var platforms = await unitOfWork.Platforms.GetByGameKeyAsync(gameKey);
        return platforms.Select(platform => new PlatformDto
        {
            Id = platform.Id,
            Type = platform.Type,
        });
    }

    public async Task CreateAsync(CreatePlatformRequestDto request)
    {
        var platform = new Platform
        {
            Id = Guid.NewGuid(),
            Type = request.Platform.Type,
        };

        await unitOfWork.Platforms.AddAsync(platform);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdatePlatformRequestDto request)
    {
        var platform = await unitOfWork.Platforms.GetByIdAsync(request.Platform.Id)
            ?? throw new KeyNotFoundException($"Platform with id {request.Platform.Id} not found");
        platform.Type = request.Platform.Type;

        await unitOfWork.Platforms.UpdateAsync(platform);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var platform = await unitOfWork.Platforms.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException($"Platform with id {id} not found");
        await unitOfWork.Platforms.DeleteAsync(platform);
        await unitOfWork.SaveChangesAsync();
    }
}