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

namespace Gamestore.BLL.Services;

public class GameService(
    IUnitOfWork unitOfWork,
    ILogger<GameService> logger,
    GameFilterPipeline pipeline,
    IMongoProductRepository mongoProductRepository) : IGameService
{
    public async Task<GameDto?> GetByKeyAsync(string key)
    {
        logger.LogInformation("Fetching game by key: {Key}", key);

        var sqlGame = await unitOfWork.Games.GetByKeyAsync(key);
        if (sqlGame != null)
        {
            sqlGame.ViewCount++;
            unitOfWork.Games.Update(sqlGame);
            await unitOfWork.SaveChangesAsync();
            return MapSqlToDto(sqlGame);
        }

        var mongoProduct = await mongoProductRepository.GetByGameStoreKeyAsync(key)
            ?? mongoProductRepository.GetAllQueryable()
                .FirstOrDefault(p => (string.IsNullOrEmpty(p.GameStoreKey) ? GenerateKey(p.ProductName) : p.GameStoreKey) == key);

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

        var mongoProducts = mongoProductRepository.GetAllQueryable().ToList();

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
        logger.LogInformation("Game '{Key}' created successfully.", request.Game.Key);
    }

    public async Task UpdateAsync(UpdateGameRequestDto request)
    {
        logger.LogInformation("Updating game with key: {Key}", request.Game.Key);
        var sqlGame = await unitOfWork.Games.GetByIdAsync(request.Game.Id);

        if (sqlGame != null)
        {
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
            logger.LogInformation("Mongo game copied and created as new SQL Game '{Key}'.", newKey);
        }
    }

    public async Task DeleteAsync(string key)
    {
        logger.LogInformation("Deleting game with key: {Key}", key);
        var game = await unitOfWork.Games.GetByKeyAsync(key);
        if (game != null)
        {
            unitOfWork.Games.Delete(game);
            await unitOfWork.SaveChangesAsync();
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
        var query = unitOfWork.Games.GetQueryable();
        query = pipeline.Execute(query, filter);
        var totalCount = query is IAsyncEnumerable<Game> ? await query.CountAsync() : query.Count();
        int pageSize = filter.PageSize == "all" || string.IsNullOrEmpty(filter.PageSize)
            ? totalCount
            : int.Parse(filter.PageSize);
        int page = filter.Page > 0 ? filter.Page : 1;
        int totalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 1;
        var pagedQuery = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize == 0 ? totalCount : pageSize)
            .Select(game => MapSqlToDto(game));
        var games = pagedQuery is IAsyncEnumerable<GameDto>
            ? await pagedQuery.ToListAsync()
            : [.. pagedQuery];
        logger.LogInformation("Fetched {Count} of {Total} games (page {Page}).", games.Count, totalCount, page);
        return new GameFilterResultDto
        {
            Games = games,
            TotalPages = totalPages,
            CurrentPage = page,
        };
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