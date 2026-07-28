using Gamestore.BLL.DTOs.Ban;
using Gamestore.BLL.DTOs.Comment;

namespace Gamestore.BLL.Interfaces;

public interface ICommentService
{
    Task CreateAsync(string gameKey, CreateCommentRequestDto request);

    Task<IEnumerable<CommentDto>> GetByGameKeyAsync(string gameKey);

    Task DeleteAsync(string gameKey, Guid commentId);

    IEnumerable<string> GetBanDurations();

    Task BanUserAsync(BanRequestDto request);
}