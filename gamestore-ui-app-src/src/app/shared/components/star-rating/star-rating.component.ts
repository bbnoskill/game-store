import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-star-rating',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="star-rating-container" [class.interactive]="!readonly" [class.small]="size === 'small'" [class.large]="size === 'large'">
      <div class="stars">
        <button
          type="button"
          *ngFor="let star of stars; let i = index"
          class="star-btn"
          [disabled]="readonly"
          (mouseenter)="onHover(i + 1)"
          (mouseleave)="onLeave()"
          (click)="onSelect(i + 1)"
          [title]="'Rating ' + (i + 1)"
        >
          <svg
            class="star-icon"
            [class.filled]="(hoverRating || rating) >= i + 1"
            [class.half]="(hoverRating || rating) >= i + 0.5 && (hoverRating || rating) < i + 1"
            viewBox="0 0 24 24"
            fill="currentColor"
          >
            <path d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" />
          </svg>
        </button>
      </div>
      <span *ngIf="showScore" class="score-text">{{ rating | number:'1.1-1' }}</span>
    </div>
  `,
  styles: [`
    .star-rating-container {
      display: inline-flex;
      align-items: center;
      gap: 6px;
    }
    .stars {
      display: inline-flex;
      align-items: center;
      gap: 2px;
    }
    .star-btn {
      background: none;
      border: none;
      padding: 0;
      cursor: default;
      display: flex;
      align-items: center;
      justify-content: center;
      line-height: 1;
    }
    .interactive .star-btn {
      cursor: pointer;
      transition: transform 0.15s ease;
    }
    .interactive .star-btn:hover {
      transform: scale(1.15);
    }
    .star-icon {
      width: 18px;
      height: 18px;
      color: #ccc;
      transition: color 0.15s ease;
    }
    .star-icon.filled {
      color: #ff9800;
    }
    .small .star-icon {
      width: 14px;
      height: 14px;
    }
    .large .star-icon {
      width: 24px;
      height: 24px;
    }
    .score-text {
      font-size: 14px;
      font-weight: 600;
      color: #555;
    }
  `]
})
export class StarRatingComponent {
  @Input() rating: number = 0;
  @Input() maxRating: number = 5;
  @Input() readonly: boolean = false;
  @Input() showScore: boolean = false;
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  @Output() ratingChange = new EventEmitter<number>();

  hoverRating: number = 0;

  get stars(): number[] {
    return Array(this.maxRating).fill(0);
  }

  onHover(val: number): void {
    if (!this.readonly) {
      this.hoverRating = val;
    }
  }

  onLeave(): void {
    if (!this.readonly) {
      this.hoverRating = 0;
    }
  }

  onSelect(val: number): void {
    if (!this.readonly) {
      this.rating = val;
      this.ratingChange.emit(val);
    }
  }
}
