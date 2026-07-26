using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Repositories.Implementations;

public class CommentRepository(GamestoreContext context) : Repository<Comment>(context), ICommentRepository
{
    public async Task<IEnumerable<Comment>> GetByGameKeyAsync(string gameKey)
    {
        return await Context.Comments
            .Where(comment => comment.Game.Key == gameKey)
            .ToListAsync();
    }
}