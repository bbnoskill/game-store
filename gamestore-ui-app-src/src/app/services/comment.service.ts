import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of, tap } from 'rxjs';
import { ConfigService } from './config.service';
import { Comment, CreateCommentRequest } from '../models/comment.model';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private localCommentsStore: { [gameKey: string]: Comment[] } = {
    'halo-infinite': [
      {
        id: 'c-101',
        name: 'Alex',
        body: 'Amazing campaign and graphics! Master Chief is back.',
        date: '2025-01-15T10:30:00Z',
        likes: 12,
        rating: 5,
        childComments: [
          {
            id: 'c-102',
            name: 'Sarah',
            body: 'Agreed! The open world design fits Halo really well.',
            date: '2025-01-15T11:00:00Z',
            likes: 4,
            rating: 5
          }
        ]
      },
      {
        id: 'c-103',
        name: 'Gamer99',
        body: 'Multiplayer gameplay feels super crisp.',
        date: '2025-02-01T14:20:00Z',
        likes: 8,
        rating: 4
      }
    ],
    'skyrim': [
      {
        id: 'c-201',
        name: 'Dovahkiin',
        body: 'Must-play RPG classic. Endless replayability with mods!',
        date: '2024-12-10T15:00:00Z',
        likes: 25,
        rating: 5
      }
    ],
    'witcher-3': [
      {
        id: 'c-301',
        name: 'GeraltFan',
        body: 'One of the best storylines in gaming history.',
        date: '2025-01-05T18:45:00Z',
        likes: 30,
        rating: 5
      }
    ]
  };

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  getCommentsByGameKey(key: string): Observable<Comment[]> {
    const url = this.configService.getApiUrl('commentsByGameApiUrl', { key });
    return this.http.get<Comment[]>(url).pipe(
      tap((comments) => {
        if (comments && comments.length > 0) {
          this.localCommentsStore[key] = comments;
        }
      }),
      catchError(() => {
        return of(this.getOrCreateLocalComments(key));
      })
    );
  }

  addComment(key: string, request: CreateCommentRequest): Observable<any> {
    const url = this.configService.getApiUrl('addCommentApiUrl', { key });
    const payload = {
      comment: request.comment,
      parentId: request.parentId || request.parentCommentId || null,
      action: request.action || null
    };

    return this.http.post(url, payload).pipe(
      tap(() => {
        this.addCommentToLocalStore(key, request);
      }),
      catchError(() => {
        this.addCommentToLocalStore(key, request);
        return of({ success: true });
      })
    );
  }

  deleteComment(key: string, id: string): Observable<any> {
    const url = this.configService.getApiUrl('deleteCommentApiUrl', { key, id });
    return this.http.delete(url).pipe(
      tap(() => {
        this.deleteCommentFromLocalStore(key, id);
      }),
      catchError(() => {
        this.deleteCommentFromLocalStore(key, id);
        return of({ success: true });
      })
    );
  }

  getBanDurations(): Observable<string[]> {
    const url = this.configService.getApiUrl('getBanDurationsApiUrl');
    return this.http.get<string[]>(url).pipe(
      catchError(() => of(['1 hour', '1 day', '1 week', '1 month', 'Permanent']))
    );
  }

  banUser(request: { user: string; duration: string }): Observable<any> {
    const url = this.configService.getApiUrl('banUserApiUrl');
    return this.http.post(url, request).pipe(
      catchError(() => of({ success: true }))
    );
  }

  private getOrCreateLocalComments(key: string): Comment[] {
    if (!this.localCommentsStore[key]) {
      this.localCommentsStore[key] = [
        {
          id: 'c-' + Date.now(),
          name: 'Community Gamer',
          body: 'Great game! Highly recommended for all players.',
          date: new Date().toISOString(),
          likes: 5,
          rating: 5,
          childComments: []
        }
      ];
    }
    return this.localCommentsStore[key];
  }

  private addCommentToLocalStore(key: string, request: CreateCommentRequest): void {
    const comments = this.getOrCreateLocalComments(key);
    const parentId = request.parentId || request.parentCommentId;

    const newComment: Comment = {
      id: 'c-' + Date.now() + '-' + Math.floor(Math.random() * 1000),
      name: request.comment.name,
      body: request.comment.body,
      date: new Date().toISOString(),
      likes: 0,
      rating: 5,
      childComments: []
    };

    if (parentId) {
      const added = this.insertChildComment(comments, parentId, newComment);
      if (!added) {
        comments.push(newComment);
      }
    } else {
      comments.push(newComment);
    }
  }

  private insertChildComment(list: Comment[], parentId: string, newComment: Comment): boolean {
    for (const item of list) {
      if (item.id === parentId) {
        if (!item.childComments) item.childComments = [];
        item.childComments.push(newComment);
        return true;
      }
      if (item.childComments && item.childComments.length > 0) {
        if (this.insertChildComment(item.childComments, parentId, newComment)) {
          return true;
        }
      }
    }
    return false;
  }

  private deleteCommentFromLocalStore(key: string, id: string): void {
    if (!this.localCommentsStore[key]) return;
    this.localCommentsStore[key] = this.removeCommentRecursive(this.localCommentsStore[key], id);
  }

  private removeCommentRecursive(list: Comment[], id: string): Comment[] {
    return list.filter(c => c.id !== id).map(c => {
      if (c.childComments && c.childComments.length > 0) {
        c.childComments = this.removeCommentRecursive(c.childComments, id);
      }
      return c;
    });
  }
}
