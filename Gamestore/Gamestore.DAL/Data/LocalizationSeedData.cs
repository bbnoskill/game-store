using System.Security.Cryptography;
using System.Text;
using Gamestore.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Data;

public static class LocalizationSeedData
{
    public static void SeedLocalizations(ModelBuilder modelBuilder)
    {
        SeedPlatformLocalizations(modelBuilder);
        SeedGenreLocalizations(modelBuilder);
        SeedPublisherLocalizations(modelBuilder);
        SeedGameLocalizations(modelBuilder);
    }

    private static Guid CreateGuid(string prefix, Guid entityId, string lang)
    {
        var input = $"{prefix}:{entityId}:{lang}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return new Guid(hash.AsSpan(0, 16));
    }

    private static void SeedPlatformLocalizations(ModelBuilder modelBuilder)
    {
        var mobileId = new Guid("e1d6d065-e3df-4c0a-8d46-b44d62d71d4a");
        var desktopId = new Guid("b5d6d065-a1cf-4c0a-9d56-c55d73e82e5b");
        var browserId = new Guid("c7e8f012-b2d0-4d1b-ae67-d66e84f93f6c");
        var consoleId = new Guid("d9f9a123-c3e1-4e2c-bf78-e77f95a04a7d");
        var vrId = new Guid("f0a0b234-d4f2-4f3d-c089-f88a06b15b8e");

        var platforms = new List<PlatformLocalization>
        {
            // Mobile
            new() { Id = CreateGuid("plat", mobileId, "en"), PlatformId = mobileId, LanguageCode = "en", Type = "Mobile" },
            new() { Id = CreateGuid("plat", mobileId, "uk"), PlatformId = mobileId, LanguageCode = "uk", Type = "Мобільні" },
            new() { Id = CreateGuid("plat", mobileId, "ja"), PlatformId = mobileId, LanguageCode = "ja", Type = "モバイル" },

            // Desktop
            new() { Id = CreateGuid("plat", desktopId, "en"), PlatformId = desktopId, LanguageCode = "en", Type = "Desktop" },
            new() { Id = CreateGuid("plat", desktopId, "uk"), PlatformId = desktopId, LanguageCode = "uk", Type = "ПК (Десктоп)" },
            new() { Id = CreateGuid("plat", desktopId, "ja"), PlatformId = desktopId, LanguageCode = "ja", Type = "デスクトップ" },

            // Browser
            new() { Id = CreateGuid("plat", browserId, "en"), PlatformId = browserId, LanguageCode = "en", Type = "Browser" },
            new() { Id = CreateGuid("plat", browserId, "uk"), PlatformId = browserId, LanguageCode = "uk", Type = "Браузерні" },
            new() { Id = CreateGuid("plat", browserId, "ja"), PlatformId = browserId, LanguageCode = "ja", Type = "ブラウザ" },

            // Console
            new() { Id = CreateGuid("plat", consoleId, "en"), PlatformId = consoleId, LanguageCode = "en", Type = "Console" },
            new() { Id = CreateGuid("plat", consoleId, "uk"), PlatformId = consoleId, LanguageCode = "uk", Type = "Консолі" },
            new() { Id = CreateGuid("plat", consoleId, "ja"), PlatformId = consoleId, LanguageCode = "ja", Type = "コンソール" },

            // VR
            new() { Id = CreateGuid("plat", vrId, "en"), PlatformId = vrId, LanguageCode = "en", Type = "VR" },
            new() { Id = CreateGuid("plat", vrId, "uk"), PlatformId = vrId, LanguageCode = "uk", Type = "Віртуальна реальність (VR)" },
            new() { Id = CreateGuid("plat", vrId, "ja"), PlatformId = vrId, LanguageCode = "ja", Type = "VR" },
        };

        modelBuilder.Entity<PlatformLocalization>().HasData(platforms);
    }

