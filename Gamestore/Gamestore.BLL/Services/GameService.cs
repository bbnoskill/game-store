using Gamestore.BLL.DTOs.Game;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;

namespace Gamestore.BLL.Services;

public class GameService(IUnitOfWork unitOfWork) : IGameService
{
    public async Task<GameDto?> GetByKeyAsync(string key)
    {
        var game = await unitOfWork.Games.GetByKeyAsync(key);
        return game == null ? null : new GameDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
        };
    }

    public async Task<GameDto?> GetByIdAsync(Guid id)
    {
        var game = await unitOfWork.Games.GetByIdAsync(id);
        return game == null ? null : new GameDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
        };
    }

    public async Task<IEnumerable<GameDto>> GetAllAsync()
    {
        var games = await unitOfWork.Games.GetAllAsync();
        return games.Select(game => new GameDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
        });
    }

    public async Task<IEnumerable<GameDto>> GetByGenreAsync(Guid genreId)
    {
        var games = await unitOfWork.Games.GetByGenreAsync(genreId);
        return games.Select(game => new GameDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
        });
    }

    public async Task<IEnumerable<GameDto>> GetByPlatformAsync(Guid platformId)
    {
        var games = await unitOfWork.Games.GetByPlatformAsync(platformId);

        return games.Select(game => new GameDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
        });
    }

    public async Task CreateAsync(CreateGameRequestDto request)
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Name = request.Game.Name,
            Key = string.IsNullOrEmpty(request.Game.Key) ? GenerateKey(request.Game.Name) : request.Game.Key,
            Description = request.Game.Description,
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

        await unitOfWork.Games.AddAsync(game);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateGameRequestDto request)
    {
        var game = await unitOfWork.Games.GetByIdAsync(request.Game.Id)
                   ?? throw new KeyNotFoundException($"Game with id {request.Game.Id} not found");

        game.Name = request.Game.Name;
        game.Key = string.IsNullOrWhiteSpace(request.Game.Key)
            ? GenerateKey(request.Game.Name)
            : request.Game.Key;
        game.Description = request.Game.Description;

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

        unitOfWork.Games.Update(game);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(string key)
    {
        var game = await unitOfWork.Games.GetByKeyAsync(key)
            ?? throw new KeyNotFoundException($"Game with key {key} not found");
        unitOfWork.Games.Delete(game);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await unitOfWork.Games.GetCountAsync();
    }

    private static string GenerateKey(string name)
    {
        return name.ToLower(System.Globalization.CultureInfo.CurrentCulture).Replace(" ", "-");
    }
}