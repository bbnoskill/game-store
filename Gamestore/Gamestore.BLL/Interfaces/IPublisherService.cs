using Gamestore.BLL.DTOs.Publisher;

namespace Gamestore.BLL.Interfaces;

public interface IPublisherService
{
    Task<PublisherDto?> GetByNameAsync(string companyName);

    Task<PublisherDto?> GetByGameKeyAsync(string gameKey);

    Task<IEnumerable<PublisherDto>> GetAllAsync();

    Task CreateAsync(CreatePublisherRequestDto request);

    Task UpdateAsync(UpdatePublisherRequestDto request);

    Task DeleteAsync(Guid id);
}