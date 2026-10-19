using Gamestore.BLL.Interfaces;

namespace Gamestore.API.Middleware;

public class LocalizationHeaders(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ILanguageService languageService)
    {
        var language = languageService.NormalizeLanguageCode(context.Request.Headers.AcceptLanguage.ToString());
        context.Response.Headers.ContentLanguage = language;
        context.Response.Headers.Append("Vary", "Accept-Language");
        await next(context);
    }
}
