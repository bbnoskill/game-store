using Gamestore.BLL.DTOs.Genre;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;

namespace Gamestore.BLL.Services;

public class GenreService(IUnitOfWork unitOfWork) : IGenreService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<GenreDto?> GetByIdAsync(Guid id)
    {
        var genre = await _unitOfWork.Genres.GetByIdAsync(id)
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
        var genres = await _unitOfWork.Genres.GetAllAsync();
        return genres.Select(genre => new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name,
            ParentGenreId = genre.ParentGenreId,
        });
    }

    public async Task<IEnumerable<GenreDto>> GetByGameKeyAsync(string gameKey)
    {
        var genres = await _unitOfWork.Genres.GetByGameKeyAsync(gameKey);
        return genres.Select(genre => new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name,
            ParentGenreId = genre.ParentGenreId,
        });
    }

    public async Task<IEnumerable<GenreDto>> GetByParentIdAsync(Guid parentId)
    {
        var genres = await _unitOfWork.Genres.GetByParentIdAsync(parentId);
        return genres.Select(genre => new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name,
            ParentGenreId = genre.ParentGenreId,
        });
    }

    public async Task CreateAsync(CreateGenreRequestDto request)
    {
        var genre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = request.Genre.Name,
            ParentGenreId = request.Genre.ParentGenreId,
        };

        await _unitOfWork.Genres.AddAsync(genre);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateGenreRequestDto request)
    {
        var genre = await _unitOfWork.Genres.GetByIdAsync(request.Genre.Id)
                    ?? throw new KeyNotFoundException($"Genre with id {request.Genre.Id} not found");

        genre.Name = request.Genre.Name;
        genre.ParentGenreId = request.Genre.ParentGenreId;

        _unitOfWork.Genres.Update(genre);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var genre = await _unitOfWork.Genres.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Genre with id {id} not found");
        _unitOfWork.Genres.Delete(genre);
        await _unitOfWork.SaveChangesAsync();
    }
}