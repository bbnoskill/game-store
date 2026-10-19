import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from './config.service';
import { Game, CreateGameRequest, UpdateGameRequest } from '../models/game.model';
import { GameFilter, GameFilterResult } from '../models/game-filter.model';

@Injectable({
  providedIn: 'root'
})
export class GameService {

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

    return this.http.get<GameFilterResult | Game[]>(url, { params });
  }

  getAllGamesWithoutFilter(): Observable<Game[]> {
    const url = this.configService.getApiUrl('gamesAllApiUrl');
    return this.http.get<Game[]>(url);
  }

  getGameByKey(key: string): Observable<Game> {
    const url = this.configService.getApiUrl('gameApiUrl', { key });
    return this.http.get<Game>(url);
  }

  getGameById(id: string): Observable<Game> {
    const url = this.configService.getApiUrl('gameByIdApiUrl', { id });
    return this.http.get<Game>(url);
  }

  createGame(request: CreateGameRequest): Observable<any> {
    const url = this.configService.getApiUrl('addGameApiUrl');
    return this.http.post(url, request);
  }

  updateGame(request: UpdateGameRequest): Observable<any> {
    const url = this.configService.getApiUrl('updateGameApiUrl');
    return this.http.put(url, request);
  }

  deleteGame(key: string): Observable<any> {
    const url = this.configService.getApiUrl('deleteGameApiUrl', { key });
    return this.http.delete(url);
  }

  buyGame(key: string): Observable<any> {
    const url = this.configService.getApiUrl('buyGameApiUrl', { key });
    return this.http.post(url, {});
  }

  downloadGameFile(key: string): Observable<Blob> {
    const url = this.configService.getApiUrl('getGameFileApiUrl', { key });
    return this.http.get(url, { responseType: 'blob' });
  }

  getGenresByGameKey(key: string): Observable<any[]> {
    const url = this.configService.getApiUrl('genresByGameApiUrl', { key });
    return this.http.get<any[]>(url);
  }

  getPlatformsByGameKey(key: string): Observable<any[]> {
    const url = this.configService.getApiUrl('platformsByGameApiUrl', { key });
    return this.http.get<any[]>(url);
  }

  getPublisherByGameKey(key: string): Observable<any> {
    const url = this.configService.getApiUrl('publisherByGameApiUrl', { key });
    return this.http.get<any>(url);
  }

  getSortingOptions(): Observable<string[]> {
    const url = this.configService.getApiUrl('getSortingOptionsApiUrl');
    return this.http.get<string[]>(url);
  }

  getPublishDateOptions(): Observable<string[]> {
    const url = this.configService.getApiUrl('getPublishedDateOptionsApiUrl');
    return this.http.get<string[]>(url);
  }

  getPaginationOptions(): Observable<string[]> {
    const url = this.configService.getApiUrl('getPaggingOptionsApiUrl');
    return this.http.get<string[]>(url);
  }
}
