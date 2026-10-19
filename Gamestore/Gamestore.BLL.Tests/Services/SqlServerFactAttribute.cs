namespace Gamestore.BLL.Tests.Services;

[AttributeUsage(AttributeTargets.Method)]
public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GAMESTORE_TEST_SQL_CONNECTION")))
        {
            Skip = "Set GAMESTORE_TEST_SQL_CONNECTION to run against a disposable SQL Server database.";
        }
    }
}
