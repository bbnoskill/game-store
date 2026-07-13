using Gamestore.DAL.Entities;

namespace Gamestore.DAL.Repositories.Interfaces;

public interface IPublisherRepository
{
    Task<Publisher?> GetByCompanyNameAsync(string companyName);
}