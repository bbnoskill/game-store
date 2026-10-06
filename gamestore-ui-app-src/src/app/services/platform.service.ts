import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { ConfigService } from './config.service';
import { Platform, CreatePlatformRequest, UpdatePlatformRequest } from '../models/platform.model';

@Injectable({
  providedIn: 'root'
})
export class PlatformService {
  private mockPlatforms: Platform[] = [
    { id: '1', type: 'PS4' },
    { id: '2', type: 'PS5' },
    { id: '3', type: 'Xbox Series X' },
    { id: '4', type: 'PC' },
    { id: '5', type: 'Nintendo Switch' }
  ];

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  getPlatforms(): Observable<Platform[]> {
    const url = this.configService.getApiUrl('platformsApiUrl');
    return this.http.get<Platform[]>(url).pipe(
      catchError(() => of(this.mockPlatforms))
    );
  }

  getPlatform(id: string): Observable<Platform> {
    const url = this.configService.getApiUrl('platformApiUrl', { id });
    return this.http.get<Platform>(url).pipe(
      catchError(() => of(this.mockPlatforms.find(p => p.id === id) || this.mockPlatforms[0]))
    );
  }

  createPlatform(request: CreatePlatformRequest): Observable<any> {
    const url = this.configService.getApiUrl('addPlatformApiUrl');
    return this.http.post(url, request).pipe(
      catchError(() => of({ success: true }))
    );
  }

  updatePlatform(request: UpdatePlatformRequest): Observable<any> {
    const url = this.configService.getApiUrl('updatePlatformApiUrl');
    return this.http.put(url, request).pipe(
      catchError(() => of({ success: true }))
    );
  }

  deletePlatform(id: string): Observable<any> {
    const url = this.configService.getApiUrl('deletePlatformApiUrl', { id });
    return this.http.delete(url).pipe(
      catchError(() => of({ success: true }))
    );
  }
}
