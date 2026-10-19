using Gamestore.BLL.DTOs.Publisher;

namespace Gamestore.BLL.Interfaces;

public interface IPublisherService
{
    Task<PublisherDto?> GetByNameAsync(string companyName, string languageCode = "en", bool includeMongo = true);

    Task<PublisherDto?> GetByGameKeyAsync(string gameKey, string languageCode = "en");

    Task<IEnumerable<PublisherDto>> GetAllAsync(string languageCode = "en", bool includeMongo = true);

    Task CreateAsync(CreatePublisherRequestDto request, string languageCode = "en");

    Task UpdateAsync(UpdatePublisherRequestDto request, string languageCode = "en");

    Task DeleteAsync(Guid id);
}