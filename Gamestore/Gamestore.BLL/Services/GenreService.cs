using Gamestore.BLL.DTOs.Genre;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;

namespace Gamestore.BLL.Services;

public class GenreService : IGenreService
{
    private readonly IUnitOfWork _unitOfWork;

    public GenreService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GenreDto?> GetByIdAsync(Guid id)
    {
        var genre = await _unitOfWork.Genres.GetByIdAsync(id);
        if (genre == null) return null;

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

    public async Task<IEnumerable<GenreDto>> GetByGameKeyAsync(string key)
    {
        var genres = await _unitOfWork.Genres.GetByGameKeyAsync(key);
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

        await _unitOfWork.Genres.UpdateAsync(genre);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var genre = await _unitOfWork.Genres.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Genre with id {id} not found");
        await _unitOfWork.Genres.DeleteAsync(genre);
        await _unitOfWork.SaveChangesAsync();
    }
}