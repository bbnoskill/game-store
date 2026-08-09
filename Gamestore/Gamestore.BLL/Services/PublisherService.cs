using System.Text.Json;
using System.Text.Json.Serialization;
using Gamestore.BLL.DTOs.Publisher;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Gamestore.BLL.Services;

public class PublisherService(
    IUnitOfWork unitOfWork,
    ILogger<GameService> logger,
    IMongoSupplierRepository mongoSupplierRepository,
    IMongoChangeLogRepository? mongoChangeLogRepository = null) : IPublisherService
{
    private static readonly JsonSerializerOptions LogJsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    public async Task<PublisherDto?> GetByNameAsync(string companyName)
    {
        logger.LogInformation("Fetching publisher by name: {CompanyName}", companyName);
        var publisher = await unitOfWork.Publishers.GetByCompanyNameAsync(companyName);
        if (publisher != null)
        {
            return MapSqlToDto(publisher);
        }

        try
        {
            var mongoSupplier = await mongoSupplierRepository.GetByCompanyNameAsync(companyName);
            if (mongoSupplier != null)
            {
                return MapMongoToDto(mongoSupplier);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "MongoDB query failed when searching publisher by company name '{Name}'", companyName);
        }

        throw new KeyNotFoundException($"Publisher with company name '{companyName}' was not found.");
    }

    public async Task<PublisherDto?> GetByGameKeyAsync(string gameKey)
    {
        logger.LogInformation("Fetching publisher by game key: {GameKey}", gameKey);
        var publisher = await unitOfWork.Publishers.GetByGameKeyAsync(gameKey);
        return publisher != null ? MapSqlToDto(publisher) : null;
    }

    public async Task<IEnumerable<PublisherDto>> GetAllAsync()
    {
        logger.LogInformation("Fetching all publishers from SQL and MongoDB.");
        var sqlPublishers = (await unitOfWork.Publishers.GetAllAsync()).ToList();
        var sqlDtos = sqlPublishers.Select(MapSqlToDto).ToList();

        var sqlCompanyNames = new HashSet<string>(sqlPublishers.Select(p => p.CompanyName), StringComparer.OrdinalIgnoreCase);

        List<MongoSupplier> mongoSuppliers = [];
        try
        {
            mongoSuppliers = [.. mongoSupplierRepository.GetAllQueryable()];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not fetch suppliers from MongoDB. Falling back to SQL publishers only.");
        }

        var uniqueMongoDtos = mongoSuppliers
            .Where(s => !string.IsNullOrEmpty(s.CompanyName) && !sqlCompanyNames.Contains(s.CompanyName))
            .GroupBy(s => s.CompanyName, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .Select(MapMongoToDto);

        var result = sqlDtos.Concat(uniqueMongoDtos).ToList();
        logger.LogInformation("Fetched total {Count} publishers ({SqlCount} SQL, {MongoCount} Mongo).", result.Count, sqlDtos.Count, result.Count - sqlDtos.Count);
        return result;
    }

    public async Task CreateAsync(CreatePublisherRequestDto request)
    {
        logger.LogInformation("Creating publisher: {CompanyName}", request.Publisher.CompanyName);
        var publisher = new Publisher
        {
            Id = Guid.NewGuid(),
            CompanyName = request.Publisher.CompanyName,
            Description = request.Publisher.Description,
            HomePage = request.Publisher.HomePage,
        };

        await unitOfWork.Publishers.AddAsync(publisher);
        await unitOfWork.SaveChangesAsync();

        await LogChangeAsync("Create", newVersion: publisher);
        logger.LogInformation("Created publisher in SQL DB: {CompanyName}", publisher.CompanyName);
    }

    public async Task UpdateAsync(UpdatePublisherRequestDto request)
    {
        logger.LogInformation("Updating publisher: {Id}", request.Publisher.Id);
        var publisher = await unitOfWork.Publishers.GetByIdAsync(request.Publisher.Id);

        if (publisher != null)
        {
            var oldVersion = MapSqlToDto(publisher);

            publisher.CompanyName = request.Publisher.CompanyName;
            publisher.Description = request.Publisher.Description;
            publisher.HomePage = request.Publisher.HomePage;

            unitOfWork.Publishers.Update(publisher);
            await unitOfWork.SaveChangesAsync();

            await LogChangeAsync("Update", oldVersion, MapSqlToDto(publisher));
            logger.LogInformation("Updated publisher in SQL DB: {Id}", publisher.Id);
        }
        else
        {
            var newPublisher = new Publisher
            {
                Id = request.Publisher.Id != Guid.Empty ? request.Publisher.Id : Guid.NewGuid(),
                CompanyName = request.Publisher.CompanyName,
                Description = request.Publisher.Description,
                HomePage = request.Publisher.HomePage,
            };

            await unitOfWork.Publishers.AddAsync(newPublisher);
            await unitOfWork.SaveChangesAsync();

            await LogChangeAsync("Create", newVersion: newPublisher);
            logger.LogInformation("Mongo supplier copied and created as new SQL Publisher '{Name}'.", request.Publisher.CompanyName);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("Deleting publisher with id: {Id}", id);
        var publisher = await unitOfWork.Publishers.GetByIdAsync(id);
        if (publisher != null)
        {
            var oldVersion = MapSqlToDto(publisher);
            unitOfWork.Publishers.Delete(publisher);
            await unitOfWork.SaveChangesAsync();

            await LogChangeAsync("Delete", oldVersion: oldVersion);
            logger.LogInformation("Deleted publisher from SQL DB: {Id}", id);
            return;
        }

        try
        {
            var mongoSupplier = mongoSupplierRepository.GetAllQueryable().FirstOrDefault(s => s.Id == id.ToString());
            if (mongoSupplier != null)
            {
                logger.LogInformation("Mongo supplier '{Id}' delete ignored (read-only per US7).", id);
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "MongoDB query failed when checking supplier by id {Id}", id);
        }

        throw new KeyNotFoundException($"Publisher with id {id} not found");
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

    private static BsonDocument ToLogDocument<T>(T entity)
    {
        var json = JsonSerializer.Serialize(entity, LogJsonOptions);
        return BsonDocument.Parse(json);
    }

    private static PublisherDto MapSqlToDto(Publisher publisher)
    {
        return new PublisherDto
        {
            Id = publisher.Id,
            CompanyName = publisher.CompanyName,
            Description = publisher.Description,
            HomePage = publisher.HomePage,
        };
    }

    private static PublisherDto MapMongoToDto(MongoSupplier supplier)
    {
        return new PublisherDto
        {
            Id = Guid.Empty,
            CompanyName = supplier.CompanyName ?? string.Empty,
            Description = $"{supplier.ContactName} ({supplier.ContactTitle}), {supplier.City}, {supplier.Country}",
            HomePage = supplier.HomePage,
        };
    }
}