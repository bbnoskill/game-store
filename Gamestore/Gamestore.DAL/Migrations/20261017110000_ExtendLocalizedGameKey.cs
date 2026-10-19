using Gamestore.DAL.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gamestore.DAL.Migrations;

[DbContext(typeof(GamestoreContext))]
[Migration("20261017110000_ExtendLocalizedGameKey")]
public partial class ExtendLocalizedGameKey : Migration
{
    private const string FunctionSql = """
        CREATE OR ALTER FUNCTION dbo.GetGameInfoByLocalization(
            @GameKey NVARCHAR(100),
            @LanguageCode NVARCHAR(10)
        )
        RETURNS TABLE
        AS
        RETURN
        (
            SELECT 
                g.Id,
                g.[Key],
                COALESCE(gl_target.[Name], gl_en.[Name], g.[Name]) AS [Name],
                COALESCE(gl_target.[Description], gl_en.[Description], g.[Description]) AS [Description],
                g.Price,
                g.Discount,
                g.UnitInStock,
                g.ViewCount,
                g.ReleaseDate,
                g.PublisherId
            FROM Games g
            LEFT JOIN GameLocalizations gl_target 
                ON g.Id = gl_target.GameId AND gl_target.LanguageCode = @LanguageCode
            LEFT JOIN GameLocalizations gl_en 
                ON g.Id = gl_en.GameId AND gl_en.LanguageCode = 'en'
            WHERE g.[Key] = @GameKey
        );
        """;

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(FunctionSql.Replace("NVARCHAR(100)", "NVARCHAR(450)", StringComparison.Ordinal));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(FunctionSql);
    }
}
