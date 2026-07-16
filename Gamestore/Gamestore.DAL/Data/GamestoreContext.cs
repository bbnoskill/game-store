using Gamestore.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Data;

public class GamestoreContext(DbContextOptions<GamestoreContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();

    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<Platform> Platforms => Set<Platform>();

    public DbSet<GameGenre> GameGenres => Set<GameGenre>();

    public DbSet<GamePlatform> GamePlatforms => Set<GamePlatform>();

    public DbSet<Publisher> Publishers => Set<Publisher>();

    public static void SeedData(ModelBuilder modelBuilder)
    {
        var mobile = new Platform { Id = new Guid("e1d6d065-e3df-4c0a-8d46-b44d62d71d4a"), Type = "Mobile" };
        var desktop = new Platform { Id = new Guid("b5d6d065-a1cf-4c0a-9d56-c55d73e82e5b"), Type = "Desktop" };
        var browser = new Platform { Id = new Guid("c7e8f012-b2d0-4d1b-ae67-d66e84f93f6c"), Type = "Browser" };
        var console = new Platform { Id = new Guid("d9f9a123-c3e1-4e2c-bf78-e77f95a04a7d"), Type = "Console" };
        var virtualReality = new Platform { Id = new Guid("f0a0b234-d4f2-4f3d-c089-f88a06b15b8e"), Type = "VR" };
        modelBuilder.Entity<Platform>().HasData(mobile, desktop, browser, console, virtualReality);

        var strategy = new Genre { Id = new Guid("a1b2c3d4-e5f6-4a7b-8c9d-e0f1a2b3c4d5"), Name = "Strategy" };
        var rts = new Genre
        { Id = new Guid("b2c3d4e5-f6a7-4b8c-9d0e-f1a2b3c4d5e6"), Name = "RTS", ParentGenreId = strategy.Id };
        var tbs = new Genre
        { Id = new Guid("c3d4e5f6-a7b8-4c9d-0e1f-a2b3c4d5e6f7"), Name = "TBS", ParentGenreId = strategy.Id };
        var rpg = new Genre { Id = new Guid("d4e5f6a7-b8c9-4d0e-1f2a-b3c4d5e6f7a8"), Name = "RPG" };
        var simulator = new Genre { Id = new Guid("e5f6a7b8-c9d0-4e1f-2a3b-c4d5e6f7a8b9"), Name = "Simulator" };
        var sports = new Genre
        { Id = new Guid("f6a7b8c9-d0e1-4f2a-3b4c-d5e6f7a8b9c0"), Name = "Sports", ParentGenreId = simulator.Id };
        var races = new Genre { Id = new Guid("a7b8c9d0-e1f2-4a3b-4c5d-e6f7a8b9c0d1"), Name = "Races" };
        var rally = new Genre
        { Id = new Guid("b8c9d0e1-f2a3-4b4c-5d6e-f7a8b9c0d1e2"), Name = "Rally", ParentGenreId = races.Id };
        var arcade = new Genre
        { Id = new Guid("c9d0e1f2-a3b4-4c5d-6e7f-a8b9c0d1e2f3"), Name = "Arcade", ParentGenreId = races.Id };
        var formula = new Genre
        { Id = new Guid("d0e1f2a3-b4c5-4d6e-7f8a-b9c0d1e2f3a4"), Name = "Formula", ParentGenreId = races.Id };
        var offroad = new Genre
        { Id = new Guid("e1f2a3b4-c5d6-4e7f-8a9b-c0d1e2f3a4b5"), Name = "Off-road", ParentGenreId = races.Id };
        var action = new Genre { Id = new Guid("f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6"), Name = "Action" };
        var fps = new Genre
        { Id = new Guid("a3b4c5d6-e7f8-4a9b-0c1d-e2f3a4b5c6d7"), Name = "FPS", ParentGenreId = action.Id };
        var tps = new Genre
        { Id = new Guid("b4c5d6e7-f8a9-4b0c-1d2e-f3a4b5c6d7e8"), Name = "TPS", ParentGenreId = action.Id };
        var fighting = new Genre { Id = new Guid("c5d6e7f8-a9b0-4c1d-2e3f-a4b5c6d7e8f9"), Name = "Fighting" };
        var battleRoyale = new Genre
        {
            Id = new Guid("d6e7f8a9-b0c1-4d2e-3f4a-b5c6d7e8f9a0"),
            Name = "Battle Royale",
            ParentGenreId = action.Id,
        };
        var heroShooter = new Genre
        { Id = new Guid("e7f8a9b0-c1d2-4e3f-4a5b-c6d7e8f9a0b1"), Name = "Hero Shooter", ParentGenreId = fps.Id };
        var adventure = new Genre { Id = new Guid("f8a9b0c1-d2e3-4f4a-5b6c-d7e8f9a0b1c2"), Name = "Adventure" };
        var puzzle = new Genre { Id = new Guid("a9b0c1d2-e3f4-4a5b-6c7d-e8f9a0b1c2d3"), Name = "Puzzle & Skill" };

        modelBuilder.Entity<Genre>().HasData(
            strategy, rts, tbs, rpg, simulator, sports, races, rally, arcade, formula, offroad, action, fps, tps, fighting, battleRoyale, heroShooter, adventure, puzzle);

        var publisher1 = new Publisher
        {
            Id = new Guid("77c06fc6-f8a1-46f9-af03-2793e500112c"),
            CompanyName = "Electronic Arts",
            HomePage = "https://www.ea.com",
            Description = "American video game company",
        };
        var publisher2 = new Publisher
        {
            Id = new Guid("0f7f61ac-babe-423d-a6fe-a5a6c842eacd"),
            CompanyName = "Test Name",
            HomePage = "https://localhost:7091/swagger/index.html",
            Description = "Some text",
        };
        modelBuilder.Entity<Publisher>().HasData(publisher1, publisher2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameGenre>()
            .HasKey(gg => new { gg.GameId, gg.GenreId });

        modelBuilder.Entity<GamePlatform>()
            .HasKey(gp => new { gp.GameId, gp.PlatformId });

        modelBuilder.Entity<Game>()
            .Property(g => g.PublisherId)
            .HasDefaultValue(new Guid("0f7f61ac-babe-423d-a6fe-a5a6c842eacd"));

        modelBuilder.Entity<Game>().HasIndex(g => g.Key).IsUnique();
        modelBuilder.Entity<Genre>().HasIndex(g => g.Name).IsUnique();
        modelBuilder.Entity<Platform>().HasIndex(p => p.Type).IsUnique();
        modelBuilder.Entity<Publisher>().HasIndex(p => p.CompanyName).IsUnique();

        modelBuilder.Entity<Genre>()
            .HasOne(g => g.ParentGenre)
            .WithMany(g => g.SubGenres)
            .HasForeignKey(g => g.ParentGenreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.Publisher)
            .WithMany(p => p.Games)
            .HasForeignKey(g => g.PublisherId)
            .OnDelete(DeleteBehavior.Cascade);

        SeedData(modelBuilder);
    }
}