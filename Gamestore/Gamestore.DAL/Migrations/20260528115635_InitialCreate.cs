using Microsoft.EntityFrameworkCore.Migrations;

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gamestore.DAL.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    private static readonly string[] Columns = ["Id", "Name", "ParentGenreId"];
    private static readonly string[] ColumnsArray = ["Id", "Type"];
    private static readonly string[] ColumnsValue = ["Id", "Name", "ParentGenreId"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Games",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Games", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Genres",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ParentGenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Genres", x => x.Id);
                table.ForeignKey(
                    name: "FK_Genres_Genres_ParentGenreId",
                    column: x => x.ParentGenreId,
                    principalTable: "Genres",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Platforms",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Platforms", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GameGenres",
            columns: table => new
            {
                GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GameGenres", x => new { x.GameId, x.GenreId });
                table.ForeignKey(
                    name: "FK_GameGenres_Games_GameId",
                    column: x => x.GameId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GameGenres_Genres_GenreId",
                    column: x => x.GenreId,
                    principalTable: "Genres",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "GamePlatforms",
            columns: table => new
            {
                GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PlatformId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GamePlatforms", x => new { x.GameId, x.PlatformId });
                table.ForeignKey(
                    name: "FK_GamePlatforms_Games_GameId",
                    column: x => x.GameId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GamePlatforms_Platforms_PlatformId",
                    column: x => x.PlatformId,
                    principalTable: "Platforms",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "Genres",
            columns: ColumnsValue,
            values: new object[,]
            {
                { new Guid("a1b2c3d4-e5f6-4a7b-8c9d-e0f1a2b3c4d5"), "Strategy", null },
                { new Guid("a7b8c9d0-e1f2-4a3b-4c5d-e6f7a8b9c0d1"), "Races", null },
                { new Guid("a9b0c1d2-e3f4-4a5b-6c7d-e8f9a0b1c2d3"), "Puzzle & Skill", null },
                { new Guid("c5d6e7f8-a9b0-4c1d-2e3f-a4b5c6d7e8f9"), "Fighting", null },
                { new Guid("d4e5f6a7-b8c9-4d0e-1f2a-b3c4d5e6f7a8"), "RPG", null },
                { new Guid("e5f6a7b8-c9d0-4e1f-2a3b-c4d5e6f7a8b9"), "Simulator", null },
                { new Guid("f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6"), "Action", null },
                { new Guid("f8a9b0c1-d2e3-4f4a-5b6c-d7e8f9a0b1c2"), "Adventure", null },
            });

        migrationBuilder.InsertData(
            table: "Platforms",
            columns: ColumnsArray,
            values: new object[,]
            {
                { new Guid("b5d6d065-a1cf-4c0a-9d56-c55d73e82e5b"), "Desktop" },
                { new Guid("c7e8f012-b2d0-4d1b-ae67-d66e84f93f6c"), "Browser" },
                { new Guid("d9f9a123-c3e1-4e2c-bf78-e77f95a04a7d"), "Console" },
                { new Guid("e1d6d065-e3df-4c0a-8d46-b44d62d71d4a"), "Mobile" },
                { new Guid("f0a0b234-d4f2-4f3d-c089-f88a06b15b8e"), "VR" },
            });

        migrationBuilder.InsertData(
            table: "Genres",
            columns: Columns,
            values: new object[,]
            {
                { new Guid("a3b4c5d6-e7f8-4a9b-0c1d-e2f3a4b5c6d7"), "FPS", new Guid("f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6") },
                { new Guid("b2c3d4e5-f6a7-4b8c-9d0e-f1a2b3c4d5e6"), "RTS", new Guid("a1b2c3d4-e5f6-4a7b-8c9d-e0f1a2b3c4d5") },
                { new Guid("b4c5d6e7-f8a9-4b0c-1d2e-f3a4b5c6d7e8"), "TPS", new Guid("f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6") },
                { new Guid("b8c9d0e1-f2a3-4b4c-5d6e-f7a8b9c0d1e2"), "Rally", new Guid("a7b8c9d0-e1f2-4a3b-4c5d-e6f7a8b9c0d1") },
                { new Guid("c3d4e5f6-a7b8-4c9d-0e1f-a2b3c4d5e6f7"), "TBS", new Guid("a1b2c3d4-e5f6-4a7b-8c9d-e0f1a2b3c4d5") },
                { new Guid("c9d0e1f2-a3b4-4c5d-6e7f-a8b9c0d1e2f3"), "Arcade", new Guid("a7b8c9d0-e1f2-4a3b-4c5d-e6f7a8b9c0d1") },
                { new Guid("d0e1f2a3-b4c5-4d6e-7f8a-b9c0d1e2f3a4"), "Formula", new Guid("a7b8c9d0-e1f2-4a3b-4c5d-e6f7a8b9c0d1") },
                { new Guid("d6e7f8a9-b0c1-4d2e-3f4a-b5c6d7e8f9a0"), "Battle Royale", new Guid("f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6") },
                { new Guid("e1f2a3b4-c5d6-4e7f-8a9b-c0d1e2f3a4b5"), "Off-road", new Guid("a7b8c9d0-e1f2-4a3b-4c5d-e6f7a8b9c0d1") },
                { new Guid("f6a7b8c9-d0e1-4f2a-3b4c-d5e6f7a8b9c0"), "Sports", new Guid("e5f6a7b8-c9d0-4e1f-2a3b-c4d5e6f7a8b9") },
                { new Guid("e7f8a9b0-c1d2-4e3f-4a5b-c6d7e8f9a0b1"), "Hero Shooter", new Guid("a3b4c5d6-e7f8-4a9b-0c1d-e2f3a4b5c6d7") },
            });

        migrationBuilder.CreateIndex(
            name: "IX_GameGenres_GenreId",
            table: "GameGenres",
            column: "GenreId");

        migrationBuilder.CreateIndex(
            name: "IX_GamePlatforms_PlatformId",
            table: "GamePlatforms",
            column: "PlatformId");

        migrationBuilder.CreateIndex(
            name: "IX_Games_Key",
            table: "Games",
            column: "Key",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Genres_Name",
            table: "Genres",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Genres_ParentGenreId",
            table: "Genres",
            column: "ParentGenreId");

        migrationBuilder.CreateIndex(
            name: "IX_Platforms_Type",
            table: "Platforms",
            column: "Type",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GameGenres");

        migrationBuilder.DropTable(
            name: "GamePlatforms");

        migrationBuilder.DropTable(
            name: "Genres");

        migrationBuilder.DropTable(
            name: "Games");

        migrationBuilder.DropTable(
            name: "Platforms");
    }
}