using Gamestore.API.Interfaces;

namespace Gamestore.API.Services;

public class FileLoggerService : IFileLoggerService
{
    private readonly string _logsDirectory;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public FileLoggerService()
    {
        _logsDirectory = Path.Combine(
            Directory.GetCurrentDirectory(), "logs");

        Directory.CreateDirectory(_logsDirectory);
    }

    public async Task LogAsync(string message)
    {
        await _semaphore.WaitAsync();
        try
        {
            var filePath = Path.Combine(
                _logsDirectory,
                $"log-{DateTime.UtcNow:yyyy-MM-dd}.txt");

            var entry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";

            await File.AppendAllTextAsync(filePath, entry);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task LogErrorAsync(string message)
    {
        await _semaphore.WaitAsync();
        try
        {
            var filePath = Path.Combine(
                _logsDirectory,
                $"error-{DateTime.UtcNow:yyyy-MM-dd}.txt");

            var entry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";

            await File.AppendAllTextAsync(filePath, entry);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}