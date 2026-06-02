using Gamestore.BLL.DTOs.Platform;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;

namespace Gamestore.BLL.Services;

public class PlatformService : IPlatformService
{
    private readonly IUnitOfWork _unitOfWork;

    public PlatformService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PlatformDto?> GetByIdAsync(Guid id)
    {
        var platform = await _unitOfWork.Platforms.GetByIdAsync(id);
        if (platform == null) return null;
        return new PlatformDto
        {
            Id = platform.Id,
            Type = platform.Type,
        };
    }

    public async Task<IEnumerable<PlatformDto>> GetAllAsync()
    {
        var platforms = await _unitOfWork.Platforms.GetAllAsync();
        return platforms.Select(platform => new PlatformDto
        {
            Id = platform.Id,
            Type = platform.Type,
        });
    }

    public async Task<IEnumerable<PlatformDto>> GetByGameKeyAsync(string key)
    {
        var platforms = await _unitOfWork.Platforms.GetByGameKeyAsync(key);
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
        
        await _unitOfWork.Platforms.AddAsync(platform);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdatePlatformRequestDto request)
    {
        var platform = await _unitOfWork.Platforms.GetByIdAsync(request.Platform.Id)
            ?? throw new KeyNotFoundException($"Platform with id {request.Platform.Id} not found");
        platform.Type = request.Platform.Type;
        
        await _unitOfWork.Platforms.UpdateAsync(platform);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var platform = await _unitOfWork.Platforms.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException($"Platform with id {id} not found");
        await _unitOfWork.Platforms.DeleteAsync(platform);
    }
}