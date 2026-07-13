using Gamestore.BLL.DTOs.Genre;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Gamestore.BLL.Services;

public class GenreService(IUnitOfWork unitOfWork, ILogger<GameService> logger) : IGenreService
{
    public async Task<GenreDto?> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Fetching genre by id: {Id}", id);
        var genre = await unitOfWork.Genres.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Genre with id '{id}' was not found.");
        return new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name,
            ParentGenreId = genre.ParentGenreId,
        };
    }

    public async Task<IEnumerable<GenreDto>> GetAllAsync()
    {
        logger.LogInformation("Fetching all genres.");
        var genres = (await unitOfWork.Genres.GetAllAsync()).ToList();
        logger.LogInformation("Fetched {Count} genres.", genres.Count);
        return genres.Select(genre => new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name,
            ParentGenreId = genre.ParentGenreId,
        });
    }

    public async Task<IEnumerable<GenreDto>> GetByGameKeyAsync(string gameKey)
    {
        logger.LogInformation("Fetching genres by game key: {GameKey}.", gameKey);
        var genres = (await unitOfWork.Genres.GetByGameKeyAsync(gameKey)).ToList();
        logger.LogInformation("Fetched {Count} genres.", genres.Count);
        return genres.Select(genre => new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name,
            ParentGenreId = genre.ParentGenreId,
        });
    }

    public async Task<IEnumerable<GenreDto>> GetByParentIdAsync(Guid parentId)
    {
        logger.LogInformation("Fetching genres by parent genre id: {ParentId}.", parentId);
        var genres = (await unitOfWork.Genres.GetByParentIdAsync(parentId)).ToList();
        logger.LogInformation("Fetched {Count} genres.", genres.Count);
        return genres.Select(genre => new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name,
            ParentGenreId = genre.ParentGenreId,
        });
    }

    public async Task CreateAsync(CreateGenreRequestDto request)
    {
        logger.LogInformation("Creating genre named: {Name}", request.Genre.Name);
        var genre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = request.Genre.Name,
            ParentGenreId = request.Genre.ParentGenreId,
        };

        await unitOfWork.Genres.AddAsync(genre);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Genre '{Name}' created successfully.", request.Genre.Name);
    }

    public async Task UpdateAsync(UpdateGenreRequestDto request)
    {
        logger.LogInformation("Updating genre with id: {Id}", request.Genre.Id);
        var genre = await unitOfWork.Genres.GetByIdAsync(request.Genre.Id)
                    ?? throw new KeyNotFoundException($"Genre with id {request.Genre.Id} not found");
        await VerifyNoCycleAsync(request.Genre.Id, request.Genre.ParentGenreId);
        genre.Name = request.Genre.Name;
        genre.ParentGenreId = request.Genre.ParentGenreId;

        unitOfWork.Genres.Update(genre);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Genre '{Id}' updated successfully.", request.Genre.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("Deleting genre with id: {Id}", id);
        var genre = await unitOfWork.Genres.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Genre with id {id} not found");
        unitOfWork.Genres.Delete(genre);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Genre with id: {Id} deleted successfully", id);
    }

    private async Task VerifyNoCycleAsync(Guid genreId, Guid? parentGenreId)
    {
        if (parentGenreId == null)
        {
            return;
        }

        if (genreId == parentGenreId)
        {
            throw new ArgumentException("A genre cannot be its own parent.");
        }

        var currentParentId = parentGenreId;
        while (currentParentId != null)
        {
            var parent = await unitOfWork.Genres.GetByIdAsync(currentParentId.Value);
            if (parent == null)
            {
                break;
            }

            if (parent.ParentGenreId == genreId)
            {
                throw new ArgumentException("Cyclic dependency detected in genres hierarchy.");
            }

            currentParentId = parent.ParentGenreId;
        }
    }
}