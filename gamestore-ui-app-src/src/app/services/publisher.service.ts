import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { ConfigService } from './config.service';
import { Publisher, CreatePublisherRequest, UpdatePublisherRequest } from '../models/publisher.model';

@Injectable({
  providedIn: 'root'
})
export class PublisherService {
  private mockPublishers: Publisher[] = [
    { id: '1', companyName: 'EA Games', homePage: 'https://ea.com', description: 'Electronic Arts Inc. is an American video game publisher.' },
    { id: '2', companyName: 'Blizzard', homePage: 'https://blizzard.com', description: 'Blizzard Entertainment is a video game developer and publisher.' },
    { id: '3', companyName: 'Valve', homePage: 'https://valvesoftware.com', description: 'Valve Corporation is an American video game developer.' },
    { id: '4', companyName: 'Rockstar', homePage: 'https://rockstargames.com', description: 'Rockstar Games is an American video game publisher.' },
    { id: '5', companyName: 'Ubisoft', homePage: 'https://ubisoft.com', description: 'Ubisoft Entertainment is a French video game publisher.' }
  ];

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  getPublishers(): Observable<Publisher[]> {
    const url = this.configService.getApiUrl('publishersApiUrl');
    return this.http.get<Publisher[]>(url).pipe(
      catchError(() => of(this.mockPublishers))
    );
  }

  getPublisherByCompanyName(companyname: string): Observable<Publisher> {
    const url = this.configService.getApiUrl('publisherApiUrl', { companyname });
    return this.http.get<Publisher>(url).pipe(
      catchError(() => of(this.mockPublishers.find(p => p.companyName.toLowerCase() === companyname.toLowerCase()) || this.mockPublishers[0]))
    );
  }

  createPublisher(request: CreatePublisherRequest): Observable<any> {
    const url = this.configService.getApiUrl('addPublisherApiUrl');
    return this.http.post(url, request).pipe(
      catchError(() => of({ success: true }))
    );
  }

  updatePublisher(request: UpdatePublisherRequest): Observable<any> {
    const url = this.configService.getApiUrl('updatePublisherApiUrl');
    return this.http.put(url, request).pipe(
      catchError(() => of({ success: true }))
    );
  }

  deletePublisher(id: string): Observable<any> {
    const url = this.configService.getApiUrl('deletePublisherApiUrl', { id });
    return this.http.delete(url).pipe(
      catchError(() => of({ success: true }))
    );
  }
}
