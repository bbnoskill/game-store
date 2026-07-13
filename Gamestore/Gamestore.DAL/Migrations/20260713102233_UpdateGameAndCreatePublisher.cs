using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gamestore.DAL.Migrations;

/// <inheritdoc />
public partial class UpdateGameAndCreatePublisher : Migration
{
    private static readonly string[] Columns = ["Id", "CompanyName", "Description", "HomePage"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "PublisherId",
            table: "Games",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("0f7f61ac-babe-423d-a6fe-a5a6c842eacd"),
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier");

        migrationBuilder.InsertData(
            table: "Publishers",
            columns: Columns,
            values: new object[,]
            {
                { new Guid("0f7f61ac-babe-423d-a6fe-a5a6c842eacd"), "Test Name", "Some text", "https://localhost:7091/swagger/index.html" },
                { new Guid("77c06fc6-f8a1-46f9-af03-2793e500112c"), "Electronic Arts", "American video game company", "https://www.ea.com" },
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Publishers",
            keyColumn: "Id",
            keyValue: new Guid("0f7f61ac-babe-423d-a6fe-a5a6c842eacd"));

        migrationBuilder.DeleteData(
            table: "Publishers",
            keyColumn: "Id",
            keyValue: new Guid("77c06fc6-f8a1-46f9-af03-2793e500112c"));

        migrationBuilder.AlterColumn<Guid>(
            name: "PublisherId",
            table: "Games",
            type: "uniqueidentifier",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier",
            oldDefaultValue: new Guid("0f7f61ac-babe-423d-a6fe-a5a6c842eacd"));
    }
}