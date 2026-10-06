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
    { id: 'f2a3b4c5-d6e7-4f8a-9b0c-d1e2f3a4b5c6', name: 'Action' },
    { id: 'a3b4c5d6-e7f8-4a9b-0c1d-e2f3a4b5c6d7', name: 'FPS' },
    { id: 'b4c5d6e7-f8a9-4b0c-1d2e-f3a4b5c6d7e8', name: 'TPS' },
    { id: 'f8a9b0c1-d2e3-4f4a-5b6c-d7e8f9a0b1c2', name: 'Adventure' },
    { id: 'd4e5f6a7-b8c9-4d0e-1f2a-b3c4d5e6f7a8', name: 'RPG' },
    { id: 'a1b2c3d4-e5f6-4a7b-8c9d-e0f1a2b3c4d5', name: 'Strategy' },
    { id: 'b2c3d4e5-f6a7-4b8c-9d0e-f1a2b3c4d5e6', name: 'RTS' },
    { id: 'c3d4e5f6-a7b8-4c9d-0e1f-a2b3c4d5e6f7', name: 'TBS' },
    { id: 'e5f6a7b8-c9d0-4e1f-2a3b-c4d5e6f7a8b9', name: 'Simulator' },
    { id: 'f6a7b8c9-d0e1-4f2a-3b4c-d5e6f7a8b9c0', name: 'Sports' },
    { id: 'a7b8c9d0-e1f2-4a3b-4c5d-e6f7a8b9c0d1', name: 'Races' },
    { id: 'b8c9d0e1-f2a3-4b4c-5d6e-f7a8b9c0d1e2', name: 'Rally' },
    { id: 'c9d0e1f2-a3b4-4c5d-6e7f-a8b9c0d1e2f3', name: 'Arcade' },
    { id: 'd0e1f2a3-b4c5-4d6e-7f8a-b9c0d1e2f3a4', name: 'Formula' },
    { id: 'e1f2a3b4-c5d6-4e7f-8a9b-c0d1e2f3a4b5', name: 'Off-road' },
    { id: 'c5d6e7f8-a9b0-4c1d-2e3f-a4b5c6d7e8f9', name: 'Fighting' },
    { id: 'd6e7f8a9-b0c1-4d2e-3f4a-b5c6d7e8f9a0', name: 'Battle Royale' },
    { id: 'e7f8a9b0-c1d2-4e3f-4a5b-c6d7e8f9a0b1', name: 'Hero Shooter' },
    { id: 'a9b0c1d2-e3f4-4a5b-6c7d-e8f9a0b1c2d3', name: 'Puzzle & Skill' }
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
