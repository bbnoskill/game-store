using Gamestore.BLL.DTOs.Publisher;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Gamestore.BLL.Services;

public class PublisherService(IUnitOfWork unitOfWork, ILogger<GameService> logger) : IPublisherService
{
    public async Task<PublisherDto?> GetByNameAsync(string companyName)
    {
        logger.LogInformation("Fetching publisher by name: {CompanyName}", companyName);
        var publisher = await unitOfWork.Publishers.GetByCompanyNameAsync(companyName)
            ?? throw new KeyNotFoundException($"Publisher with company name '{companyName}' was not found.");
        return new PublisherDto
        {
            Id = publisher.Id,
            CompanyName = publisher.CompanyName,
            Description = publisher.Description,
            HomePage = publisher.HomePage,
        };
    }

    public async Task<PublisherDto?> GetByGameKeyAsync(string gameKey)
    {
        logger.LogInformation("Fetching publisher by game key: {GameKey}", gameKey);
        var publisher = await unitOfWork.Publishers.GetByGameKeyAsync(gameKey)
            ?? throw new KeyNotFoundException($"Publisher with game key '{gameKey}' was not found.");
        return new PublisherDto()
        {
            Id = publisher.Id,
            CompanyName = publisher.CompanyName,
            Description = publisher.Description,
            HomePage = publisher.HomePage,
        };
    }

    public async Task<IEnumerable<PublisherDto>> GetAllAsync()
    {
        logger.LogInformation("Fetching all publishers.");
        var publishers = (await unitOfWork.Publishers.GetAllAsync()).ToList();
        logger.LogInformation("Fetched {Count} publishers.", publishers.Count);
        return publishers.Select(publisher => new PublisherDto
        {
            Id = publisher.Id,
            CompanyName = publisher.CompanyName,
            Description = publisher.Description,
            HomePage = publisher.HomePage,
        });
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
        logger.LogInformation("Created publisher: {CompanyName}", publisher.CompanyName);
    }

    public async Task UpdateAsync(UpdatePublisherRequestDto request)
    {
        logger.LogInformation("Updating publisher: {Id}", request.Publisher.Id);
        var publisher = await unitOfWork.Publishers.GetByIdAsync(request.Publisher.Id)
                       ?? throw new KeyNotFoundException($"Publisher with id {request.Publisher.Id} not found");
        publisher.CompanyName = request.Publisher.CompanyName;
        publisher.Description = request.Publisher.Description;
        publisher.HomePage = request.Publisher.HomePage;

        unitOfWork.Publishers.Update(publisher);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Updated publisher: {Id}", publisher.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("Deleting publisher with id: {Id}", id);
        var publisher = await unitOfWork.Publishers.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException($"Publisher with id {id} not found");
        unitOfWork.Publishers.Delete(publisher);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Deleted publisher with id: {Id}", id);
    }
}