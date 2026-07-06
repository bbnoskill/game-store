namespace Gamestore.API.Interfaces;

public interface IFileLoggerService
{
    Task LogAsync(string message);

    Task LogErrorAsync(string message);
}