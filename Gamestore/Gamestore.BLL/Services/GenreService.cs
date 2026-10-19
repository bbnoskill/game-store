using System.Text.Json;
using System.Text.Json.Serialization;
using Gamestore.BLL.DTOs.Genre;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Gamestore.BLL.Services;

public class GenreService(
    IUnitOfWork unitOfWork,
    ILogger<GameService> logger,
    IMongoCategoryRepository mongoCategoryRepository,
    IMongoChangeLogRepository? mongoChangeLogRepository = null) : IGenreService
{
    private static readonly JsonSerializerOptions LogJsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    public async Task<GenreDto?> GetByIdAsync(Guid id, string languageCode = "en", bool includeMongo = true)
    {
        logger.LogInformation("Fetching genre by id: {Id}", id);
        var genre = await unitOfWork.Genres.GetByIdAsync(id);
        if (genre != null)
        {
            return MapSqlToDto(genre, languageCode);
        }

        if (!includeMongo)
        {
            throw new KeyNotFoundException($"Genre with id '{id}' was not found!");
        }

        try
        {
            var mongoCategory = mongoCategoryRepository.GetAllQueryable().FirstOrDefault(c => c.Id == id.ToString());
            if (mongoCategory != null)
            {
                return MapMongoToDto(mongoCategory);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "MongoDB query failed when searching genre by id {Id}", id);
        }

        throw new KeyNotFoundException($"Genre with id '{id}' was not found!");
    }

    public async Task<IEnumerable<GenreDto>> GetAllAsync(string languageCode = "en", bool includeMongo = true)
    {
        logger.LogInformation("Fetching all genres from SQL and MongoDB.");
        var sqlGenres = (await unitOfWork.Genres.GetAllAsync()).ToList();
        var sqlDtos = sqlGenres.Select(entity => MapSqlToDto(entity, languageCode)).ToList();
        if (!includeMongo)
        {
            return sqlDtos;
        }

        var sqlNames = new HashSet<string>(sqlGenres.Select(g => g.Name), StringComparer.OrdinalIgnoreCase);

        List<MongoCategory> mongoCategories = [];
        try
        {
            mongoCategories = [.. mongoCategoryRepository.GetAllQueryable()];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not fetch genres from MongoDB. Falling back to SQL genres only.");
        }

        var uniqueMongoDtos = mongoCategories
            .Where(c => !string.IsNullOrEmpty(c.CategoryName) && !sqlNames.Contains(c.CategoryName))
            .GroupBy(c => c.CategoryName, StringComparer.OrdinalIgnoreCase)
            .Select(c => c.First())
            .Select(MapMongoToDto);

        var result = sqlDtos.Concat(uniqueMongoDtos).ToList();
        logger.LogInformation("Fetched total {Count} genres ({SqlCount} SQL, {MongoCount} Mongo).", result.Count, sqlDtos.Count, result.Count - sqlDtos.Count);
        return result;
    }

    public async Task<IEnumerable<GenreDto>> GetByGameKeyAsync(string gameKey, string languageCode = "en")
    {
        logger.LogInformation("Fetching genres by game key: {GameKey}.", gameKey);
        var genres = (await unitOfWork.Genres.GetByGameKeyAsync(gameKey)).ToList();
        logger.LogInformation("Fetched {Count} genres.", genres.Count);
        return genres.Select(entity => MapSqlToDto(entity, languageCode));
    }

    public async Task<IEnumerable<GenreDto>> GetByParentIdAsync(Guid parentId, string languageCode = "en")
    {
        logger.LogInformation("Fetching genres by parent genre id: {ParentId}.", parentId);
        var genres = (await unitOfWork.Genres.GetByParentIdAsync(parentId)).ToList();
        logger.LogInformation("Fetched {Count} genres.", genres.Count);
        return genres.Select(entity => MapSqlToDto(entity, languageCode));
    }

    public async Task CreateAsync(CreateGenreRequestDto request, string languageCode = "en")
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

        EntityLocalization.Update(genre, languageCode, request.Genre.Name);

        await unitOfWork.Genres.AddAsync(genre);
        await unitOfWork.SaveChangesAsync();

        await LogChangeAsync("Create", newVersion: genre);
        logger.LogInformation("Genre '{Name}' created successfully in SQL DB.", request.Genre.Name);
    }

    public async Task UpdateAsync(UpdateGenreRequestDto request, string languageCode = "en")
    {
        logger.LogInformation("Updating genre with id: {Id}", request.Genre.Id);
        var sqlGenre = await unitOfWork.Genres.GetByIdAsync(request.Genre.Id);

        if (sqlGenre != null)
        {
            var oldVersion = MapSqlToDto(sqlGenre);

            await VerifyNoCycleAsync(request.Genre.Id, request.Genre.ParentGenreId);
            EntityLocalization.Update(sqlGenre, languageCode, request.Genre.Name);
            sqlGenre.ParentGenreId = request.Genre.ParentGenreId;

            unitOfWork.Genres.Update(sqlGenre);
            await unitOfWork.SaveChangesAsync();

            await LogChangeAsync("Update", oldVersion, MapSqlToDto(sqlGenre));
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
            EntityLocalization.Update(newGenre, languageCode, request.Genre.Name);

            await unitOfWork.Genres.AddAsync(newGenre);
            await unitOfWork.SaveChangesAsync();

            await LogChangeAsync("Create", newVersion: newGenre);
            logger.LogInformation("Mongo category copied and created as new SQL Genre '{Name}'.", request.Genre.Name);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("Deleting genre with id: {Id}", id);
        var genre = await unitOfWork.Genres.GetByIdAsync(id);
        if (genre != null)
        {
            var oldVersion = MapSqlToDto(genre);
            unitOfWork.Genres.Delete(genre);
            await unitOfWork.SaveChangesAsync();

            await LogChangeAsync("Delete", oldVersion: oldVersion);
            logger.LogInformation("Genre with id: {Id} deleted successfully from SQL DB.", id);
            return;
        }

        try
        {
            var mongoCategory = mongoCategoryRepository.GetAllQueryable().FirstOrDefault(c => c.Id == id.ToString());
            if (mongoCategory != null)
            {
                logger.LogInformation("Mongo category '{Id}' delete ignored (read-only per US7).", id);
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "MongoDB query failed when checking genre by id {Id}", id);
        }

        throw new KeyNotFoundException($"Genre with id {id} not found");
    }

    private async Task LogChangeAsync<T>(string action, T? oldVersion = default, T? newVersion = default)
    {
        if (mongoChangeLogRepository != null)
        {
            try
            {
                await mongoChangeLogRepository.InsertLogRawAsync(new MongoChangeLog
                {
                    DateTime = DateTime.UtcNow,
                    Action = action,
                    EntityType = typeof(T).Name,
                    OldVersion = oldVersion != null ? ToLogDocument(oldVersion) : null,
                    NewVersion = newVersion != null ? ToLogDocument(newVersion) : null,
                });
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to write change log into MongoDB.");
            }
        }
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

    private static BsonDocument ToLogDocument<T>(T entity)
    {
        var json = JsonSerializer.Serialize(entity, LogJsonOptions);
        return BsonDocument.Parse(json);
    }

    private static GenreDto MapSqlToDto(Genre genre, string languageCode = "en")
    {
        return new GenreDto
        {
            Id = genre.Id,
            Name = genre.Localizations.FirstOrDefault(l => l.LanguageCode == languageCode)?.Name
                ?? genre.Localizations.FirstOrDefault(l => l.LanguageCode == "en")?.Name
                ?? genre.Name,
            ParentGenreId = genre.ParentGenreId,
        };
    }

    private static GenreDto MapMongoToDto(MongoCategory category)
    {
        return new GenreDto
        {
            Id = Guid.Empty,
            Name = category.CategoryName ?? string.Empty,
            ParentGenreId = Guid.Empty,
        };
    }
}