    private static void SeedGenreLocalizations(ModelBuilder modelBuilder)
    {
        var strategy = new Guid("a1b2c3d4-e5f6-4a7b-8c9d-e0f1a2b3c4d5");
        var rts = new Guid("b2c3d4e5-f6a7-4b8c-9d0e-f1a2b3c4d5e6");
        var tbs = new Guid("c3d4e5f6-a7b8-4c9d-0e1f-a2b3c4d5e6f7");
        var rpg = new Guid("d4e5f6a7-b8c9-4d0e-1f2a-b3c4d5e6f7a8");
        var simulator = new Guid("e5f6a7b8-c9d0-4e1f-2a3b-c4d5e6f7a8b9");
        var sports = new Guid("f6a7b8c9-d0e1-4f2a-3b4c-d5e6f7a8b9c0");
        var races = new Guid("a7b8c9d0-e1f2-4a3b-4c5d-e6f7a8b9c0d1");
        var rally = new Guid("b8c9d0e1-f2a3-4b4c-5d6e-f7a8b9c0d1e2");
        var arcade = new Guid("c9d0e1f2-a3b4-4c5d-6e7f-a8b9c0d1e2f3");
        var formula = new Guid("d0e1f2a3-b4c5-4d6e-7f8a-b9c0d1e2f3a4");
        var offroad = new Guid("e1f2a3b4-c5d6-4e7f-8a9b-c0d1e2f3a4b5");
        var action = new Guid("f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6");
        var fps = new Guid("a3b4c5d6-e7f8-4a9b-0c1d-e2f3a4b5c6d7");
        var tps = new Guid("b4c5d6e7-f8a9-4b0c-1d2e-f3a4b5c6d7e8");
        var fighting = new Guid("c5d6e7f8-a9b0-4c1d-2e3f-a4b5c6d7e8f9");
        var battleRoyale = new Guid("d6e7f8a9-b0c1-4d2e-3f4a-b5c6d7e8f9a0");
        var heroShooter = new Guid("e7f8a9b0-c1d2-4e3f-4a5b-c6d7e8f9a0b1");
        var adventure = new Guid("f8a9b0c1-d2e3-4f4a-5b6c-d7e8f9a0b1c2");
        var puzzle = new Guid("a9b0c1d2-e3f4-4a5b-6c7d-e8f9a0b1c2d3");

        var genres = new List<GenreLocalization>
        {
            // Strategy
            new() { Id = CreateGuid("gen", strategy, "en"), GenreId = strategy, LanguageCode = "en", Name = "Strategy" },
            new() { Id = CreateGuid("gen", strategy, "uk"), GenreId = strategy, LanguageCode = "uk", Name = "Стратегії" },
            new() { Id = CreateGuid("gen", strategy, "ja"), GenreId = strategy, LanguageCode = "ja", Name = "ストラテジー" },

            // RTS
            new() { Id = CreateGuid("gen", rts, "en"), GenreId = rts, LanguageCode = "en", Name = "RTS" },
            new() { Id = CreateGuid("gen", rts, "uk"), GenreId = rts, LanguageCode = "uk", Name = "RTS (Стратегії в реальному часі)" },
            new() { Id = CreateGuid("gen", rts, "ja"), GenreId = rts, LanguageCode = "ja", Name = "リアルタイムストラテジー" },

            // TBS
            new() { Id = CreateGuid("gen", tbs, "en"), GenreId = tbs, LanguageCode = "en", Name = "TBS" },
            new() { Id = CreateGuid("gen", tbs, "uk"), GenreId = tbs, LanguageCode = "uk", Name = "TBS (Покрокові стратегії)" },
            new() { Id = CreateGuid("gen", tbs, "ja"), GenreId = tbs, LanguageCode = "ja", Name = "ターン制ストラテジー" },

            // RPG
            new() { Id = CreateGuid("gen", rpg, "en"), GenreId = rpg, LanguageCode = "en", Name = "RPG" },
            new() { Id = CreateGuid("gen", rpg, "uk"), GenreId = rpg, LanguageCode = "uk", Name = "Рольові ігри (RPG)" },
            new() { Id = CreateGuid("gen", rpg, "ja"), GenreId = rpg, LanguageCode = "ja", Name = "RPG" },

            // Simulator
            new() { Id = CreateGuid("gen", simulator, "en"), GenreId = simulator, LanguageCode = "en", Name = "Simulator" },
            new() { Id = CreateGuid("gen", simulator, "uk"), GenreId = simulator, LanguageCode = "uk", Name = "Симулятори" },
            new() { Id = CreateGuid("gen", simulator, "ja"), GenreId = simulator, LanguageCode = "ja", Name = "シミュレーター" },

            // Sports
            new() { Id = CreateGuid("gen", sports, "en"), GenreId = sports, LanguageCode = "en", Name = "Sports" },
            new() { Id = CreateGuid("gen", sports, "uk"), GenreId = sports, LanguageCode = "uk", Name = "Спортивні ігри" },
            new() { Id = CreateGuid("gen", sports, "ja"), GenreId = sports, LanguageCode = "ja", Name = "スポーツ" },

            // Races
            new() { Id = CreateGuid("gen", races, "en"), GenreId = races, LanguageCode = "en", Name = "Races" },
            new() { Id = CreateGuid("gen", races, "uk"), GenreId = races, LanguageCode = "uk", Name = "Гонки" },
            new() { Id = CreateGuid("gen", races, "ja"), GenreId = races, LanguageCode = "ja", Name = "レース" },

            // Rally
            new() { Id = CreateGuid("gen", rally, "en"), GenreId = rally, LanguageCode = "en", Name = "Rally" },
            new() { Id = CreateGuid("gen", rally, "uk"), GenreId = rally, LanguageCode = "uk", Name = "Ралі" },
            new() { Id = CreateGuid("gen", rally, "ja"), GenreId = rally, LanguageCode = "ja", Name = "ラリー" },

            // Arcade
            new() { Id = CreateGuid("gen", arcade, "en"), GenreId = arcade, LanguageCode = "en", Name = "Arcade" },
            new() { Id = CreateGuid("gen", arcade, "uk"), GenreId = arcade, LanguageCode = "uk", Name = "Аркади" },
            new() { Id = CreateGuid("gen", arcade, "ja"), GenreId = arcade, LanguageCode = "ja", Name = "アーケード" },

            // Formula
            new() { Id = CreateGuid("gen", formula, "en"), GenreId = formula, LanguageCode = "en", Name = "Formula" },
            new() { Id = CreateGuid("gen", formula, "uk"), GenreId = formula, LanguageCode = "uk", Name = "Формула" },
            new() { Id = CreateGuid("gen", formula, "ja"), GenreId = formula, LanguageCode = "ja", Name = "フォーミュラ" },

            // Off-road
            new() { Id = CreateGuid("gen", offroad, "en"), GenreId = offroad, LanguageCode = "en", Name = "Off-road" },
            new() { Id = CreateGuid("gen", offroad, "uk"), GenreId = offroad, LanguageCode = "uk", Name = "Бездоріжжя" },
            new() { Id = CreateGuid("gen", offroad, "ja"), GenreId = offroad, LanguageCode = "ja", Name = "オフロード" },

            // Action
            new() { Id = CreateGuid("gen", action, "en"), GenreId = action, LanguageCode = "en", Name = "Action" },
            new() { Id = CreateGuid("gen", action, "uk"), GenreId = action, LanguageCode = "uk", Name = "Екшн" },
            new() { Id = CreateGuid("gen", action, "ja"), GenreId = action, LanguageCode = "ja", Name = "アクション" },

            // FPS
            new() { Id = CreateGuid("gen", fps, "en"), GenreId = fps, LanguageCode = "en", Name = "FPS" },
            new() { Id = CreateGuid("gen", fps, "uk"), GenreId = fps, LanguageCode = "uk", Name = "Шутери від першої особи (FPS)" },
            new() { Id = CreateGuid("gen", fps, "ja"), GenreId = fps, LanguageCode = "ja", Name = "ファーストパーソン・シューティング" },

            // TPS
            new() { Id = CreateGuid("gen", tps, "en"), GenreId = tps, LanguageCode = "en", Name = "TPS" },
            new() { Id = CreateGuid("gen", tps, "uk"), GenreId = tps, LanguageCode = "uk", Name = "Шутери від третьої особи (TPS)" },
            new() { Id = CreateGuid("gen", tps, "ja"), GenreId = tps, LanguageCode = "ja", Name = "サードパーソン・シューティング" },

            // Fighting
            new() { Id = CreateGuid("gen", fighting, "en"), GenreId = fighting, LanguageCode = "en", Name = "Fighting" },
            new() { Id = CreateGuid("gen", fighting, "uk"), GenreId = fighting, LanguageCode = "uk", Name = "Файтинги" },
            new() { Id = CreateGuid("gen", fighting, "ja"), GenreId = fighting, LanguageCode = "ja", Name = "対戦型格闘ゲーム" },

            // Battle Royale
            new() { Id = CreateGuid("gen", battleRoyale, "en"), GenreId = battleRoyale, LanguageCode = "en", Name = "Battle Royale" },
            new() { Id = CreateGuid("gen", battleRoyale, "uk"), GenreId = battleRoyale, LanguageCode = "uk", Name = "Королівська битва" },
            new() { Id = CreateGuid("gen", battleRoyale, "ja"), GenreId = battleRoyale, LanguageCode = "ja", Name = "バトルロイヤル" },

            // Hero Shooter
            new() { Id = CreateGuid("gen", heroShooter, "en"), GenreId = heroShooter, LanguageCode = "en", Name = "Hero Shooter" },
            new() { Id = CreateGuid("gen", heroShooter, "uk"), GenreId = heroShooter, LanguageCode = "uk", Name = "Геройський шутер" },
            new() { Id = CreateGuid("gen", heroShooter, "ja"), GenreId = heroShooter, LanguageCode = "ja", Name = "ヒーローシューター" },

            // Adventure
            new() { Id = CreateGuid("gen", adventure, "en"), GenreId = adventure, LanguageCode = "en", Name = "Adventure" },
            new() { Id = CreateGuid("gen", adventure, "uk"), GenreId = adventure, LanguageCode = "uk", Name = "Пригоди" },
            new() { Id = CreateGuid("gen", adventure, "ja"), GenreId = adventure, LanguageCode = "ja", Name = "アドベンチャー" },

            // Puzzle
            new() { Id = CreateGuid("gen", puzzle, "en"), GenreId = puzzle, LanguageCode = "en", Name = "Puzzle & Skill" },
            new() { Id = CreateGuid("gen", puzzle, "uk"), GenreId = puzzle, LanguageCode = "uk", Name = "Головоломки та майстерність" },
            new() { Id = CreateGuid("gen", puzzle, "ja"), GenreId = puzzle, LanguageCode = "ja", Name = "パズル＆スキル" },
        };

        modelBuilder.Entity<GenreLocalization>().HasData(genres);
    }

