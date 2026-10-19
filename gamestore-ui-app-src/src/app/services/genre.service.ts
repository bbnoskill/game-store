import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from './config.service';
import { Genre, CreateGenreRequest, UpdateGenreRequest } from '../models/genre.model';

@Injectable({
  providedIn: 'root'
})
export class GenreService {

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  getGenres(): Observable<Genre[]> {
    const url = this.configService.getApiUrl('genresApiUrl');
    return this.http.get<Genre[]>(url);
  }

  getGenre(id: string): Observable<Genre> {
    const url = this.configService.getApiUrl('genreApiUrl', { id });
    return this.http.get<Genre>(url);
  }

  getGenresByParent(id: string): Observable<Genre[]> {
    const url = this.configService.getApiUrl('genresByParentApiUrl', { id });
    return this.http.get<Genre[]>(url);
  }

  createGenre(request: CreateGenreRequest): Observable<any> {
    const url = this.configService.getApiUrl('addGenreApiUrl');
    return this.http.post(url, request);
  }

  updateGenre(request: UpdateGenreRequest): Observable<any> {
    const url = this.configService.getApiUrl('updateGenreApiUrl');
    return this.http.put(url, request);
  }

  deleteGenre(id: string): Observable<any> {
    const url = this.configService.getApiUrl('deleteGenreApiUrl', { id });
    return this.http.delete(url);
  }
}
