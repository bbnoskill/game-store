using Gamestore.BLL.DTOs.Genre;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Gamestore.BLL.Services;

public class GenreService(
    IUnitOfWork unitOfWork,
    ILogger<GameService> logger,
    IMongoCategoryRepository mongoCategoryRepository) : IGenreService
{
    public async Task<GenreDto?> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Fetching genre by id: {Id}", id);
        var genre = await unitOfWork.Genres.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Genre with id '{id}' was not found!");
        return MapSqlToDto(genre);
    }

    public async Task<IEnumerable<GenreDto>> GetAllAsync()
    {
        logger.LogInformation("Fetching all genres from SQL and MongoDB.");
        var sqlGenres = (await unitOfWork.Genres.GetAllAsync()).ToList();
        var sqlDtos = sqlGenres.Select(MapSqlToDto).ToList();

        var sqlNames = new HashSet<string>(sqlGenres.Select(g => g.Name), StringComparer.OrdinalIgnoreCase);

        var mongoCategories = mongoCategoryRepository.GetAllQueryable().ToList();

        var uniqueMongoDtos = mongoCategories
            .Where(c => !sqlNames.Contains(c.CategoryName))
            .GroupBy(c => c.CategoryName, StringComparer.OrdinalIgnoreCase)
            .Select(c => c.First())
            .Select(MapMongoToDto);

        var result = sqlDtos.Concat(uniqueMongoDtos).ToList();
        logger.LogInformation("Fetched total {Count} genres ({SqlCount} SQL, {MongoCount} Mongo).", result.Count, sqlDtos.Count, result.Count - sqlDtos.Count);
        return result;
    }

    public async Task<IEnumerable<GenreDto>> GetByGameKeyAsync(string gameKey)
    {
        logger.LogInformation("Fetching genres by game key: {GameKey}.", gameKey);
        var genres = (await unitOfWork.Genres.GetByGameKeyAsync(gameKey)).ToList();
        logger.LogInformation("Fetched {Count} genres.", genres.Count);
        return genres.Select(MapSqlToDto);
    }

    public async Task<IEnumerable<GenreDto>> GetByParentIdAsync(Guid parentId)
    {
        logger.LogInformation("Fetching genres by parent genre id: {ParentId}.", parentId);
        var genres = (await unitOfWork.Genres.GetByParentIdAsync(parentId)).ToList();
        logger.LogInformation("Fetched {Count} genres.", genres.Count);
        return genres.Select(MapSqlToDto);
    }

    public async Task CreateAsync(CreateGenreRequestDto request)
    {
        logger.LogInformation("Creating genre named: {Name}", request.Genre.Name);

        var newId = Guid.NewGuid();
        await VerifyNoCycleAsync(newId, request.Genre.ParentGenreId);

        var genre = new Genre
        {
            Id = newId,
            Name = request.Genre.Name,
            ParentGenreId = request.Genre.ParentGenreId,
        };

        await unitOfWork.Genres.AddAsync(genre);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Genre '{Name}' created successfully in SQL DB.", request.Genre.Name);
    }

    public async Task UpdateAsync(UpdateGenreRequestDto request)
    {
        logger.LogInformation("Updating genre with id: {Id}", request.Genre.Id);
        var sqlGenre = await unitOfWork.Genres.GetByIdAsync(request.Genre.Id);

        if (sqlGenre != null)
        {
            await VerifyNoCycleAsync(request.Genre.Id, request.Genre.ParentGenreId);
            sqlGenre.Name = request.Genre.Name;
            sqlGenre.ParentGenreId = request.Genre.ParentGenreId;

            unitOfWork.Genres.Update(sqlGenre);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Genre '{Id}' updated successfully in SQL DB.", request.Genre.Id);
        }
        else
        {
            var newGenre = new Genre
            {
                Id = request.Genre.Id != Guid.Empty ? request.Genre.Id : Guid.NewGuid(),
                Name = request.Genre.Name,
                ParentGenreId = request.Genre.ParentGenreId,
            };

            await VerifyNoCycleAsync(newGenre.Id, newGenre.ParentGenreId);
            await unitOfWork.Genres.AddAsync(newGenre);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Mongo category copied and created as new SQL Genre '{Name}'.", request.Genre.Name);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("Deleting genre with id: {Id}", id);
        var genre = await unitOfWork.Genres.GetByIdAsync(id);
        if (genre != null)
        {
            unitOfWork.Genres.Delete(genre);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Genre with id: {Id} deleted successfully from SQL DB.", id);
            return;
        }

        _ = mongoCategoryRepository.GetAllQueryable().FirstOrDefault(c => c.Id == id.ToString())
            ?? throw new KeyNotFoundException($"Genre with id {id} not found");

        logger.LogInformation("Mongo category '{Id}' delete ignored (read-only per US7).", id);
    }

    private Task VerifyNoCycleAsync(Guid genreId, Guid? parentGenreId)
    {
        return parentGenreId switch
        {
            null => Task.CompletedTask,
            _ when genreId == parentGenreId => throw new ArgumentException("A genre cannot be its own parent."),
            _ => VerifyNoCycleInternalAsync(genreId, parentGenreId),
        };
    }

    private async Task VerifyNoCycleInternalAsync(Guid genreId, Guid? parentGenreId)
    {
        var currentParentId = parentGenreId;
        while (currentParentId != null)
        {
            var parent = await unitOfWork.Genres.GetByIdAsync(currentParentId.Value);
            if (parent == null)
            {
                if (currentParentId == parentGenreId)
                {
                    throw new ArgumentException($"Parent genre with ID '{parentGenreId}' does not exist.");
                }

                break;
            }

            if (parent.ParentGenreId == genreId)
            {
                throw new ArgumentException("Cyclic dependency detected in genres hierarchy.");
            }

            currentParentId = parent.ParentGenreId;
        }
    }

    private static GenreDto MapSqlToDto(Genre genre)
    {
        return new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name,
            ParentGenreId = genre.ParentGenreId,
        };
    }

    private static GenreDto MapMongoToDto(MongoCategory category)
    {
        return new GenreDto
        {
            Id = Guid.Empty,
            Name = category.CategoryName,
            ParentGenreId = Guid.Empty,
        };
    }
}