    private static void SeedPublisherLocalizations(ModelBuilder modelBuilder)
    {
        var rawPublishers = new (Guid Id, string EnName, string EnDesc, string UkName, string UkDesc, string JaName, string JaDesc)[]
        {
            (new Guid("77c06fc6-f8a1-46f9-af03-2793e500112c"), "Electronic Arts", "American video game company", "Electronic Arts", "Американська компанія з розробки відеоігор", "エレクトロニック・アーツ", "アメリカの世界的ゲーム会社"),
            (new Guid("0f7f61ac-babe-423d-a6fe-a5a6c842eacd"), "Test Name", "Some text", "Тестова назва", "Якийсь текст", "テスト名", "説明テキスト"),
            (new Guid("4ca4f3b7-7e23-45f6-8c43-bcde64a51fac"), "Valve", "Creators of Steam and Half-Life", "Valve", "Творці платформи Steam та Half-Life", "Valve", "SteamおよびHalf-Lifeの開発元"),
            (new Guid("5bc5d6a2-6f91-4d3f-91ab-64213d8e578c"), "Bethesda", "Makers of Skyrim and Fallout", "Bethesda", "Творці Skyrim та серії Fallout", "ベセスダ", "スカイリムやフォールアウトの開発元"),
            (new Guid("1c3a64b9-9142-4f3b-823c-7c4a1653d8a1"), "CD Projekt Red", "Witcher and Cyberpunk developers", "CD Projekt Red", "Розробники серій Відьмак та Cyberpunk 2077", "CD Projekt Red", "ウィッチャーやサイバーパンク2077の開発元"),
            (new Guid("a1110001-0001-4001-8001-000000000001"), "Nintendo", "Japanese gaming giant", "Nintendo", "Японський ігровий гігант", "任天堂", "日本の世界的ゲーム開発・販売会社"),
            (new Guid("a1110002-0002-4002-8002-000000000002"), "Sony Interactive", "PlayStation maker", "Sony Interactive", "Творець консолей PlayStation", "ソニー・インタラクティブエンタテインメント", "PlayStationのメーカー"),
            (new Guid("a1110003-0003-4003-8003-000000000003"), "Ubisoft", "French publisher of Assassin's Creed", "Ubisoft", "Французький видавець Assassin's Creed", "ユービーアイソフト", "アサシンクリードなどのパブリッシャー"),
            (new Guid("a1110004-0004-4004-8004-000000000004"), "Activision", "Call of Duty publisher", "Activision", "Видавець Call of Duty", "アクティビジョン", "コールオブデューティのパブリッシャー"),
            (new Guid("a1110005-0005-4005-8005-000000000005"), "Square Enix", "Final Fantasy developers", "Square Enix", "Розробники культової Final Fantasy", "スクウェア・エニックス", "ファイナルファンタジーの開発・発売元"),
            (new Guid("a1110006-0006-4006-8006-000000000006"), "Capcom", "Resident Evil and Monster Hunter", "Capcom", "Творці Resident Evil та Monster Hunter", "カプコン", "バイオハザードやモンスターハンターの開発元"),
            (new Guid("a1110007-0007-4007-8007-000000000007"), "Rockstar Games", "GTA and Red Dead developers", "Rockstar Games", "Розробники GTA та Red Dead Redemption", "ロックスター・ゲームス", "GTAやレッドデッドの開発元"),
            (new Guid("a1110008-0008-4008-8008-000000000008"), "FromSoftware", "Dark Souls and Elden Ring creators", "FromSoftware", "Творці Dark Souls та Elden Ring", "フロム・ソフトウェア", "ダークソウルやエルデンリングの開発会社"),
            (new Guid("a1110009-0009-4009-8009-000000000009"), "Bungie", "Destiny developers", "Bungie", "Розробники Destiny", "バンジー", "Destinyシリーズの開発元"),
            (new Guid("a1110010-0010-4010-8010-000000000010"), "Epic Games", "Fortnite and Unreal Engine", "Epic Games", "Творці Fortnite та рушія Unreal Engine", "Epic Games", "フォートナイトやUnreal Engineの開発元"),
            (new Guid("a1110011-0011-4011-8011-000000000011"), "Sega", "Sonic and Yakuza publisher", "Sega", "Видавець Sonic та серії Yakuza", "セガ", "ソニックや龍が如くシリーズのパブリッシャー"),
            (new Guid("a1110012-0012-4012-8012-000000000012"), "Bandai Namco", "Tekken and Elden Ring publisher", "Bandai Namco", "Видавець Tekken та Elden Ring", "バンダイナムコエンターテインメント", "鉄拳やエルデンリングのパブリッシャー"),
            (new Guid("a1110013-0013-4013-8013-000000000013"), "Konami", "Metal Gear Solid publisher", "Konami", "Видавець Metal Gear Solid", "コナミ", "メタルギアソリッドのパブリッシャー"),
            (new Guid("a1110014-0014-4014-8014-000000000014"), "Take-Two", "Parent of Rockstar and 2K", "Take-Two", "Материнська компанія Rockstar та 2K", "Take-Two", "Rockstarや2Kの親会社"),
            (new Guid("a1110015-0015-4015-8015-000000000015"), "Xbox Game Studios", "Microsoft gaming division", "Xbox Game Studios", "Ігровий підрозділ корпорації Microsoft", "Xbox Game Studios", "マイクロソフトのゲーム開発部門"),
            (new Guid("a1110016-0016-4016-8016-000000000016"), "Naughty Dog", "The Last of Us and Uncharted", "Naughty Dog", "Творці The Last of Us та Uncharted", "ノーティードッグ", "The Last of Usやアンチャーテッドの開発元"),
            (new Guid("a1110017-0017-4017-8017-000000000017"), "Insomniac Games", "Spider-Man PS developers", "Insomniac Games", "Розробники хітів Marvel's Spider-Man", "インソムニアック・ゲームズ", "Marvel's Spider-Manの開発元"),
            (new Guid("a1110018-0018-4018-8018-000000000018"), "Devolver Digital", "Indie game publisher", "Devolver Digital", "Провідний видавець інді-ігор", "Devolver Digital", "個性派インディーゲームのパブリッシャー"),
            (new Guid("a1110019-0019-4019-8019-000000000019"), "Supergiant Games", "Hades and Bastion creators", "Supergiant Games", "Творці шедеврів Hades та Bastion", "Supergiant Games", "HadesやBastionのクリエイター"),
            (new Guid("a1110020-0020-4020-8020-000000000020"), "Riot Games", "League of Legends developer", "Riot Games", "Розробники League of Legends та Valorant", "ライアットゲームズ", "League of LegendsやVALORANTの開発元"),
        };

        var publishers = new List<PublisherLocalization>();
        foreach (var (id, enName, enDesc, ukName, ukDesc, jaName, jaDesc) in rawPublishers)
        {
            publishers.Add(new PublisherLocalization { Id = CreateGuid("pub", id, "en"), PublisherId = id, LanguageCode = "en", CompanyName = enName, Description = enDesc });
            publishers.Add(new PublisherLocalization { Id = CreateGuid("pub", id, "uk"), PublisherId = id, LanguageCode = "uk", CompanyName = ukName, Description = ukDesc });
            publishers.Add(new PublisherLocalization { Id = CreateGuid("pub", id, "ja"), PublisherId = id, LanguageCode = "ja", CompanyName = jaName, Description = jaDesc });
        }

        modelBuilder.Entity<PublisherLocalization>().HasData(publishers);
    }

