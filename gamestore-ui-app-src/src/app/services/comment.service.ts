import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from './config.service';
import { Comment, CreateCommentRequest } from '../models/comment.model';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  getCommentsByGameKey(key: string): Observable<Comment[]> {
    const url = this.configService.getApiUrl('commentsByGameApiUrl', { key });
    return this.http.get<Comment[]>(url);
  }

  addComment(key: string, request: CreateCommentRequest): Observable<any> {
    const url = this.configService.getApiUrl('addCommentApiUrl', { key });
    return this.http.post(url, request);
  }

  deleteComment(key: string, id: string): Observable<any> {
    const url = this.configService.getApiUrl('deleteCommentApiUrl', { key, id });
    return this.http.delete(url);
  }

  getBanDurations(): Observable<string[]> {
    const url = this.configService.getApiUrl('getBanDurationsApiUrl');
    return this.http.get<string[]>(url);
  }

  banUser(request: { user: string; duration: string }): Observable<any> {
    const url = this.configService.getApiUrl('banUserApiUrl');
    return this.http.post(url, request);
  }
}
