using Gamestore.BLL.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Gamestore.API.Middleware;

public class TotalGamesCount(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(async () =>
        {
            var cache = context.RequestServices.GetRequiredService<IMemoryCache>();
            var gameService = context.RequestServices.GetRequiredService<IGameService>();
            var count = await cache.GetOrCreateAsync("total-games-count", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            return await gameService.GetTotalCountAsync();
        });
            context.Response.Headers["x-total-number-of-games"] = count.ToString();
        });
        await next(context);
    }
}