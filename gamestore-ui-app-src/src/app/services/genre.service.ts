import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { ConfigService } from './config.service';
import { Genre, CreateGenreRequest, UpdateGenreRequest } from '../models/genre.model';

@Injectable({
  providedIn: 'root'
})
export class GenreService {
  private mockGenres: Genre[] = [
    { id: '1', name: 'Action' },
    { id: '2', name: 'Adventure' },
    { id: '3', name: 'Arcade' },
    { id: '4', name: 'Shooter' },
    { id: '5', name: 'Simulation' },
    { id: '6', name: 'Strategy' },
    { id: '7', name: 'Role-Playing' },
    { id: '8', name: 'Sports' }
  ];

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  getGenres(): Observable<Genre[]> {
    const url = this.configService.getApiUrl('genresApiUrl');
    return this.http.get<Genre[]>(url).pipe(
      catchError(() => of(this.mockGenres))
    );
  }

  getGenre(id: string): Observable<Genre> {
    const url = this.configService.getApiUrl('genreApiUrl', { id });
    return this.http.get<Genre>(url).pipe(
      catchError(() => of(this.mockGenres.find(g => g.id === id) || this.mockGenres[0]))
    );
  }

  getGenresByParent(id: string): Observable<Genre[]> {
    const url = this.configService.getApiUrl('genresByParentApiUrl', { id });
    return this.http.get<Genre[]>(url).pipe(
      catchError(() => of([]))
    );
  }

  createGenre(request: CreateGenreRequest): Observable<any> {
    const url = this.configService.getApiUrl('addGenreApiUrl');
    return this.http.post(url, request).pipe(
      catchError(() => of({ success: true }))
    );
  }

  updateGenre(request: UpdateGenreRequest): Observable<any> {
    const url = this.configService.getApiUrl('updateGenreApiUrl');
    return this.http.put(url, request).pipe(
      catchError(() => of({ success: true }))
    );
  }

  deleteGenre(id: string): Observable<any> {
    const url = this.configService.getApiUrl('deleteGenreApiUrl', { id });
    return this.http.delete(url).pipe(
      catchError(() => of({ success: true }))
    );
  }
}