    private static void SeedGameLocalizations(ModelBuilder modelBuilder)
    {
        var rawGames = new (Guid Id, string EnName, string EnDesc, string UkName, string UkDesc, string JaName, string JaDesc)[]
        {
            (new Guid("9c5e3d7f-2b1a-4c8d-93e1-38a4d7c6b9f2"), "Halo Infinite", "Sci-fi FPS", "Halo Infinite", "Науково-фантастичний шутер від першої особи", "Halo Infinite", "マスターチーフの戦いを描く名作SFファーストパーソンシューター"),
            (new Guid("4a8b7c2d-9e6f-4315-81a9-d3c2b1f8e7a6"), "Skyrim", "Epic fantasy RPG", "Скайрім", "Легендарна фентезійна рольова гра про Довакіна", "The Elder Scrolls V: Skyrim", "ドラゴンと戦う伝説のオープンワールド・ファンタジーRPG"),
            (new Guid("2f1e9d8c-7b6a-45c3-84d2-f1e3a2b4c5d6"), "The Witcher 3", "Geralt's grand adventure", "Відьмак 3: Дикий Гін", "Велична пригода Ґеральта з Рівії", "ウィッチャー3 ワイルドハント", "リヴィアのゲラルトの壮大な冒険"),
            (new Guid("3a4b5c6d-e7f8-4912-8c7d-6e5f4a3b2c1d"), "Portal 2", "Thinking with portals", "Портал 2", "Геніальна просторова головоломка з порталами", "Portal 2", "ポータル銃を駆使して謎を解く傑作パズルアドベンチャー"),
            (new Guid("8e7f6d5c-4b3a-4198-a2b1-c3d4e5f6a7b8"), "EA FC 25", "Football simulator", "EA FC 25", "Найновіший симулятор футболу", "EA SPORTS FC 25", "世界的人気を誇る最新サッカーシミュレーション"),
            (new Guid("b2220001-0001-4001-8001-000000000001"), "Elden Ring", "Open world dark fantasy RPG", "Елден Рінг", "Рольовий екшн у відкритому світі темного фентезі", "ELDEN RING", "広大なダークファンタジー世界を舞台にしたオープンワールドRPG"),
            (new Guid("b2220002-0002-4002-8002-000000000002"), "Grand Theft Auto V", "Open world action-adventure", "Grand Theft Auto V", "Культовий кримінальний екшн у місті Лос-Сантос", "グランド・セフト・オートV", "ロスサントスを駆け巡る大人気オープンワールドクライムアクション"),
            (new Guid("b2220003-0003-4003-8003-000000000003"), "Cyberpunk 2077", "Futuristic action RPG", "Кіберпанк 2077", "Футуристична рольова гра у відкритому місті Найт-Сіті", "サイバーパンク2077", "未来都市ナイトシティを舞台にしたオープンワールドアクションRPG"),
            (new Guid("b2220004-0004-4004-8004-000000000004"), "Zelda: Tears of the Kingdom", "Adventure in Hyrule", "Зельда: Сльози Королівства", "Неймовірна пригода на землях і в небі Гайрулу", "ゼルダの伝説 ティアーズ オブ ザ キングダム", "ハイラルの大地と大空を巡る冒険"),
            (new Guid("b2220005-0005-4005-8005-000000000005"), "God of War Ragnarok", "Norse mythology action", "Бог Війни: Рагнарок", "Скандинавська епічна пригода Кратоса та Атрея", "ゴッド・オブ・ウォー ラグナロク", "北欧神話を舞台にしたクレイトスとアトレウスの壮大な旅"),
            (new Guid("b2220006-0006-4006-8006-000000000006"), "Call of Duty: MW III", "Modern military FPS", "Call of Duty: MW III", "Військовий тактичний шутер сучасності", "コール オブ デューティ モダン・ウォーフェアIII", "臨場感溢れる現代戦を描くミリタリーFPS"),
            (new Guid("b2220007-0007-4007-8007-000000000007"), "Final Fantasy XVI", "Action JRPG", "Final Fantasy XVI", "Масштабна рольова гра в темному середньовічному фентезі", "ファイナルファンタジーXVI", "重厚なダークファンタジーを舞台にした本格アクションRPG"),
            (new Guid("b2220008-0008-4008-8008-000000000008"), "Resident Evil 4 Remake", "Survival horror classic", "Оселя Зла 4 Ремейк", "Культова класика горору на виживання", "バイオハザード RE:4", "サバイバルホラーの金字塔を最新技術で再構築"),
            (new Guid("b2220009-0009-4009-8009-000000000009"), "Spider-Man 2", "Superhero action-adventure", "Людина-Павук 2", "Супергеройська пригода Пітера Паркера та Майлза Моралеса", "Marvel's Spider-Man 2", "ピーターとマイルズの2人が活躍するオープンワールドアクション"),
            (new Guid("b2220010-0010-4010-8010-000000000010"), "Destiny 2", "Online looter-shooter", "Destiny 2", "Командний онлайн лутер-шутер у космосі", "Destiny 2", "進化を続ける壮大なオンラインマルチプレイ・アクションFPS"),
            (new Guid("b2220011-0011-4011-8011-000000000011"), "Fortnite", "Battle royale phenomenon", "Фортнайт", "Всесвітньо відома королівська битва", "フォートナイト", "世界的大ヒットを記録したバトルロイヤルゲーム"),
            (new Guid("b2220012-0012-4012-8012-000000000012"), "Tekken 8", "3D fighting game", "Теккен 8", "Нова частина легендарного 3D-файтингу", "鉄拳8", "圧倒的な迫力を誇る3D対戦格闘ゲームの最新作"),
            (new Guid("b2220013-0013-4013-8013-000000000013"), "Assassin's Creed Valhalla", "Viking open-world RPG", "Assassin's Creed Вальгалла", "Сага про вікінгів у середньовічній Англії", "アサシン クリード ヴァルハラ", "ヴァイキングの栄光と征服を描くオープンワールドRPG"),
            (new Guid("b2220014-0014-4014-8014-000000000014"), "Counter-Strike 2", "Competitive tactical FPS", "Counter-Strike 2", "Змагальний командний тактичний шутер", "Counter-Strike 2", "最高峰の競技性を誇るタクティカルマルチプレイFPS"),
            (new Guid("b2220015-0015-4015-8015-000000000015"), "Hades II", "Roguelike action dungeon crawler", "Аїд II", "Динамічний рогалик у давньогрецькому підземному царстві", "Hades II", "ギリシャ神話を題材にした大人気ローグライク・アクション"),
            (new Guid("b2220016-0016-4016-8016-000000000016"), "The Last of Us Part II", "Post-apocalyptic action-adventure", "Останні з нас: Частина II", "Драматична пригодницька історія в постапокаліптичному світі", "The Last of Us Part II", "感染者が蠢く過酷な世界を生き抜くサバイバルアクション"),
            (new Guid("b2220017-0017-4017-8017-000000000017"), "Metal Gear Solid V", "Stealth action", "Metal Gear Solid V", "Тактичний шпигунський стелс-екшн", "メタルギア ソリッド V", "究極の自由度を誇るステルスアクションゲーム"),
            (new Guid("b2220018-0018-4018-8018-000000000018"), "Valorant", "Tactical hero shooter", "Валорант", "Тактичний геройський шутер 5 на 5", "VALORANT", "個性豊かなエージェントが激突する5対5のタクティカルシューター"),
            (new Guid("b2220019-0019-4019-8019-000000000019"), "Sonic Frontiers", "Open-zone platformer", "Сонік Фронтірс", "Швидкісний платформер у відкритих зонах", "ソニックフロンティア", "縦横無尽に駆け巡るオープンゾーン・ハイスピードアクション"),
            (new Guid("b2220020-0020-4020-8020-000000000020"), "NBA 2K25", "Basketball simulation", "NBA 2K25", "Реалістичний симулятор баскетболу", "NBA 2K25", "最高峰の臨場感を誇るバスケットボールシミュレーション"),
        };

        var games = new List<GameLocalization>();
        foreach (var (id, enName, enDesc, ukName, ukDesc, jaName, jaDesc) in rawGames)
        {
            games.Add(new GameLocalization { Id = CreateGuid("game", id, "en"), GameId = id, LanguageCode = "en", Name = enName, Description = enDesc });
            games.Add(new GameLocalization { Id = CreateGuid("game", id, "uk"), GameId = id, LanguageCode = "uk", Name = ukName, Description = ukDesc });
            games.Add(new GameLocalization { Id = CreateGuid("game", id, "ja"), GameId = id, LanguageCode = "ja", Name = jaName, Description = jaDesc });
        }

        modelBuilder.Entity<GameLocalization>().HasData(games);
    }
}
