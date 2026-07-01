using Gamestore.API.Interfaces;
using Gamestore.API.Services;

namespace Gamestore.API.Middleware;

public class ExceptionHandling(RequestDelegate next, FileLoggerService fileLogger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var errorLog = $"""
                        Exception Type: {ex.GetType().Name}
                        Message: {ex.Message}
                        StackTrace: {ex.StackTrace}
                        """;
        await fileLogger.LogErrorAsync(errorLog);
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = ex switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError,
        };

        var response = new
        {
            statusCode = context.Response.StatusCode,
            message = ex.Message,
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}