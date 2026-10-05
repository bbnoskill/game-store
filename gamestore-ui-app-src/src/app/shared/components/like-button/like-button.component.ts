import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UiStateService } from '../../../services/ui-state.service';

@Component({
  selector: 'app-like-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button
      type="button"
      class="like-btn"
      [class.liked]="isLiked"
      (click)="onToggleLike($event)"
      title="Like comment"
    >
      <svg class="like-icon" viewBox="0 0 24 24" fill="currentColor">
        <path d="M1 21h4V9H1v12zm22-11c0-1.1-.9-2-2-2h-6.31l.95-4.57.03-.32c0-.41-.17-.79-.44-1.06L14.17 1 7.59 7.59C7.22 7.95 7 8.45 7 9v10c0 1.1.9 2 2 2h9c.83 0 1.54-.5 1.84-1.22l3.02-7.05c.09-.23.14-.47.14-.73v-2z"/>
      </svg>
      <span class="like-text">Like</span>
      <span class="like-count" *ngIf="likeCount > 0">({{ likeCount }})</span>
    </button>
  `,
  styles: [`
    .like-btn {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      background: none;
      border: none;
      padding: 2px 6px;
      font-size: 13px;
      font-weight: 500;
      color: #777;
      cursor: pointer;
      border-radius: 4px;
      transition: all 0.2s ease;
    }
    .like-btn:hover {
      color: #e65100;
      background-color: #fff3e0;
    }
    .like-btn.liked {
      color: #f57c00;
      font-weight: 600;
    }
    .like-icon {
      width: 14px;
      height: 14px;
    }
    .like-count {
      font-size: 12px;
    }
  `]
})
export class LikeButtonComponent implements OnInit {
  @Input({ required: true }) commentId!: string;
  @Input() initialLikes: number = 0;
  @Output() likeChange = new EventEmitter<number>();

  likeCount: number = 0;
  isLiked: boolean = false;

  constructor(private uiStateService: UiStateService) {}

  ngOnInit(): void {
    this.likeCount = this.uiStateService.getCommentLikes(this.commentId, this.initialLikes);
    this.isLiked = this.uiStateService.isCommentLikedByUser(this.commentId);
  }

  onToggleLike(event: Event): void {
    event.stopPropagation();
    this.likeCount = this.uiStateService.toggleCommentLike(this.commentId, this.initialLikes);
    this.isLiked = this.uiStateService.isCommentLikedByUser(this.commentId);
    this.likeChange.emit(this.likeCount);
  }
}
