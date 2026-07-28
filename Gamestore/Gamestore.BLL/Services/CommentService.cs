using Gamestore.BLL.DTOs.Ban;
using Gamestore.BLL.DTOs.Comment;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Gamestore.BLL.Services;

public class CommentService(IUnitOfWork unitOfWork, IMemoryCache cache, ILogger<CommentService> logger) : ICommentService
{
    private const string DeletedMessage = "A comment/quote was deleted";

    private static readonly List<string> BanDurations =
    [
        "1 hour",
        "1 day",
        "1 week",
        "1 month",
        "permanent",
    ];

    public async Task CreateAsync(string gameKey, CreateCommentRequestDto request)
    {
        logger.LogInformation("Adding comment to game '{Key}' by '{Name}'.", gameKey, request.Comment.Name);

        var game = await unitOfWork.Games.GetByKeyAsync(gameKey)
                   ?? throw new KeyNotFoundException($"Game with key '{gameKey}' was not found.");

        var cacheKey = $"ban_{request.Comment.Name.ToLower(System.Globalization.CultureInfo.CurrentCulture)}";
        if (cache.TryGetValue(cacheKey, out _))
        {
            throw new InvalidOperationException($"User '{request.Comment.Name}' is banned and cannot add comments.");
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Name = request.Comment.Name,
            Body = request.Comment.Body,
            GameId = game.Id,
        };

        if (!string.IsNullOrEmpty(request.ParentId) && Guid.TryParse(request.ParentId, out var parentId))
        {
            var parentComment = await unitOfWork.Comments.GetByIdAsync(parentId)
                                ?? throw new KeyNotFoundException($"Parent comment with id '{parentId}' was not found.");

            comment.ParentCommentId = parentId;

            if (string.Equals(request.Action, "reply", StringComparison.OrdinalIgnoreCase))
            {
                comment.Body = $"[{parentComment.Name}], {request.Comment.Body}";
            }
            else if (string.Equals(request.Action, "quote", StringComparison.OrdinalIgnoreCase))
            {
                var quoteText = parentComment.Body == DeletedMessage ? DeletedMessage : parentComment.Body;
                comment.Body = $"[{quoteText}], {request.Comment.Body}";
            }
        }

        await unitOfWork.Comments.AddAsync(comment);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Comment '{Id}' added to game '{Key}' successfully.", comment.Id, gameKey);
    }

    public async Task<IEnumerable<CommentDto>> GetByGameKeyAsync(string gameKey)
    {
        logger.LogInformation("Fetching comments for game '{Key}'.", gameKey);

        var comments = (await unitOfWork.Comments.GetByGameKeyAsync(gameKey)).ToList();

        var topLevel = comments
            .Where(c => c.ParentCommentId == null)
            .Select(c => BuildCommentTree(c, comments))
            .ToList();

        logger.LogInformation("Fetched {Count} top-level comments for game '{Key}'.", topLevel.Count, gameKey);
        return topLevel;
    }

    public async Task DeleteAsync(string gameKey, Guid commentId)
    {
        logger.LogInformation("Deleting comment '{Id}' from game '{Key}'.", commentId, gameKey);

        var comment = await unitOfWork.Comments.GetByIdAsync(commentId)
                      ?? throw new KeyNotFoundException($"Comment with id '{commentId}' was not found.");

        var gameComments = (await unitOfWork.Comments.GetByGameKeyAsync(gameKey)).ToList();
        if (gameComments.All(c => c.Id != commentId))
        {
            throw new KeyNotFoundException($"Comment '{commentId}' does not belong to game '{gameKey}'.");
        }

        comment.Body = DeletedMessage;
        unitOfWork.Comments.Update(comment);

        var childQuotes = gameComments
            .Where(c => c.ParentCommentId == commentId && c.Body.StartsWith("[", StringComparison.CurrentCulture));

        foreach (var child in childQuotes)
        {
            var closingBracket = child.Body.IndexOf("],", StringComparison.CurrentCulture);
            if (closingBracket > 0)
            {
                child.Body = $"[{DeletedMessage}]{child.Body[closingBracket..]}";
                unitOfWork.Comments.Update(child);
            }
        }

        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Comment '{Id}' deleted (soft) from game '{Key}'.", commentId, gameKey);
    }

    public IEnumerable<string> GetBanDurations()
    {
        logger.LogInformation("Fetching ban durations.");
        return BanDurations;
    }

    public Task BanUserAsync(BanRequestDto request)
    {
        logger.LogInformation("Banning user '{User}' for '{Duration}'.", request.User, request.Duration);

        if (!BanDurations.Contains(request.Duration))
        {
            throw new ArgumentException($"Invalid ban duration: '{request.Duration}'.");
        }

        var cacheKey = $"ban_{request.User.ToLowerInvariant()}";

        if (request.Duration == "permanent")
        {
            cache.Set(cacheKey, true);
        }
        else
        {
            var expiration = request.Duration switch
            {
                "1 hour" => TimeSpan.FromHours(1),
                "1 day" => TimeSpan.FromDays(1),
                "1 week" => TimeSpan.FromDays(7),
                "1 month" => TimeSpan.FromDays(30),
                _ => throw new ArgumentException($"Invalid ban duration: '{request.Duration}'."),
            };

            cache.Set(cacheKey, true, expiration);
        }

        logger.LogInformation("User '{User}' banned for '{Duration}' successfully.", request.User, request.Duration);
        return Task.CompletedTask;
    }

    private static CommentDto BuildCommentTree(Comment comment, List<Comment> allComments)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Name = comment.Name,
            Body = comment.Body,
            ChildComments =
            [
                ..allComments
                    .Where(c => c.ParentCommentId == comment.Id)
                    .Select(c => BuildCommentTree(c, allComments)),
            ],
        };
    }
}