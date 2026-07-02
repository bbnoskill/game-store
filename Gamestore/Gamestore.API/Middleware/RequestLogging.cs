using System.Diagnostics;
using System.Text;
using Gamestore.API.Interfaces;

namespace Gamestore.API.Middleware;

public class RequestLogging(RequestDelegate next, IFileLoggerService fileLogger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestBody = await ReadRequestBodyAsync(context.Request);

        var originalBody = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            // Йдемо далі по конвеєру
            await next(context);

            // Якщо все пройшло успішно — логуємо нормальну відповідь
            stopwatch.Stop();
            var responseBody = await ReadResponseBodyAsync(responseBodyStream);

            context.Response.Body = originalBody;
            await responseBodyStream.CopyToAsync(originalBody);

            var log = $"""
                       --- Request ---
                       IP: {context.Connection.RemoteIpAddress}
                       Method: {context.Request.Method}
                       URL: {context.Request.Path}{context.Request.QueryString}
                       Body: {requestBody}
                       --- Response ---
                       Status: {context.Response.StatusCode}
                       Body: {responseBody}
                       Elapsed: {stopwatch.ElapsedMilliseconds}ms
                       ----------------
                       """;

            await fileLogger.LogAsync(log);
        }
        catch (Exception ex)
        {
            // КРИТИЧНО: Якщо сталася помилка, обов'язково повертаємо оригінальний потік!
            // Тепер ExceptionHandling зможе успішно записати свій JSON.
            context.Response.Body = originalBody;

            stopwatch.Stop();

            // (Опціонально) Записуємо у файл, що запит впав з помилкою
            var errorLog = $"""
                           --- Request (FAILED) ---
                           IP: {context.Connection.RemoteIpAddress}
                           Method: {context.Request.Method}
                           URL: {context.Request.Path}{context.Request.QueryString}
                           Body: {requestBody}
                           --- Error ---
                           Exception: {ex.Message}
                           Elapsed: {stopwatch.ElapsedMilliseconds}ms
                           ----------------
                           """;

            await fileLogger.LogAsync(errorLog);

            // Прокидаємо помилку далі, щоб її впіймав ExceptionHandling Middleware
            throw;
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        using var reader = new StreamReader(
            request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return body;
    }

    private static async Task<string> ReadResponseBodyAsync(MemoryStream responseBodyStream)
    {
        responseBodyStream.Position = 0;
        var body = await new StreamReader(responseBodyStream).ReadToEndAsync();
        responseBodyStream.Position = 0;
        return body;
    }
}