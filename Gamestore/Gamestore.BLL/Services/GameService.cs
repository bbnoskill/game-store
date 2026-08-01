using Gamestore.BLL.DTOs.Game;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Gamestore.BLL.Services;

public class GameService(IUnitOfWork unitOfWork, ILogger<GameService> logger) : IGameService
{
    public async Task<GameDto?> GetByKeyAsync(string key)
    {
        logger.LogInformation("Fetching game by key: {Key}", key);
        var game = await unitOfWork.Games.GetByKeyAsync(key)
                   ?? throw new KeyNotFoundException($"Game with key '{key}' was not found.");
        game.ViewCount++;
        unitOfWork.Games.Update(game);
        await unitOfWork.SaveChangesAsync();
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

    public async Task<GameDto?> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Fetching game by id: {Id}", id);
        var game = await unitOfWork.Games.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException($"Game with id '{id}' was not found.");
        game.ViewCount++;
        unitOfWork.Games.Update(game);
        await unitOfWork.SaveChangesAsync();
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

    public async Task<IEnumerable<GameDto>> GetAllAsync()
    {
        logger.LogInformation("Fetching all games.");
        var games = (await unitOfWork.Games.GetAllAsync()).ToList();
        logger.LogInformation("Fetched {Count} games.", games.Count);
        return games.Select(game => new GameDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
            Price = game.Price,
            Discount = game.Discount,
            UnitInStock = game.UnitInStock,
        });
    }

    public async Task<IEnumerable<GameDto>> GetByGenreAsync(Guid genreId)
    {
        logger.LogInformation("Fetching games by genre: {GenreId}.", genreId);
        var games = (await unitOfWork.Games.GetByGenreAsync(genreId)).ToList();
        logger.LogInformation("Fetched {Count} games.", games.Count);
        return games.Select(game => new GameDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
            Price = game.Price,
            Discount = game.Discount,
            UnitInStock = game.UnitInStock,
        });
    }

    public async Task<IEnumerable<GameDto>> GetByPlatformAsync(Guid platformId)
    {
        logger.LogInformation("Fetching games by platform: {PlatformId}.", platformId);
        var games = (await unitOfWork.Games.GetByPlatformAsync(platformId)).ToList();
        logger.LogInformation("Fetched {Count} games.", games.Count);
        return games.Select(game => new GameDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
            Price = game.Price,
            Discount = game.Discount,
            UnitInStock = game.UnitInStock,
        });
    }

    public async Task<IEnumerable<GameDto>> GetByPublisherAsync(string companyName)
    {
        logger.LogInformation("Fetching games by publisher: {PublisherName}.", companyName);
        var games = (await unitOfWork.Games.GetByPublisherAsync(companyName)).ToList();
        logger.LogInformation("Fetched {Count} games.", games.Count);
        return games.Select(game => new GameDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
            Price = game.Price,
            Discount = game.Discount,
            UnitInStock = game.UnitInStock,
        });
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
        var game = await unitOfWork.Games.GetByIdAsync(request.Game.Id)
                   ?? throw new KeyNotFoundException($"Game with id {request.Game.Id} not found");

        game.Name = request.Game.Name;
        game.Key = string.IsNullOrWhiteSpace(request.Game.Key)
            ? GenerateKey(request.Game.Name)
            : request.Game.Key;
        game.Description = request.Game.Description;
        game.Price = request.Game.Price;
        game.Discount = request.Game.Discount;
        game.UnitInStock = request.Game.UnitInStock;

        game.GameGenres =
        [
            ..request.Genres.Select(genreId => new GameGenre
            {
                GameId = game.Id,
                GenreId = genreId,
            })
        ];

        game.GamePlatforms =
        [
            ..request.Platforms.Select(platformId => new GamePlatform
            {
                GameId = game.Id,
                PlatformId = platformId,
            })
        ];

        await ValidateRequest(request.Genres, request.Platforms, request.Publisher);
        unitOfWork.Games.Update(game);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Game '{Key}' updated successfully.", request.Game.Key);
    }

    public async Task DeleteAsync(string key)
    {
        logger.LogInformation("Deleting game with key: {Key}", key);
        var game = await unitOfWork.Games.GetByKeyAsync(key)
            ?? throw new KeyNotFoundException($"Game with key {key} not found");
        unitOfWork.Games.Delete(game);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Game '{Key}' deleted successfully.", key);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await unitOfWork.Games.GetCountAsync();
    }

    private static string GenerateKey(string name)
    {
        return name.ToLower(System.Globalization.CultureInfo.CurrentCulture).Replace(" ", "-");
    }

    private async Task ValidateRequest(List<Guid> genreIds, List<Guid> platformIds, Guid publisherId)
    {
        _ = await unitOfWork.Publishers.GetByIdAsync(publisherId)
           ?? throw new ArgumentException($"Publisher with ID '{publisherId}' not found.");

        foreach (var genreId in genreIds)
        {
            _ = await unitOfWork.Genres.GetByIdAsync(genreId) ??
                throw new ArgumentException($"Genre with ID '{genreId}' not found.");
        }

        foreach (var platformId in platformIds)
        {
            _ = await unitOfWork.Platforms.GetByIdAsync(platformId) ??
                throw new ArgumentException($"Platform with ID '{platformId}' not found.");
        }
    }
}