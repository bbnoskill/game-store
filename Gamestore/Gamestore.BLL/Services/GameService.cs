using System.Text.Json;
using System.Text.Json.Serialization;
using Gamestore.BLL.DTOs.Game;
using Gamestore.BLL.DTOs.Game.Filtering;
using Gamestore.BLL.Filtering;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Gamestore.BLL.Services;

public class GameService(
    IUnitOfWork unitOfWork,
    ILogger<GameService> logger,
    GameFilterPipeline pipeline,
    IMongoProductRepository mongoProductRepository,
    IMongoCategoryRepository? mongoCategoryRepository = null,
    IMongoSupplierRepository? mongoSupplierRepository = null,
    IMongoChangeLogRepository? mongoChangeLogRepository = null) : IGameService
{
    private static readonly JsonSerializerOptions LogJsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    public async Task<GameDto?> GetByKeyAsync(string key)
    {
        logger.LogInformation("Fetching game by key: {Key}", key);
        var decodedKey = Uri.UnescapeDataString(key);

        var sqlGame = await unitOfWork.Games.GetByKeyAsync(key)
            ?? await unitOfWork.Games.GetByKeyAsync(decodedKey);

        if (sqlGame != null)
        {
            sqlGame.ViewCount++;
            unitOfWork.Games.Update(sqlGame);
            await unitOfWork.SaveChangesAsync();
            return MapSqlToDto(sqlGame);
        }

        MongoProduct? mongoProduct = null;
        try
        {
            mongoProduct = await mongoProductRepository.GetByGameStoreKeyAsync(key)
                ?? await mongoProductRepository.GetByGameStoreKeyAsync(decodedKey);

            if (mongoProduct == null)
            {
                List<MongoProduct> allMongoProducts = [.. mongoProductRepository.GetAllQueryable()];
                mongoProduct = allMongoProducts.FirstOrDefault(p =>
                    (!string.IsNullOrEmpty(p.GameStoreKey) && (p.GameStoreKey.Equals(key, StringComparison.OrdinalIgnoreCase) || p.GameStoreKey.Equals(decodedKey, StringComparison.OrdinalIgnoreCase)))
                    || GenerateKey(p.ProductName).Equals(key, StringComparison.OrdinalIgnoreCase)
                    || GenerateKey(p.ProductName).Equals(decodedKey, StringComparison.OrdinalIgnoreCase));
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not fetch product from MongoDB by key '{Key}'", key);
        }

        if (mongoProduct != null)
        {
            await mongoProductRepository.IncrementViewsCountAsync(mongoProduct.Id);
            return MapMongoToDto(mongoProduct);
        }

        throw new KeyNotFoundException($"Game with key '{key}' was not found.");
    }

    public async Task<GameDto?> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Fetching game by id: {Id}", id);
        var game = await unitOfWork.Games.GetByIdAsync(id);
        if (game != null)
        {
            game.ViewCount++;
            unitOfWork.Games.Update(game);
            await unitOfWork.SaveChangesAsync();
            return MapSqlToDto(game);
        }

        throw new KeyNotFoundException($"Game with id '{id}' was not found.");
    }

    public async Task<IEnumerable<GameDto>> GetAllAsync()
    {
        logger.LogInformation("Fetching all games from SQL and MongoDB.");
        var sqlGames = (await unitOfWork.Games.GetAllAsync()).ToList();
        var sqlDtos = sqlGames.Select(MapSqlToDto).ToList();

        var sqlKeys = new HashSet<string>(sqlGames.Select(g => g.Key), StringComparer.OrdinalIgnoreCase);
        var sqlNames = new HashSet<string>(sqlGames.Select(g => g.Name), StringComparer.OrdinalIgnoreCase);

        List<MongoProduct> mongoProducts = FetchMongoProducts();

        var uniqueMongoDtos = mongoProducts
            .Where(p => (string.IsNullOrEmpty(p.GameStoreKey) || !sqlKeys.Contains(p.GameStoreKey))
                        && !sqlNames.Contains(p.ProductName))
            .GroupBy(p => p.ProductName, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .Select(MapMongoToDto);

        var result = sqlDtos.Concat(uniqueMongoDtos).ToList();
        logger.LogInformation("Fetched total {Count} games ({SqlCount} SQL, {MongoCount} Mongo).", result.Count, sqlDtos.Count, result.Count - sqlDtos.Count);
        return result;
    }

    public async Task<IEnumerable<GameDto>> GetByGenreAsync(Guid genreId)
    {
        logger.LogInformation("Fetching games by genre: {GenreId}.", genreId);
        var games = (await unitOfWork.Games.GetByGenreAsync(genreId)).ToList();
        logger.LogInformation("Fetched {Count} games.", games.Count);
        return games.Select(MapSqlToDto);
    }

    public async Task<IEnumerable<GameDto>> GetByPlatformAsync(Guid platformId)
    {
        logger.LogInformation("Fetching games by platform: {PlatformId}.", platformId);
        var games = (await unitOfWork.Games.GetByPlatformAsync(platformId)).ToList();
        logger.LogInformation("Fetched {Count} games.", games.Count);
        return games.Select(MapSqlToDto);
    }

    public async Task<IEnumerable<GameDto>> GetByPublisherAsync(string companyName)
    {
        logger.LogInformation("Fetching games by publisher: {PublisherName}.", companyName);
        var games = (await unitOfWork.Games.GetByPublisherAsync(companyName)).ToList();
        logger.LogInformation("Fetched {Count} games.", games.Count);
        return games.Select(MapSqlToDto);
    }

    public async Task CreateAsync(CreateGameRequestDto request)
    {
        logger.LogInformation("Creating game with key: {Key}", request.Game.Key);
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Name = request.Game.Name,
            Key = string.IsNullOrEmpty(request.Game.Key) ? GenerateKey(request.Game.Name) : request.Game.Key,
            Description = request.Game.Description,
            Price = request.Game.Price,
            Discount = request.Game.Discount,
            UnitInStock = request.Game.UnitInStock,
            PublisherId = request.Publisher,
        };

        game.GameGenres = [..request.Genres.Select(genreId => new GameGenre
        {
            GameId = game.Id,
            GenreId = genreId,
        })];

        game.GamePlatforms =
        [
            ..request.Platforms.Select(platformId => new GamePlatform
            {
                GameId = game.Id,
                PlatformId = platformId,
            })
        ];

        await ValidateRequest(request.Genres, request.Platforms, request.Publisher);
        await unitOfWork.Games.AddAsync(game);
        await unitOfWork.SaveChangesAsync();

        await LogChangeAsync("Create", newVersion: game);
        logger.LogInformation("Game '{Key}' created successfully.", request.Game.Key);
    }

    public async Task UpdateAsync(UpdateGameRequestDto request)
    {
        logger.LogInformation("Updating game with key: {Key}", request.Game.Key);
        var sqlGame = await unitOfWork.Games.GetByIdAsync(request.Game.Id);

        if (sqlGame != null)
        {
            var oldVersion = MapSqlToDto(sqlGame);

            sqlGame.Name = request.Game.Name;
            sqlGame.Key = string.IsNullOrWhiteSpace(request.Game.Key)
                ? GenerateKey(request.Game.Name)
                : request.Game.Key;
            sqlGame.Description = request.Game.Description;
            sqlGame.Price = request.Game.Price;
            sqlGame.Discount = request.Game.Discount;
            sqlGame.UnitInStock = request.Game.UnitInStock;
            sqlGame.PublisherId = request.Publisher;

            sqlGame.GameGenres =
            [
                ..request.Genres.Select(genreId => new GameGenre
                {
                    GameId = sqlGame.Id,
                    GenreId = genreId,
                })
            ];

            sqlGame.GamePlatforms =
            [
                ..request.Platforms.Select(platformId => new GamePlatform
                {
                    GameId = sqlGame.Id,
                    PlatformId = platformId,
                })
            ];

            await ValidateRequest(request.Genres, request.Platforms, request.Publisher);
            unitOfWork.Games.Update(sqlGame);
            await unitOfWork.SaveChangesAsync();

            await LogChangeAsync("Update", oldVersion, MapSqlToDto(sqlGame));
            logger.LogInformation("Game '{Key}' updated successfully in SQL DB.", request.Game.Key);
        }
        else
        {
            var newKey = string.IsNullOrWhiteSpace(request.Game.Key)
                ? GenerateKey(request.Game.Name)
                : request.Game.Key;

            var newGame = new Game
            {
                Id = request.Game.Id != Guid.Empty ? request.Game.Id : Guid.NewGuid(),
                Name = request.Game.Name,
                Key = newKey,
                Description = request.Game.Description,
                Price = request.Game.Price,
                Discount = request.Game.Discount,
                UnitInStock = request.Game.UnitInStock,
                PublisherId = request.Publisher,
            };

            newGame.GameGenres =
            [
                ..request.Genres.Select(genreId => new GameGenre
                {
                    GameId = newGame.Id,
                    GenreId = genreId,
                })
            ];

            newGame.GamePlatforms =
            [
                ..request.Platforms.Select(platformId => new GamePlatform
                {
                    GameId = newGame.Id,
                    PlatformId = platformId,
                })
            ];

            await ValidateRequest(request.Genres, request.Platforms, request.Publisher);
            await unitOfWork.Games.AddAsync(newGame);
            await unitOfWork.SaveChangesAsync();

            await LogChangeAsync("Create", newVersion: newGame);
            logger.LogInformation("Mongo game copied and created as new SQL Game '{Key}'.", newKey);
        }
    }

    public async Task DeleteAsync(string key)
    {
        logger.LogInformation("Deleting game with key: {Key}", key);
        var game = await unitOfWork.Games.GetByKeyAsync(key);
        if (game != null)
        {
            var oldVersion = MapSqlToDto(game);
            unitOfWork.Games.Delete(game);
            await unitOfWork.SaveChangesAsync();

            await LogChangeAsync("Delete", oldVersion: oldVersion);
            logger.LogInformation("Game '{Key}' deleted from SQL DB successfully.", key);
            return;
        }

        _ = await mongoProductRepository.GetByGameStoreKeyAsync(key)
            ?? mongoProductRepository.GetAllQueryable()
                .FirstOrDefault(p => (string.IsNullOrEmpty(p.GameStoreKey) ? GenerateKey(p.ProductName) : p.GameStoreKey) == key)
            ?? throw new KeyNotFoundException($"Game with key '{key}' not found");

        logger.LogInformation("Mongo product '{Key}' delete ignored (read-only per US7).", key);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await unitOfWork.Games.GetCountAsync();
    }

    public async Task<GameFilterResultDto> GetFilteredAsync(GameFilterDto filter)
    {
        logger.LogInformation("Fetching filtered games.");
        var (sqlDtos, sqlKeys, sqlNames) = await FetchSqlGamesAsync(filter);
        var mongoProducts = FetchMongoProducts();

        var mongoDtos = mongoProducts
            .Where(p => (string.IsNullOrEmpty(p.GameStoreKey) || !sqlKeys.Contains(p.GameStoreKey))
                        && !sqlNames.Contains(p.ProductName))
            .GroupBy(p => p.ProductName, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .Select(MapMongoToDto);

        mongoDtos = await FilterMongoDtosAsync(mongoDtos, mongoProducts, filter);

        var combined = SortGames([.. sqlDtos.Concat(mongoDtos)], filter.SortBy ?? filter.Sort);

        return PaginateGames(combined, filter.PageSize, filter.Page);
    }

    private async Task<(List<GameDto> SqlDtos, HashSet<string> SqlKeys, HashSet<string> SqlNames)> FetchSqlGamesAsync(GameFilterDto filter)
    {
        var sqlQuery = unitOfWork.Games.GetQueryable();
        sqlQuery = pipeline.Execute(sqlQuery, filter);
        var sqlGames = sqlQuery is IAsyncEnumerable<Game> ? await sqlQuery.ToListAsync() : [.. sqlQuery];
        List<GameDto> sqlDtos = [.. sqlGames.Select(MapSqlToDto)];

        var sqlKeys = new HashSet<string>(sqlGames.Select(g => g.Key), StringComparer.OrdinalIgnoreCase);
        var sqlNames = new HashSet<string>(sqlGames.Select(g => g.Name), StringComparer.OrdinalIgnoreCase);

        return (sqlDtos, sqlKeys, sqlNames);
    }

    private List<MongoProduct> FetchMongoProducts()
    {
        try
        {
            return [.. mongoProductRepository.GetAllQueryable()];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not fetch products from MongoDB.");
            return [];
        }
    }

    private async Task<IEnumerable<GameDto>> FilterMongoDtosAsync(
        IEnumerable<GameDto> mongoDtos,
        List<MongoProduct> mongoProducts,
        GameFilterDto filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            mongoDtos = mongoDtos.Where(g => g.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
        }

        if (filter.MinPrice.HasValue)
        {
            mongoDtos = mongoDtos.Where(g => g.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            mongoDtos = mongoDtos.Where(g => g.Price <= filter.MaxPrice.Value);
        }

        mongoDtos = await FilterMongoByGenresAsync(mongoDtos, mongoProducts, filter.Genres);
        mongoDtos = await FilterMongoByPublishersAsync(mongoDtos, mongoProducts, filter.Publishers);

        var activePlatformIds = filter.Platforms?.Where(p => p.HasValue).Select(p => p!.Value).ToList();
        return activePlatformIds != null && activePlatformIds.Count > 0 ? [] : mongoDtos;
    }

    private async Task<IEnumerable<GameDto>> FilterMongoByGenresAsync(
        IEnumerable<GameDto> mongoDtos,
        List<MongoProduct> mongoProducts,
        List<Guid?>? genres)
    {
        var activeGenreIds = genres?.Where(g => g.HasValue).Select(g => g!.Value).ToList();
        if (activeGenreIds == null || activeGenreIds.Count == 0 || unitOfWork.Genres == null || mongoCategoryRepository == null)
        {
            return mongoDtos;
        }

        var selectedGenreNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var genreId in activeGenreIds)
        {
            var genre = await unitOfWork.Genres.GetByIdAsync(genreId);
            if (genre != null)
            {
                selectedGenreNames.Add(genre.Name);
            }
        }

        var mongoCategories = mongoCategoryRepository.GetAllQueryable().ToList();
        var categoryIdToName = mongoCategories.ToDictionary(c => c.CategoryID, c => c.CategoryName);

        var matchingMongoProducts = mongoProducts
            .Where(p => p.CategoryID.HasValue && categoryIdToName.TryGetValue(p.CategoryID.Value, out var catName) && selectedGenreNames.Contains(catName))
            .Select(p => p.ProductName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return mongoDtos.Where(g => matchingMongoProducts.Contains(g.Name));
    }

    private async Task<IEnumerable<GameDto>> FilterMongoByPublishersAsync(
        IEnumerable<GameDto> mongoDtos,
        List<MongoProduct> mongoProducts,
        List<Guid?>? publishers)
    {
        var activePublisherIds = publishers?.Where(p => p.HasValue).Select(p => p!.Value).ToList();
        if (activePublisherIds == null || activePublisherIds.Count == 0 || unitOfWork.Publishers == null || mongoSupplierRepository == null)
        {
            return mongoDtos;
        }

        var selectedPublisherNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pubId in activePublisherIds)
        {
            var pub = await unitOfWork.Publishers.GetByIdAsync(pubId);
            if (pub != null)
            {
                selectedPublisherNames.Add(pub.CompanyName);
            }
        }

        var mongoSuppliers = mongoSupplierRepository.GetAllQueryable().ToList();
        var supplierIdToName = mongoSuppliers.ToDictionary(s => s.SupplierID, s => s.CompanyName);

        var matchingMongoProducts = mongoProducts
            .Where(p => p.SupplierID.HasValue && supplierIdToName.TryGetValue(p.SupplierID.Value, out var suppName) && selectedPublisherNames.Contains(suppName))
            .Select(p => p.ProductName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return mongoDtos.Where(g => matchingMongoProducts.Contains(g.Name));
    }

    private static List<GameDto> SortGames(List<GameDto> games, string? sortOption)
    {
        return sortOption switch
        {
            "Price ASC" or "price asc" => [.. games.OrderBy(g => g.Price)],
            "Price DESC" or "price desc" => [.. games.OrderByDescending(g => g.Price)],
            "Most popular" or "most popular" => [.. games.OrderByDescending(g => g.Name)],
            "Most commented" or "most commented" => [.. games.OrderByDescending(g => g.Name)],
            "New" or "new" => [.. games.OrderByDescending(g => g.Name)],
            _ => [.. games.OrderBy(g => g.Name)],
        };
    }

    private GameFilterResultDto PaginateGames(List<GameDto> combined, string? pageSizeRaw, int pageRaw)
    {
        int totalCount = combined.Count;
        int pageSize = pageSizeRaw == "all" || string.IsNullOrEmpty(pageSizeRaw)
            ? totalCount
            : int.Parse(pageSizeRaw);
        int page = pageRaw > 0 ? pageRaw : 1;
        int totalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 1;

        List<GameDto> pagedGames = [.. combined
            .Skip((page - 1) * pageSize)
            .Take(pageSize == 0 ? totalCount : pageSize)];

        logger.LogInformation("Fetched {Count} of {Total} games (page {Page}).", pagedGames.Count, totalCount, page);
        return new GameFilterResultDto
        {
            Games = pagedGames,
            TotalPages = totalPages,
            CurrentPage = page,
        };
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

    private static GameDto MapSqlToDto(Game game)
    {
        return new GameDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
            Price = game.Price,
            Discount = game.Discount,
            UnitInStock = game.UnitInStock,
        };
    }

    private static GameDto MapMongoToDto(MongoProduct product)
    {
        return new GameDto
        {
            Id = Guid.Empty,
            Name = product.ProductName,
            Key = !string.IsNullOrEmpty(product.GameStoreKey) ? product.GameStoreKey : GenerateKey(product.ProductName),
            Description = product.QuantityPerUnit,
            Price = (double)(product.UnitPrice ?? 0m),
            Discount = 0,
            UnitInStock = product.UnitsInStock ?? 0,
        };
    }

    private static string GenerateKey(string name)
    {
        return name.ToLower(System.Globalization.CultureInfo.CurrentCulture).Replace(" ", "-");
    }

    private async Task ValidateRequest(List<Guid> genreIds, List<Guid> platformIds, Guid publisherId)
    {
        if (publisherId != Guid.Empty && unitOfWork.Publishers != null)
        {
            _ = await unitOfWork.Publishers.GetByIdAsync(publisherId)
                ?? throw new ArgumentException($"Publisher with ID '{publisherId}' not found.");
        }

        if (genreIds != null && unitOfWork.Genres != null)
        {
            foreach (var genreId in genreIds)
            {
                _ = await unitOfWork.Genres.GetByIdAsync(genreId)
                    ?? throw new ArgumentException($"Genre with ID '{genreId}' not found.");
            }
        }

        if (platformIds != null && unitOfWork.Platforms != null)
        {
            foreach (var platformId in platformIds)
            {
                _ = await unitOfWork.Platforms.GetByIdAsync(platformId)
                    ?? throw new ArgumentException($"Platform with ID '{platformId}' not found.");
            }
        }
    }
}