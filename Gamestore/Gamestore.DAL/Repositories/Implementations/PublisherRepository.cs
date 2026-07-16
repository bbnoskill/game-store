using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Repositories.Implementations;

public class PublisherRepository(GamestoreContext context) : Repository<Publisher>(context), IPublisherRepository
{
    public Task<Publisher?> GetByCompanyNameAsync(string companyName)
    {
        return Context.Publishers
            .FirstOrDefaultAsync(publisher => publisher.CompanyName == companyName);
    }

    public Task<Publisher?> GetByGameKeyAsync(string gameKey)
    {
        return Context.Publishers
            .FirstOrDefaultAsync(publisher => publisher.Games.Any(g => g.Key == gameKey));
    }
}