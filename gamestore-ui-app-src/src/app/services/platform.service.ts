import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from './config.service';
import { Platform, CreatePlatformRequest, UpdatePlatformRequest } from '../models/platform.model';

@Injectable({
  providedIn: 'root'
})
export class PlatformService {
  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  getPlatforms(): Observable<Platform[]> {
    const url = this.configService.getApiUrl('platformsApiUrl');
    return this.http.get<Platform[]>(url);
  }

  getPlatform(id: string): Observable<Platform> {
    const url = this.configService.getApiUrl('platformApiUrl', { id });
    return this.http.get<Platform>(url);
  }

  createPlatform(request: CreatePlatformRequest): Observable<any> {
    const url = this.configService.getApiUrl('addPlatformApiUrl');
    return this.http.post(url, request);
  }

  updatePlatform(request: UpdatePlatformRequest): Observable<any> {
    const url = this.configService.getApiUrl('updatePlatformApiUrl');
    return this.http.put(url, request);
  }

  deletePlatform(id: string): Observable<any> {
    const url = this.configService.getApiUrl('deletePlatformApiUrl', { id });
    return this.http.delete(url);
  }
}
