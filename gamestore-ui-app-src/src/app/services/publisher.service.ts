import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from './config.service';
import { Publisher, CreatePublisherRequest, UpdatePublisherRequest } from '../models/publisher.model';

@Injectable({
  providedIn: 'root'
})
export class PublisherService {

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  getPublishers(): Observable<Publisher[]> {
    const url = this.configService.getApiUrl('publishersApiUrl');
    return this.http.get<Publisher[]>(url, { params: { includeMongo: false } });
  }

  getPublisherByCompanyName(companyname: string): Observable<Publisher> {
    const url = this.configService.getApiUrl('publisherApiUrl', { companyname });
    return this.http.get<Publisher>(url, { params: { includeMongo: false } });
  }

  createPublisher(request: CreatePublisherRequest): Observable<any> {
    const url = this.configService.getApiUrl('addPublisherApiUrl');
    return this.http.post(url, request);
  }

  updatePublisher(request: UpdatePublisherRequest): Observable<any> {
    const url = this.configService.getApiUrl('updatePublisherApiUrl');
    return this.http.put(url, request);
  }

  deletePublisher(id: string): Observable<any> {
    const url = this.configService.getApiUrl('deletePublisherApiUrl', { id });
    return this.http.delete(url);
  }
}
