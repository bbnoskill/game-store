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

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderGame> OrderGames => Set<OrderGame>();

    public DbSet<Comment> Comments => Set<Comment>();

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

        // Publishers (25)
        var pubEa = new Publisher { Id = new Guid("77c06fc6-f8a1-46f9-af03-2793e500112c"), CompanyName = "Electronic Arts", HomePage = "https://www.ea.com", Description = "American video game company" };
        var pubDefault = new Publisher { Id = new Guid("0f7f61ac-babe-423d-a6fe-a5a6c842eacd"), CompanyName = "Test Name", HomePage = "https://localhost:7091/swagger/index.html", Description = "Some text" };
        var pubValve = new Publisher { Id = new Guid("4ca4f3b7-7e23-45f6-8c43-bcde64a51fac"), CompanyName = "Valve", HomePage = "https://www.valvesoftware.com", Description = "Creators of Steam and Half-Life" };
        var pubBethesda = new Publisher { Id = new Guid("5bc5d6a2-6f91-4d3f-91ab-64213d8e578c"), CompanyName = "Bethesda", HomePage = "https://bethesda.net", Description = "Makers of Skyrim and Fallout" };
        var pubCdpr = new Publisher { Id = new Guid("1c3a64b9-9142-4f3b-823c-7c4a1653d8a1"), CompanyName = "CD Projekt Red", HomePage = "https://cdprojekt.com", Description = "Witcher and Cyberpunk developers" };
        var pubNintendo = new Publisher { Id = new Guid("a1110001-0001-4001-8001-000000000001"), CompanyName = "Nintendo", HomePage = "https://www.nintendo.com", Description = "Japanese gaming giant" };
        var pubSony = new Publisher { Id = new Guid("a1110002-0002-4002-8002-000000000002"), CompanyName = "Sony Interactive", HomePage = "https://www.playstation.com", Description = "PlayStation maker" };
        var pubUbisoft = new Publisher { Id = new Guid("a1110003-0003-4003-8003-000000000003"), CompanyName = "Ubisoft", HomePage = "https://www.ubisoft.com", Description = "French publisher of Assassin's Creed" };
        var pubActivision = new Publisher { Id = new Guid("a1110004-0004-4004-8004-000000000004"), CompanyName = "Activision", HomePage = "https://www.activision.com", Description = "Call of Duty publisher" };
        var pubSquare = new Publisher { Id = new Guid("a1110005-0005-4005-8005-000000000005"), CompanyName = "Square Enix", HomePage = "https://www.square-enix.com", Description = "Final Fantasy developers" };
        var pubCapcom = new Publisher { Id = new Guid("a1110006-0006-4006-8006-000000000006"), CompanyName = "Capcom", HomePage = "https://www.capcom.com", Description = "Resident Evil and Monster Hunter" };
        var pubRockstar = new Publisher { Id = new Guid("a1110007-0007-4007-8007-000000000007"), CompanyName = "Rockstar Games", HomePage = "https://www.rockstargames.com", Description = "GTA and Red Dead developers" };
        var pubFromSoft = new Publisher { Id = new Guid("a1110008-0008-4008-8008-000000000008"), CompanyName = "FromSoftware", HomePage = "https://www.fromsoftware.jp", Description = "Dark Souls and Elden Ring creators" };
        var pubBungie = new Publisher { Id = new Guid("a1110009-0009-4009-8009-000000000009"), CompanyName = "Bungie", HomePage = "https://www.bungie.net", Description = "Destiny developers" };
        var pubEpic = new Publisher { Id = new Guid("a1110010-0010-4010-8010-000000000010"), CompanyName = "Epic Games", HomePage = "https://www.epicgames.com", Description = "Fortnite and Unreal Engine" };
        var pubSega = new Publisher { Id = new Guid("a1110011-0011-4011-8011-000000000011"), CompanyName = "Sega", HomePage = "https://www.sega.com", Description = "Sonic and Yakuza publisher" };
        var pubBandai = new Publisher { Id = new Guid("a1110012-0012-4012-8012-000000000012"), CompanyName = "Bandai Namco", HomePage = "https://www.bandainamcoent.com", Description = "Tekken and Elden Ring publisher" };
        var pubKonami = new Publisher { Id = new Guid("a1110013-0013-4013-8013-000000000013"), CompanyName = "Konami", HomePage = "https://www.konami.com", Description = "Metal Gear Solid publisher" };
        var pubTakeTwo = new Publisher { Id = new Guid("a1110014-0014-4014-8014-000000000014"), CompanyName = "Take-Two", HomePage = "https://www.take2games.com", Description = "Parent of Rockstar and 2K" };
        var pubMicrosoft = new Publisher { Id = new Guid("a1110015-0015-4015-8015-000000000015"), CompanyName = "Xbox Game Studios", HomePage = "https://www.xbox.com", Description = "Microsoft gaming division" };
        var pubNaughtyDog = new Publisher { Id = new Guid("a1110016-0016-4016-8016-000000000016"), CompanyName = "Naughty Dog", HomePage = "https://www.naughtydog.com", Description = "The Last of Us and Uncharted" };
        var pubInsomniac = new Publisher { Id = new Guid("a1110017-0017-4017-8017-000000000017"), CompanyName = "Insomniac Games", HomePage = "https://insomniac.games", Description = "Spider-Man PS developers" };
        var pubDevolver = new Publisher { Id = new Guid("a1110018-0018-4018-8018-000000000018"), CompanyName = "Devolver Digital", HomePage = "https://www.devolverdigital.com", Description = "Indie game publisher" };
        var pubSupergiant = new Publisher { Id = new Guid("a1110019-0019-4019-8019-000000000019"), CompanyName = "Supergiant Games", HomePage = "https://www.supergiantgames.com", Description = "Hades and Bastion creators" };
        var pubRiot = new Publisher { Id = new Guid("a1110020-0020-4020-8020-000000000020"), CompanyName = "Riot Games", HomePage = "https://www.riotgames.com", Description = "League of Legends developer" };
        modelBuilder.Entity<Publisher>().HasData(
            pubEa,
            pubDefault,
            pubValve,
            pubBethesda,
            pubCdpr,
            pubNintendo,
            pubSony,
            pubUbisoft,
            pubActivision,
            pubSquare,
            pubCapcom,
            pubRockstar,
            pubFromSoft,
            pubBungie,
            pubEpic,
            pubSega,
            pubBandai,
            pubKonami,
            pubTakeTwo,
            pubMicrosoft,
            pubNaughtyDog,
            pubInsomniac,
            pubDevolver,
            pubSupergiant,
            pubRiot);

        // Games (25) — diverse prices, dates (spread last 3 years), view counts
        var halo = new Game { Id = new Guid("9c5e3d7f-2b1a-4c8d-93e1-38a4d7c6b9f2"), Name = "Halo Infinite", Key = "halo-infinite", Description = "Sci-fi FPS", Price = 30.0, UnitInStock = 100, Discount = 0, ViewCount = 450, ReleaseDate = new DateTime(2024, 12, 8, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubMicrosoft.Id };
        var skyrim = new Game { Id = new Guid("4a8b7c2d-9e6f-4315-81a9-d3c2b1f8e7a6"), Name = "Skyrim", Key = "skyrim", Description = "Epic fantasy RPG", Price = 40.0, UnitInStock = 50, Discount = 10, ViewCount = 1200, ReleaseDate = new DateTime(2023, 11, 11, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubBethesda.Id };
        var witcher3 = new Game { Id = new Guid("2f1e9d8c-7b6a-45c3-84d2-f1e3a2b4c5d6"), Name = "The Witcher 3", Key = "witcher-3", Description = "Geralt's grand adventure", Price = 25.0, UnitInStock = 200, Discount = 5, ViewCount = 980, ReleaseDate = new DateTime(2024, 5, 19, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubCdpr.Id };
        var portal2 = new Game { Id = new Guid("3a4b5c6d-e7f8-4912-8c7d-6e5f4a3b2c1d"), Name = "Portal 2", Key = "portal-2", Description = "Thinking with portals", Price = 10.0, UnitInStock = 500, Discount = 2, ViewCount = 650, ReleaseDate = new DateTime(2024, 4, 18, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubValve.Id };
        var fifa = new Game { Id = new Guid("8e7f6d5c-4b3a-4198-a2b1-c3d4e5f6a7b8"), Name = "EA FC 25", Key = "ea-fc-25", Description = "Football simulator", Price = 60.0, UnitInStock = 1000, Discount = 15, ViewCount = 2100, ReleaseDate = new DateTime(2025, 9, 27, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubEa.Id };
        var eldenRing = new Game { Id = new Guid("b2220001-0001-4001-8001-000000000001"), Name = "Elden Ring", Key = "elden-ring", Description = "Open world dark fantasy RPG", Price = 59.99, UnitInStock = 300, Discount = 0, ViewCount = 3500, ReleaseDate = new DateTime(2025, 2, 25, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubBandai.Id };
        var gta5 = new Game { Id = new Guid("b2220002-0002-4002-8002-000000000002"), Name = "Grand Theft Auto V", Key = "gta-v", Description = "Open world action-adventure", Price = 29.99, UnitInStock = 800, Discount = 20, ViewCount = 5000, ReleaseDate = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubRockstar.Id };
        var cyberpunk = new Game { Id = new Guid("b2220003-0003-4003-8003-000000000003"), Name = "Cyberpunk 2077", Key = "cyberpunk-2077", Description = "Futuristic action RPG", Price = 49.99, UnitInStock = 150, Discount = 10, ViewCount = 2800, ReleaseDate = new DateTime(2024, 12, 10, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubCdpr.Id };
        var zelda = new Game { Id = new Guid("b2220004-0004-4004-8004-000000000004"), Name = "Zelda: Tears of the Kingdom", Key = "zelda-totk", Description = "Adventure in Hyrule", Price = 59.99, UnitInStock = 400, Discount = 0, ViewCount = 4200, ReleaseDate = new DateTime(2025, 5, 12, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubNintendo.Id };
        var godOfWar = new Game { Id = new Guid("b2220005-0005-4005-8005-000000000005"), Name = "God of War Ragnarok", Key = "god-of-war-ragnarok", Description = "Norse mythology action", Price = 49.99, UnitInStock = 250, Discount = 5, ViewCount = 3100, ReleaseDate = new DateTime(2024, 11, 9, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubSony.Id };
        var cod = new Game { Id = new Guid("b2220006-0006-4006-8006-000000000006"), Name = "Call of Duty: MW III", Key = "cod-mw3", Description = "Modern military FPS", Price = 69.99, UnitInStock = 600, Discount = 0, ViewCount = 4500, ReleaseDate = new DateTime(2025, 11, 10, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubActivision.Id };
        var ff16 = new Game { Id = new Guid("b2220007-0007-4007-8007-000000000007"), Name = "Final Fantasy XVI", Key = "ff-16", Description = "Action JRPG", Price = 49.99, UnitInStock = 180, Discount = 8, ViewCount = 1800, ReleaseDate = new DateTime(2025, 6, 22, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubSquare.Id };
        var re4 = new Game { Id = new Guid("b2220008-0008-4008-8008-000000000008"), Name = "Resident Evil 4 Remake", Key = "re4-remake", Description = "Survival horror classic", Price = 39.99, UnitInStock = 220, Discount = 12, ViewCount = 2200, ReleaseDate = new DateTime(2025, 3, 24, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubCapcom.Id };
        var spiderman = new Game { Id = new Guid("b2220009-0009-4009-8009-000000000009"), Name = "Spider-Man 2", Key = "spider-man-2", Description = "Superhero action-adventure", Price = 49.99, UnitInStock = 350, Discount = 0, ViewCount = 2900, ReleaseDate = new DateTime(2025, 10, 20, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubInsomniac.Id };
        var destiny2 = new Game { Id = new Guid("b2220010-0010-4010-8010-000000000010"), Name = "Destiny 2", Key = "destiny-2", Description = "Online looter-shooter", Price = 0.0, UnitInStock = 9999, Discount = 0, ViewCount = 3800, ReleaseDate = new DateTime(2024, 2, 28, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubBungie.Id };
        var fortnite = new Game { Id = new Guid("b2220011-0011-4011-8011-000000000011"), Name = "Fortnite", Key = "fortnite", Description = "Battle royale phenomenon", Price = 0.0, UnitInStock = 9999, Discount = 0, ViewCount = 8000, ReleaseDate = new DateTime(2024, 7, 25, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubEpic.Id };
        var tekken8 = new Game { Id = new Guid("b2220012-0012-4012-8012-000000000012"), Name = "Tekken 8", Key = "tekken-8", Description = "3D fighting game", Price = 49.99, UnitInStock = 160, Discount = 0, ViewCount = 1500, ReleaseDate = new DateTime(2026, 1, 26, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubBandai.Id };
        var acValhalla = new Game { Id = new Guid("b2220013-0013-4013-8013-000000000013"), Name = "Assassin's Creed Valhalla", Key = "ac-valhalla", Description = "Viking open-world RPG", Price = 39.99, UnitInStock = 270, Discount = 25, ViewCount = 2000, ReleaseDate = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubUbisoft.Id };
        var csgo = new Game { Id = new Guid("b2220014-0014-4014-8014-000000000014"), Name = "Counter-Strike 2", Key = "cs2", Description = "Competitive tactical FPS", Price = 0.0, UnitInStock = 9999, Discount = 0, ViewCount = 6500, ReleaseDate = new DateTime(2025, 9, 27, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubValve.Id };
        var hades = new Game { Id = new Guid("b2220015-0015-4015-8015-000000000015"), Name = "Hades II", Key = "hades-2", Description = "Roguelike action dungeon crawler", Price = 29.99, UnitInStock = 500, Discount = 0, ViewCount = 3200, ReleaseDate = new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubSupergiant.Id };
        var lastOfUs = new Game { Id = new Guid("b2220016-0016-4016-8016-000000000016"), Name = "The Last of Us Part II", Key = "tlou-2", Description = "Post-apocalyptic action-adventure", Price = 39.99, UnitInStock = 190, Discount = 10, ViewCount = 2600, ReleaseDate = new DateTime(2024, 6, 19, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubNaughtyDog.Id };
        var mgs = new Game { Id = new Guid("b2220017-0017-4017-8017-000000000017"), Name = "Metal Gear Solid V", Key = "mgs-v", Description = "Stealth action", Price = 19.99, UnitInStock = 320, Discount = 30, ViewCount = 1700, ReleaseDate = new DateTime(2023, 9, 1, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubKonami.Id };
        var valorant = new Game { Id = new Guid("b2220018-0018-4018-8018-000000000018"), Name = "Valorant", Key = "valorant", Description = "Tactical hero shooter", Price = 0.0, UnitInStock = 9999, Discount = 0, ViewCount = 7200, ReleaseDate = new DateTime(2024, 6, 2, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubRiot.Id };
        var sonic = new Game { Id = new Guid("b2220019-0019-4019-8019-000000000019"), Name = "Sonic Frontiers", Key = "sonic-frontiers", Description = "Open-zone platformer", Price = 39.99, UnitInStock = 230, Discount = 15, ViewCount = 900, ReleaseDate = new DateTime(2024, 11, 8, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubSega.Id };
        var nba = new Game { Id = new Guid("b2220020-0020-4020-8020-000000000020"), Name = "NBA 2K25", Key = "nba-2k25", Description = "Basketball simulation", Price = 59.99, UnitInStock = 400, Discount = 10, ViewCount = 1400, ReleaseDate = new DateTime(2026, 7, 6, 0, 0, 0, DateTimeKind.Utc), PublisherId = pubTakeTwo.Id };
        modelBuilder.Entity<Game>().HasData(
            halo,
            skyrim,
            witcher3,
            portal2,
            fifa,
            eldenRing,
            gta5,
            cyberpunk,
            zelda,
            godOfWar,
            cod,
            ff16,
            re4,
            spiderman,
            destiny2,
            fortnite,
            tekken8,
            acValhalla,
            csgo,
            hades,
            lastOfUs,
            mgs,
            valorant,
            sonic,
            nba);

        modelBuilder.Entity<GameGenre>().HasData(
            new GameGenre { GameId = halo.Id, GenreId = fps.Id },
            new GameGenre { GameId = halo.Id, GenreId = action.Id },
            new GameGenre { GameId = skyrim.Id, GenreId = rpg.Id },
            new GameGenre { GameId = skyrim.Id, GenreId = adventure.Id },
            new GameGenre { GameId = witcher3.Id, GenreId = rpg.Id },
            new GameGenre { GameId = witcher3.Id, GenreId = action.Id },
            new GameGenre { GameId = witcher3.Id, GenreId = adventure.Id },
            new GameGenre { GameId = portal2.Id, GenreId = puzzle.Id },
            new GameGenre { GameId = fifa.Id, GenreId = sports.Id },
            new GameGenre { GameId = fifa.Id, GenreId = simulator.Id },
            new GameGenre { GameId = eldenRing.Id, GenreId = rpg.Id },
            new GameGenre { GameId = eldenRing.Id, GenreId = action.Id },
            new GameGenre { GameId = gta5.Id, GenreId = action.Id },
            new GameGenre { GameId = gta5.Id, GenreId = adventure.Id },
            new GameGenre { GameId = gta5.Id, GenreId = tps.Id },
            new GameGenre { GameId = cyberpunk.Id, GenreId = rpg.Id },
            new GameGenre { GameId = cyberpunk.Id, GenreId = action.Id },
            new GameGenre { GameId = cyberpunk.Id, GenreId = fps.Id },
            new GameGenre { GameId = zelda.Id, GenreId = adventure.Id },
            new GameGenre { GameId = zelda.Id, GenreId = action.Id },
            new GameGenre { GameId = zelda.Id, GenreId = rpg.Id },
            new GameGenre { GameId = godOfWar.Id, GenreId = action.Id },
            new GameGenre { GameId = godOfWar.Id, GenreId = adventure.Id },
            new GameGenre { GameId = cod.Id, GenreId = fps.Id },
            new GameGenre { GameId = cod.Id, GenreId = action.Id },
            new GameGenre { GameId = ff16.Id, GenreId = rpg.Id },
            new GameGenre { GameId = ff16.Id, GenreId = action.Id },
            new GameGenre { GameId = re4.Id, GenreId = action.Id },
            new GameGenre { GameId = re4.Id, GenreId = tps.Id },
            new GameGenre { GameId = spiderman.Id, GenreId = action.Id },
            new GameGenre { GameId = spiderman.Id, GenreId = adventure.Id },
            new GameGenre { GameId = destiny2.Id, GenreId = fps.Id },
            new GameGenre { GameId = destiny2.Id, GenreId = rpg.Id },
            new GameGenre { GameId = fortnite.Id, GenreId = battleRoyale.Id },
            new GameGenre { GameId = fortnite.Id, GenreId = tps.Id },
            new GameGenre { GameId = tekken8.Id, GenreId = fighting.Id },
            new GameGenre { GameId = acValhalla.Id, GenreId = rpg.Id },
            new GameGenre { GameId = acValhalla.Id, GenreId = action.Id },
            new GameGenre { GameId = acValhalla.Id, GenreId = adventure.Id },
            new GameGenre { GameId = csgo.Id, GenreId = fps.Id },
            new GameGenre { GameId = hades.Id, GenreId = rpg.Id },
            new GameGenre { GameId = hades.Id, GenreId = action.Id },
            new GameGenre { GameId = lastOfUs.Id, GenreId = action.Id },
            new GameGenre { GameId = lastOfUs.Id, GenreId = adventure.Id },
            new GameGenre { GameId = mgs.Id, GenreId = action.Id },
            new GameGenre { GameId = mgs.Id, GenreId = tps.Id },
            new GameGenre { GameId = mgs.Id, GenreId = strategy.Id },
            new GameGenre { GameId = valorant.Id, GenreId = fps.Id },
            new GameGenre { GameId = valorant.Id, GenreId = heroShooter.Id },
            new GameGenre { GameId = sonic.Id, GenreId = arcade.Id },
            new GameGenre { GameId = sonic.Id, GenreId = action.Id },
            new GameGenre { GameId = nba.Id, GenreId = sports.Id },
            new GameGenre { GameId = nba.Id, GenreId = simulator.Id });

        modelBuilder.Entity<GamePlatform>().HasData(
            new GamePlatform { GameId = halo.Id, PlatformId = console.Id },
            new GamePlatform { GameId = halo.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = skyrim.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = skyrim.Id, PlatformId = console.Id },
            new GamePlatform { GameId = witcher3.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = witcher3.Id, PlatformId = console.Id },
            new GamePlatform { GameId = portal2.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = portal2.Id, PlatformId = console.Id },
            new GamePlatform { GameId = fifa.Id, PlatformId = console.Id },
            new GamePlatform { GameId = fifa.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = fifa.Id, PlatformId = mobile.Id },
            new GamePlatform { GameId = eldenRing.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = eldenRing.Id, PlatformId = console.Id },
            new GamePlatform { GameId = gta5.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = gta5.Id, PlatformId = console.Id },
            new GamePlatform { GameId = cyberpunk.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = cyberpunk.Id, PlatformId = console.Id },
            new GamePlatform { GameId = zelda.Id, PlatformId = console.Id },
            new GamePlatform { GameId = godOfWar.Id, PlatformId = console.Id },
            new GamePlatform { GameId = godOfWar.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = cod.Id, PlatformId = console.Id },
            new GamePlatform { GameId = cod.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = ff16.Id, PlatformId = console.Id },
            new GamePlatform { GameId = ff16.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = re4.Id, PlatformId = console.Id },
            new GamePlatform { GameId = re4.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = re4.Id, PlatformId = virtualReality.Id },
            new GamePlatform { GameId = spiderman.Id, PlatformId = console.Id },
            new GamePlatform { GameId = destiny2.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = destiny2.Id, PlatformId = console.Id },
            new GamePlatform { GameId = fortnite.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = fortnite.Id, PlatformId = console.Id },
            new GamePlatform { GameId = fortnite.Id, PlatformId = mobile.Id },
            new GamePlatform { GameId = tekken8.Id, PlatformId = console.Id },
            new GamePlatform { GameId = tekken8.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = acValhalla.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = acValhalla.Id, PlatformId = console.Id },
            new GamePlatform { GameId = csgo.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = hades.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = hades.Id, PlatformId = console.Id },
            new GamePlatform { GameId = lastOfUs.Id, PlatformId = console.Id },
            new GamePlatform { GameId = lastOfUs.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = mgs.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = mgs.Id, PlatformId = console.Id },
            new GamePlatform { GameId = valorant.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = sonic.Id, PlatformId = console.Id },
            new GamePlatform { GameId = sonic.Id, PlatformId = desktop.Id },
            new GamePlatform { GameId = sonic.Id, PlatformId = mobile.Id },
            new GamePlatform { GameId = nba.Id, PlatformId = console.Id },
            new GamePlatform { GameId = nba.Id, PlatformId = desktop.Id });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameGenre>()
            .HasKey(gg => new { gg.GameId, gg.GenreId });

        modelBuilder.Entity<GamePlatform>()
            .HasKey(gp => new { gp.GameId, gp.PlatformId });

        modelBuilder.Entity<OrderGame>()
            .HasKey(og => new { og.OrderId, og.ProductId });

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
        modelBuilder.Entity<OrderGame>()
            .HasOne(og => og.Order)
            .WithMany(o => o.OrderGames)
            .HasForeignKey(og => og.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<OrderGame>()
            .HasOne(og => og.Game)
            .WithMany(g => g.OrderGames)
            .HasForeignKey(og => og.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Game)
            .WithMany(g => g.GameComments)
            .HasForeignKey(c => c.GameId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.ChildComments)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);
        SeedData(modelBuilder);
    }
}