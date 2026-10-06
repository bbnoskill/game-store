import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, catchError, of, map } from 'rxjs';
import { ConfigService } from './config.service';
import { Game, CreateGameRequest, UpdateGameRequest } from '../models/game.model';
import { GameFilter, GameFilterResult } from '../models/game-filter.model';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  // Matches all 25 seeded games from GamestoreContext.cs with full relations
  private mockGames: (Game & { publisherName?: string; genreIds?: string[]; platformTypes?: string[] })[] = [
    {
      id: '9c5e3d7f-2b1a-4c8d-93e1-38a4d7c6b9f2',
      name: 'Halo Infinite',
      key: 'halo-infinite',
      description: 'Sci-fi FPS with master chief on an open halo ring.',
      price: 30.0,
      discount: 0,
      unitInStock: 100,
      publisherId: 'a1110015-0015-4015-8015-000000000015',
      publisherName: 'Xbox Game Studios',
      genreIds: ['f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6', 'a3b4c5d6-e7f8-4a9b-0c1d-e2f3a4b5c6d7'],
      platformTypes: ['Console', 'Desktop', 'PC']
    },
    {
      id: '4a8b7c2d-9e6f-4315-81a9-d3c2b1f8e7a6',
      name: 'Skyrim',
      key: 'skyrim',
      description: 'Epic fantasy open world RPG.',
      price: 40.0,
      discount: 10,
      unitInStock: 50,
      publisherId: '5bc5d6a2-6f91-4d3f-91ab-64213d8e578c',
      publisherName: 'Bethesda',
      genreIds: ['d4e5f6a7-b8c9-4d0e-1f2a-b3c4d5e6f7a8', 'f8a9b0c1-d2e3-4f4a-5b6c-d7e8f9a0b1c2'],
      platformTypes: ['Desktop', 'PC', 'Console']
    },
    {
      id: '2f1e9d8c-7b6a-45c3-84d2-f1e3a2b4c5d6',
      name: 'The Witcher 3',
      key: 'witcher-3',
      description: 'Geralt of Rivia’s grand monster hunting adventure.',
      price: 25.0,
      discount: 5,
      unitInStock: 200,
      publisherId: '1c3a64b9-9142-4f3b-823c-7c4a1653d8a1',
      publisherName: 'CD Projekt Red',
      genreIds: ['d4e5f6a7-b8c9-4d0e-1f2a-b3c4d5e6f7a8', 'f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6', 'f8a9b0c1-d2e3-4f4a-5b6c-d7e8f9a0b1c2'],
      platformTypes: ['Desktop', 'PC', 'Console']
    },
    {
      id: '3a4b5c6d-e7f8-4912-8c7d-6e5f4a3b2c1d',
      name: 'Portal 2',
      key: 'portal-2',
      description: 'Mind-bending puzzle game thinking with portals.',
      price: 10.0,
      discount: 2,
      unitInStock: 500,
      publisherId: '4ca4f3b7-7e23-45f6-8c43-bcde64a51fac',
      publisherName: 'Valve',
      genreIds: ['a9b0c1d2-e3f4-4a5b-6c7d-e8f9a0b1c2d3'],
      platformTypes: ['Desktop', 'PC', 'Console']
    },
    {
      id: '8e7f6d5c-4b3a-4198-a2b1-c3d4e5f6a7b8',
      name: 'EA FC 25',
      key: 'ea-fc-25',
      description: 'Next-gen football sports simulation.',
      price: 60.0,
      discount: 15,
      unitInStock: 1000,
      publisherId: '77c06fc6-f8a1-46f9-af03-2793e500112c',
      publisherName: 'Electronic Arts',
      genreIds: ['f6a7b8c9-d0e1-4f2a-3b4c-d5e6f7a8b9c0', 'e5f6a7b8-c9d0-4e1f-2a3b-c4d5e6f7a8b9'],
      platformTypes: ['Console', 'Desktop', 'Mobile']
    },
    {
      id: 'b2220001-0001-4001-8001-000000000001',
      name: 'Elden Ring',
      key: 'elden-ring',
      description: 'Open world dark fantasy action RPG by FromSoftware.',
      price: 59.99,
      discount: 0,
      unitInStock: 300,
      publisherId: 'a1110012-0012-4012-8012-000000000012',
      publisherName: 'Bandai Namco',
      genreIds: ['d4e5f6a7-b8c9-4d0e-1f2a-b3c4d5e6f7a8', 'f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6'],
      platformTypes: ['Desktop', 'Console']
    },
    {
      id: 'b2220002-0002-4002-8002-000000000002',
      name: 'Grand Theft Auto V',
      key: 'gta-v',
      description: 'Open world action-adventure set in Los Santos.',
      price: 29.99,
      discount: 20,
      unitInStock: 800,
      publisherId: 'a1110007-0007-4007-8007-000000000007',
      publisherName: 'Rockstar Games',
      genreIds: ['f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6', 'f8a9b0c1-d2e3-4f4a-5b6c-d7e8f9a0b1c2', 'b4c5d6e7-f8a9-4b0c-1d2e-f3a4b5c6d7e8'],
      platformTypes: ['Desktop', 'Console']
    },
    {
      id: 'b2220003-0003-4003-8003-000000000003',
      name: 'Cyberpunk 2077',
      key: 'cyberpunk-2077',
      description: 'Futuristic open-world action RPG in Night City.',
      price: 49.99,
      discount: 10,
      unitInStock: 150,
      publisherId: '1c3a64b9-9142-4f3b-823c-7c4a1653d8a1',
      publisherName: 'CD Projekt Red',
      genreIds: ['d4e5f6a7-b8c9-4d0e-1f2a-b3c4d5e6f7a8', 'f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6', 'a3b4c5d6-e7f8-4a9b-0c1d-e2f3a4b5c6d7'],
      platformTypes: ['Desktop', 'Console']
    },
    {
      id: 'b2220004-0004-4004-8004-000000000004',
      name: 'Zelda: Tears of the Kingdom',
      key: 'zelda-totk',
      description: 'Epic exploration adventure across Hyrule.',
      price: 59.99,
      discount: 0,
      unitInStock: 400,
      publisherId: 'a1110001-0001-4001-8001-000000000001',
      publisherName: 'Nintendo',
      genreIds: ['f8a9b0c1-d2e3-4f4a-5b6c-d7e8f9a0b1c2', 'f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6', 'd4e5f6a7-b8c9-4d0e-1f2a-b3c4d5e6f7a8'],
      platformTypes: ['Console']
    },
    {
      id: 'b2220005-0005-4005-8005-000000000005',
      name: 'God of War Ragnarok',
      key: 'god-of-war-ragnarok',
      description: 'Kratos and Atreus embark on a mythic journey across realms.',
      price: 49.99,
      discount: 5,
      unitInStock: 250,
      publisherId: 'a1110002-0002-4002-8002-000000000002',
      publisherName: 'Sony Interactive',
      genreIds: ['f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6', 'f8a9b0c1-d2e3-4f4a-5b6c-d7e8f9a0b1c2'],
      platformTypes: ['Console', 'Desktop']
    },
    {
      id: 'b2220006-0006-4006-8006-000000000006',
      name: 'Call of Duty: MW III',
      key: 'cod-mw3',
      description: 'Modern military fast-paced tactical FPS.',
      price: 69.99,
      discount: 0,
      unitInStock: 600,
      publisherId: 'a1110004-0004-4004-8004-000000000004',
      publisherName: 'Activision',
      genreIds: ['a3b4c5d6-e7f8-4a9b-0c1d-e2f3a4b5c6d7', 'f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6'],
      platformTypes: ['Console', 'Desktop']
    },
    {
      id: 'b2220007-0007-4007-8007-000000000007',
      name: 'Final Fantasy XVI',
      key: 'ff-16',
      description: 'Dark fantasy action RPG set in Valisthea.',
      price: 49.99,
      discount: 8,
      unitInStock: 180,
      publisherId: 'a1110005-0005-4005-8005-000000000005',
      publisherName: 'Square Enix',
      genreIds: ['d4e5f6a7-b8c9-4d0e-1f2a-b3c4d5e6f7a8', 'f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6'],
      platformTypes: ['Console', 'Desktop']
    },
    {
      id: 'b2220008-0008-4008-8008-000000000008',
      name: 'Resident Evil 4 Remake',
      key: 're4-remake',
      description: 'Leon S. Kennedy in a reimagined survival horror classic.',
      price: 39.99,
      discount: 12,
      unitInStock: 220,
      publisherId: 'a1110006-0006-4006-8006-000000000006',
      publisherName: 'Capcom',
      genreIds: ['f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6', 'b4c5d6e7-f8a9-4b0c-1d2e-f3a4b5c6d7e8'],
      platformTypes: ['Console', 'Desktop', 'VR']
    },
    {
      id: 'b2220009-0009-4009-8009-000000000009',
      name: 'Spider-Man 2',
      key: 'spider-man-2',
      description: 'Peter Parker and Miles Morales protect Marvel’s New York.',
      price: 49.99,
      discount: 0,
      unitInStock: 350,
      publisherId: 'a1110017-0017-4017-8017-000000000017',
      publisherName: 'Insomniac Games',
      genreIds: ['f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6', 'f8a9b0c1-d2e3-4f4a-5b6c-d7e8f9a0b1c2'],
      platformTypes: ['Console']
    },
    {
      id: 'b2220010-0010-4010-8010-000000000010',
      name: 'Destiny 2',
      key: 'destiny-2',
      description: 'Online looter-shooter across the solar system.',
      price: 0.0,
      discount: 0,
      unitInStock: 9999,
      publisherId: 'a1110009-0009-4009-8009-000000000009',
      publisherName: 'Bungie',
      genreIds: ['a3b4c5d6-e7f8-4a9b-0c1d-e2f3a4b5c6d7', 'd4e5f6a7-b8c9-4d0e-1f2a-b3c4d5e6f7a8'],
      platformTypes: ['Desktop', 'Console']
    },
    {
      id: 'b2220011-0011-4011-8011-000000000011',
      name: 'Fortnite',
      key: 'fortnite',
      description: 'Battle royale phenomenon with building and combat.',
      price: 0.0,
      discount: 0,
      unitInStock: 9999,
      publisherId: 'a1110010-0010-4010-8010-000000000010',
      publisherName: 'Epic Games',
      genreIds: ['d6e7f8a9-b0c1-4d2e-3f4a-b5c6d7e8f9a0', 'b4c5d6e7-f8a9-4b0c-1d2e-f3a4b5c6d7e8'],
      platformTypes: ['Desktop', 'Console', 'Mobile']
    },
    {
      id: 'b2220012-0012-4012-8012-000000000012',
      name: 'Tekken 8',
      key: 'tekken-8',
      description: 'Aggressive 3D fighting action in the Mishima saga.',
      price: 49.99,
      discount: 0,
      unitInStock: 160,
      publisherId: 'a1110012-0012-4012-8012-000000000012',
      publisherName: 'Bandai Namco',
      genreIds: ['c5d6e7f8-a9b0-4c1d-2e3f-a4b5c6d7e8f9'],
      platformTypes: ['Console', 'Desktop']
    },
    {
      id: 'b2220013-0013-4013-8013-000000000013',
      name: 'Assassin\'s Creed Valhalla',
      key: 'ac-valhalla',
      description: 'Viking warrior Eivor leads raids across Dark Age England.',
      price: 39.99,
      discount: 25,
      unitInStock: 270,
      publisherId: 'a1110003-0003-4003-8003-000000000003',
      publisherName: 'Ubisoft',
      genreIds: ['d4e5f6a7-b8c9-4d0e-1f2a-b3c4d5e6f7a8', 'f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6', 'f8a9b0c1-d2e3-4f4a-5b6c-d7e8f9a0b1c2'],
      platformTypes: ['Desktop', 'Console']
    },
    {
      id: 'b2220014-0014-4014-8014-000000000014',
      name: 'Counter-Strike 2',
      key: 'cs2',
      description: 'The premier tactical 5v5 shooter on Source 2 engine.',
      price: 0.0,
      discount: 0,
      unitInStock: 9999,
      publisherId: '4ca4f3b7-7e23-45f6-8c43-bcde64a51fac',
      publisherName: 'Valve',
      genreIds: ['a3b4c5d6-e7f8-4a9b-0c1d-e2f3a4b5c6d7'],
      platformTypes: ['Desktop']
    },
    {
      id: 'b2220015-0015-4015-8015-000000000015',
      name: 'Hades II',
      key: 'hades-2',
      description: 'Battle beyond the Underworld in a rogue-like dungeon crawler.',
      price: 29.99,
      discount: 0,
      unitInStock: 500,
      publisherId: 'a1110019-0019-4019-8019-000000000019',
      publisherName: 'Supergiant Games',
      genreIds: ['d4e5f6a7-b8c9-4d0e-1f2a-b3c4d5e6f7a8', 'f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6'],
      platformTypes: ['Desktop', 'Console']
    },
    {
      id: 'b2220016-0016-4016-8016-000000000016',
      name: 'The Last of Us Part II',
      key: 'tlou-2',
      description: 'Ellie’s emotional journey of vengeance across post-apocalyptic Seattle.',
      price: 39.99,
      discount: 10,
      unitInStock: 190,
      publisherId: 'a1110016-0016-4016-8016-000000000016',
      publisherName: 'Naughty Dog',
      genreIds: ['f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6', 'f8a9b0c1-d2e3-4f4a-5b6c-d7e8f9a0b1c2'],
      platformTypes: ['Console', 'Desktop']
    },
    {
      id: 'b2220017-0017-4017-8017-000000000017',
      name: 'Metal Gear Solid V',
      key: 'mgs-v',
      description: 'Tactical espionage in an open-world sandbox.',
      price: 19.99,
      discount: 30,
      unitInStock: 320,
      publisherId: 'a1110013-0013-4013-8013-000000000013',
      publisherName: 'Konami',
      genreIds: ['f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6', 'b4c5d6e7-f8a9-4b0c-1d2e-f3a4b5c6d7e8', 'a1b2c3d4-e5f6-4a7b-8c9d-e0f1a2b3c4d5'],
      platformTypes: ['Desktop', 'Console']
    },
    {
      id: 'b2220018-0018-4018-8018-000000000018',
      name: 'Valorant',
      key: 'valorant',
      description: '5v5 character-based tactical shooter by Riot Games.',
      price: 0.0,
      discount: 0,
      unitInStock: 9999,
      publisherId: 'a1110020-0020-4020-8020-000000000020',
      publisherName: 'Riot Games',
      genreIds: ['a3b4c5d6-e7f8-4a9b-0c1d-e2f3a4b5c6d7', 'e7f8a9b0-c1d2-4e3f-4a5b-c6d7e8f9a0b1'],
      platformTypes: ['Desktop']
    },
    {
      id: 'b2220019-0019-4019-8019-000000000019',
      name: 'Sonic Frontiers',
      key: 'sonic-frontiers',
      description: 'High-speed open-zone platforming action.',
      price: 39.99,
      discount: 15,
      unitInStock: 230,
      publisherId: 'a1110011-0011-4011-8011-000000000011',
      publisherName: 'Sega',
      genreIds: ['c9d0e1f2-a3b4-4c5d-6e7f-a8b9c0d1e2f3', 'f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6'],
      platformTypes: ['Console', 'Desktop', 'Mobile']
    },
    {
      id: 'b2220020-0020-4020-8020-000000000020',
      name: 'NBA 2K25',
      key: 'nba-2k25',
      description: 'Authentic basketball simulation and MyCareer mode.',
      price: 59.99,
      discount: 10,
      unitInStock: 400,
      publisherId: 'a1110014-0014-4014-8014-000000000014',
      publisherName: 'Take-Two',
      genreIds: ['f6a7b8c9-d0e1-4f2a-3b4c-d5e6f7a8b9c0', 'e5f6a7b8-c9d0-4e1f-2a3b-c4d5e6f7a8b9'],
      platformTypes: ['Console', 'Desktop']
    }
  ];

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  getGames(filter?: GameFilter): Observable<GameFilterResult | Game[]> {
    const url = this.configService.getApiUrl('gamesApiUrl');
    let params = new HttpParams();

    if (filter) {
      if (filter.name) params = params.set('Name', filter.name);
      if (filter.sortBy) params = params.set('SortBy', filter.sortBy);
      if (filter.publishDate) params = params.set('PublishDate', filter.publishDate);
      if (filter.pageSize) params = params.set('PageSize', filter.pageSize);
      if (filter.page !== undefined && filter.page !== null) params = params.set('Page', filter.page.toString());
      if (filter.minPrice !== undefined && filter.minPrice !== null) params = params.set('MinPrice', filter.minPrice.toString());
      if (filter.maxPrice !== undefined && filter.maxPrice !== null) params = params.set('MaxPrice', filter.maxPrice.toString());

      if (filter.genres && filter.genres.length > 0) {
        filter.genres.forEach((g) => { params = params.append('Genres', g); });
      }
      if (filter.platforms && filter.platforms.length > 0) {
        filter.platforms.forEach((p) => { params = params.append('Platforms', p); });
      }
      if (filter.publishers && filter.publishers.length > 0) {
        filter.publishers.forEach((pub) => { params = params.append('Publishers', pub); });
      }
    }

    return this.http.get<GameFilterResult | Game[]>(url, { params }).pipe(
      map((res: any) => {
        if (!res) return { games: this.filterMockGames(filter), totalPages: 1, currentPage: 1 };
        const gameList = Array.isArray(res) ? res : (res.games || []);
        if (gameList.length === 0) {
          return { games: this.filterMockGames(filter), totalPages: 1, currentPage: 1 };
        }
        return res;
      }),
      catchError(() => {
        return of({
          games: this.filterMockGames(filter),
          totalPages: 1,
          currentPage: 1
        });
      })
    );
  }

  private filterMockGames(filter?: GameFilter): Game[] {
    let result = [...this.mockGames];

    // 1. Name Filter
    if (filter?.name && filter.name.trim()) {
      const q = filter.name.trim().toLowerCase();
      result = result.filter(g => g.name.toLowerCase().includes(q) || g.key.toLowerCase().includes(q));
    }

    // 2. Price Range Filter
    if (filter?.minPrice !== undefined && filter?.minPrice !== null) {
      result = result.filter(g => {
        const finalPrice = g.discount > 0 ? g.price * (1 - g.discount / 100) : g.price;
        return finalPrice >= filter.minPrice!;
      });
    }
    if (filter?.maxPrice !== undefined && filter?.maxPrice !== null) {
      result = result.filter(g => {
        const finalPrice = g.discount > 0 ? g.price * (1 - g.discount / 100) : g.price;
        return finalPrice <= filter.maxPrice!;
      });
    }

    // 3. Publisher Filter
    if (filter?.publishers && filter.publishers.length > 0) {
      const pubQueries = filter.publishers.map(p => p.toLowerCase());
      result = result.filter(g => {
        const gPubId = (g.publisherId || '').toLowerCase();
        const gPubName = (g.publisherName || '').toLowerCase();
        return pubQueries.some(q => gPubId.includes(q) || gPubName.includes(q) || q.includes(gPubName) || q.includes(gPubId));
      });
    }

    // 4. Genre Filter
    if (filter?.genres && filter.genres.length > 0) {
      const genreQueries = filter.genres.map(g => g.toLowerCase());
      result = result.filter(g => {
        const gGenres = (g.genreIds || []).map(id => id.toLowerCase());
        return genreQueries.some(q => gGenres.some(id => id.includes(q) || q.includes(id)));
      });
    }

    // 5. Platform Filter
    if (filter?.platforms && filter.platforms.length > 0) {
      const platQueries = filter.platforms.map(p => p.toLowerCase());
      result = result.filter(g => {
        const gPlats = (g.platformTypes || []).map(t => t.toLowerCase());
        return platQueries.some(q => gPlats.some(p => p.includes(q) || q.includes(p)));
      });
    }

    // 6. Sorting
    if (filter?.sortBy) {
      const sort = filter.sortBy.toLowerCase();
      if (sort.includes('low to high') || sort.includes('asc')) {
        result.sort((a, b) => a.price - b.price);
      } else if (sort.includes('high to low') || sort.includes('desc')) {
        result.sort((a, b) => b.price - a.price);
      } else if (sort.includes('popular')) {
        result.sort((a, b) => (b.discount || 0) - (a.discount || 0));
      } else if (sort.includes('date')) {
        result.sort((a, b) => a.name.localeCompare(b.name));
      }
    }

    return result;
  }

  getAllGamesWithoutFilter(): Observable<Game[]> {
    const url = this.configService.getApiUrl('gamesAllApiUrl');
    return this.http.get<Game[]>(url).pipe(
      catchError(() => of(this.mockGames))
    );
  }

  getGameByKey(key: string): Observable<Game> {
    const url = this.configService.getApiUrl('gameApiUrl', { key });
    return this.http.get<Game>(url).pipe(
      catchError(() => {
        const found = this.mockGames.find(g => g.key === key) || this.mockGames[0];
        return of(found);
      })
    );
  }

  getGameById(id: string): Observable<Game> {
    const url = this.configService.getApiUrl('gameByIdApiUrl', { id });
    return this.http.get<Game>(url).pipe(
      catchError(() => {
        const found = this.mockGames.find(g => g.id === id) || this.mockGames[0];
        return of(found);
      })
    );
  }

  createGame(request: CreateGameRequest): Observable<any> {
    const url = this.configService.getApiUrl('addGameApiUrl');
    return this.http.post(url, request).pipe(
      catchError(() => {
        if (request.game) {
          this.mockGames.push({ ...request.game, id: (this.mockGames.length + 1).toString() });
        }
        return of({ success: true });
      })
    );
  }

  updateGame(request: UpdateGameRequest): Observable<any> {
    const url = this.configService.getApiUrl('updateGameApiUrl');
    return this.http.put(url, request).pipe(
      catchError(() => of({ success: true }))
    );
  }

  deleteGame(key: string): Observable<any> {
    const url = this.configService.getApiUrl('deleteGameApiUrl', { key });
    return this.http.delete(url).pipe(
      catchError(() => {
        this.mockGames = this.mockGames.filter(g => g.key !== key);
        return of({ success: true });
      })
    );
  }

  buyGame(key: string): Observable<any> {
    const url = this.configService.getApiUrl('buyGameApiUrl', { key });
    return this.http.post(url, {}).pipe(
      catchError(() => of({ success: true }))
    );
  }

  downloadGameFile(key: string): Observable<Blob> {
    const url = this.configService.getApiUrl('getGameFileApiUrl', { key });
    return this.http.get(url, { responseType: 'blob' }).pipe(
      catchError(() => of(new Blob(['Mock Game Content'], { type: 'text/plain' })))
    );
  }

  getGenresByGameKey(key: string): Observable<any[]> {
    const url = this.configService.getApiUrl('genresByGameApiUrl', { key });
    return this.http.get<any[]>(url).pipe(
      catchError(() => of([{ id: 'f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6', name: 'Action' }]))
    );
  }

  getPlatformsByGameKey(key: string): Observable<any[]> {
    const url = this.configService.getApiUrl('platformsByGameApiUrl', { key });
    return this.http.get<any[]>(url).pipe(
      catchError(() => of([{ id: 'b5d6d065-a1cf-4c0a-9d56-c55d73e82e5b', type: 'PC' }]))
    );
  }

  getPublisherByGameKey(key: string): Observable<any> {
    const url = this.configService.getApiUrl('publisherByGameApiUrl', { key });
    return this.http.get<any>(url).pipe(
      catchError(() => of({ id: '77c06fc6-f8a1-46f9-af03-2793e500112c', companyName: 'Electronic Arts' }))
    );
  }

  getSortingOptions(): Observable<string[]> {
    const url = this.configService.getApiUrl('getSortingOptionsApiUrl');
    return this.http.get<string[]>(url).pipe(
      catchError(() => of(['Most Popular', 'Price: Low to High', 'Price: High to Low', 'Release Date']))
    );
  }

  getPublishDateOptions(): Observable<string[]> {
    const url = this.configService.getApiUrl('getPublishedDateOptionsApiUrl');
    return this.http.get<string[]>(url).pipe(
      catchError(() => of(['Coming Soon', 'Last 7 Days', 'Last 30 Days']))
    );
  }

  getPaginationOptions(): Observable<string[]> {
    const url = this.configService.getApiUrl('getPaggingOptionsApiUrl');
    return this.http.get<string[]>(url).pipe(
      catchError(() => of(['10', '25', '50', '100']))
    );
  }
}
