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
    { id: 'b5d6d065-a1cf-4c0a-9d56-c55d73e82e5b', type: 'Desktop' },
    { id: 'd9f9a123-c3e1-4e2c-bf78-e77f95a04a7d', type: 'Console' },
    { id: 'e1d6d065-e3df-4c0a-8d46-b44d62d71d4a', type: 'Mobile' },
    { id: 'c7e8f012-b2d0-4d1b-ae67-d66e84f93f6c', type: 'Browser' },
    { id: 'f0a0b234-d4f2-4f3d-c089-f88a06b15b8e', type: 'VR' }
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
