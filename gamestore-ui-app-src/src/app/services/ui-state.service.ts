import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UiStateService {
  // UI-only storage that resets on page refresh
  private commentLikesMap = new Map<string, number>();
  private userLikedComments = new Set<string>();
  private commentRatingsMap = new Map<string, number>();
  private gameRatingsMap = new Map<string, number>();

  // Comment Likes
  getCommentLikes(commentId: string, initialLikes: number = 0): number {
    if (!this.commentLikesMap.has(commentId)) {
      this.commentLikesMap.set(commentId, initialLikes);
    }
    return this.commentLikesMap.get(commentId)!;
  }

  isCommentLikedByUser(commentId: string): boolean {
    return this.userLikedComments.has(commentId);
  }

  toggleCommentLike(commentId: string, initialLikes: number = 0): number {
    const current = this.getCommentLikes(commentId, initialLikes);
    if (this.userLikedComments.has(commentId)) {
      const next = Math.max(0, current - 1);
      this.commentLikesMap.set(commentId, next);
      this.userLikedComments.delete(commentId);
      return next;
    } else {
      const next = current + 1;
      this.commentLikesMap.set(commentId, next);
      this.userLikedComments.add(commentId);
      return next;
    }
  }

  // Comment Ratings
  getCommentRating(commentId: string, defaultRating: number = 5): number {
    if (!this.commentRatingsMap.has(commentId)) {
      this.commentRatingsMap.set(commentId, defaultRating);
    }
    return this.commentRatingsMap.get(commentId)!;
  }

  setCommentRating(commentId: string, rating: number): void {
    this.commentRatingsMap.set(commentId, rating);
  }

  // Game Ratings
  getGameRating(gameKey: string, defaultRating: number = 4): number {
    if (!this.gameRatingsMap.has(gameKey)) {
      this.gameRatingsMap.set(gameKey, defaultRating);
    }
    return this.gameRatingsMap.get(gameKey)!;
  }

  setGameRating(gameKey: string, rating: number): void {
    this.gameRatingsMap.set(gameKey, rating);
  }
}
