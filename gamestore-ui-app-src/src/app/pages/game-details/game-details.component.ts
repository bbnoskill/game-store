import { LanguageService } from '../../services/language.service';
import { TranslatePipe } from '../../shared/pipes/translate.pipe';
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { forkJoin, of, catchError } from 'rxjs';
import { GameService } from '../../services/game.service';
import { CommentService } from '../../services/comment.service';
import { GameImageService } from '../../services/game-image.service';
import { CartService } from '../../services/cart.service';
import { UiStateService } from '../../services/ui-state.service';
import { Game } from '../../models/game.model';
import { Comment, CreateCommentRequest } from '../../models/comment.model';
import { Genre } from '../../models/genre.model';
import { Platform } from '../../models/platform.model';
import { Publisher } from '../../models/publisher.model';
import { StarRatingComponent } from '../../shared/components/star-rating/star-rating.component';
import { LikeButtonComponent } from '../../shared/components/like-button/like-button.component';
import { DiscountPricePipe } from '../../shared/pipes/discount-price.pipe';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-game-details',
  standalone: true,
  imports: [TranslatePipe, CommonModule, RouterModule, FormsModule, StarRatingComponent, LikeButtonComponent, DiscountPricePipe],
  template: `
    <div class="page-container" *ngIf="game">
      <!-- Back Link -->
      <a routerLink="/games" class="back-link">{{ '< Go back to the list of games' | translate }}</a>

      <!-- Game Detail Section -->
      <div class="game-detail-grid">
        <!-- Left: Game Image -->
        <div class="game-image-section">
          <div class="image-wrapper">
            <img [src]="gameImage" [alt]="game.name" class="game-main-image" />
            <button class="img-nav img-nav-left" [title]="'Previous' | translate">&lt;</button>
            <button class="img-nav img-nav-right" [title]="'Next' | translate">&gt;</button>
            <button class="play-btn" [title]="'Play trailer' | translate">
              <svg viewBox="0 0 24 24" fill="currentColor"><path d="M8 5v14l11-7z"/></svg>
            </button>
          </div>
        </div>

        <!-- Right: Game Info -->
        <div class="game-info-section">
          <h1 class="game-title">{{ game.name }}</h1>

          <!-- Price with Discount (US6) -->
          <div class="price-section">
            <span *ngIf="game.discount > 0" class="price-original">\${{ game.price | number:'1.2-2' }}</span>
            <span class="price-current" [class.discounted]="game.discount > 0">
              \${{ (game.discount > 0 ? (game.price | discountPrice:game.discount) : game.price) | number:'1.2-2' }}
            </span>
            <span *ngIf="game.discount > 0" class="discount-tag">-{{ game.discount }}%</span>
          </div>

          <!-- Game Rating (US4) -->
          <div class="rating-row">
            <app-star-rating
              [rating]="gameRating"
              [readonly]="false"
              (ratingChange)="onGameRatingChange($event)"
              size="medium"
            ></app-star-rating>
            <a class="comment-count-link" (click)="scrollToComments()">{{ comments.length }} {{ 'Comments' | translate }}</a>
          </div>

          <!-- Publisher & Meta -->
          <div class="meta-info">
            <p class="publisher-name" *ngIf="publisher">{{ publisher.companyName }}</p>
            <p class="meta-detail" *ngIf="game.releaseDate">
              <span class="meta-label">{{ 'Released' | translate }}</span> {{ game.releaseDate | date:'mediumDate' }}
            </p>
          </div>

          <div class="tech-info-row" *ngIf="platforms.length > 0">
            <div class="tech-item">
              <span class="tech-label">{{ 'Playable on' | translate }}</span>
              <span class="tech-value">{{ platformNames }}</span>
            </div>
          </div>

          <!-- Genres as chips -->
          <div class="genres-section" *ngIf="genres.length > 0">
            <span class="genres-label">{{ 'Genres' | translate }}</span>
            <div class="genre-chips">
              <span class="genre-chip" *ngFor="let genre of genres">{{ genre.name }}</span>
            </div>
          </div>

          <!-- Add To Cart Button -->
          <button class="btn-add-to-cart" (click)="addToCart()" [disabled]="addingToCart">
            {{ (addingToCart ? 'Adding...' : 'Add To Cart') | translate }}
          </button>

          <!-- Tax Information (US7) -->
          <div class="tax-info-section">
            <div class="tax-row">
              <span class="tax-label">{{ 'Price:' | translate }}</span>
              <span class="tax-value">\${{ effectivePrice | number:'1.2-2' }}</span>
            </div>
            <div class="tax-row">
              <span class="tax-label">{{ 'Tax (' | translate }}{{ taxPercent }}%):</span>
              <span class="tax-value">\${{ taxAmount | number:'1.2-2' }}</span>
            </div>
            <div class="tax-row tax-total">
              <span class="tax-label">{{ 'Total:' | translate }}</span>
              <span class="tax-value">\${{ totalWithTax | number:'1.2-2' }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- What You Get Section -->
      <section class="content-section" *ngIf="game.description">
        <h2 class="section-heading">{{ 'What You Get' | translate }}</h2>
        <ul class="feature-list">
          <li>{{ 'Full digital copy of' | translate }} {{ game.name }}</li>
          <li *ngIf="game.unitInStock > 0">{{ game.unitInStock }} {{ 'units currently in stock' | translate }}</li>
          <li>{{ 'Access to all game content and future updates' | translate }}</li>
        </ul>
      </section>

      <!-- Game Description -->
      <section class="content-section" *ngIf="game.description">
        <h2 class="section-heading">{{ 'Game Description' | translate }}</h2>
        <div class="description-text">{{ game.description }}</div>
      </section>

      <!-- Comments Section (US3 + US5) -->
      <section class="comments-section" id="commentsSection">
        <div class="comments-header">
          <div class="comments-header-left">
            <h2 class="section-heading">{{ comments.length }} {{ 'Comments' | translate }}</h2>
            <app-star-rating
              [rating]="averageCommentRating"
              [readonly]="true"
              [showScore]="true"
              size="medium"
            ></app-star-rating>
          </div>
          <button class="btn-write-comment" (click)="showCommentForm = !showCommentForm">{{ 'Write A Comment' | translate }}</button>
        </div>

        <!-- Comment Form -->
        <div class="comment-form" *ngIf="showCommentForm">
          <div class="form-group">
            <label>{{ 'Name' | translate }}</label>
            <input type="text" [(ngModel)]="newComment.name" [placeholder]="'Your name' | translate" />
          </div>
          <div class="form-group">
            <label>{{ 'Comment' | translate }}</label>
            <textarea [(ngModel)]="newComment.body" [placeholder]="'Write your comment...' | translate" rows="4"></textarea>
          </div>
          <div class="form-actions">
            <button class="btn-submit" (click)="submitComment()" [disabled]="!newComment.name || !newComment.body">{{ 'Submit' | translate }}</button>
            <button class="btn-cancel" (click)="showCommentForm = false">{{ 'Cancel' | translate }}</button>
          </div>
        </div>

        <!-- Comment List -->
        <div class="comment-list">
          <ng-container *ngFor="let comment of paginatedComments">
            <ng-container *ngTemplateOutlet="commentTemplate; context: { $implicit: comment, depth: 0 }"></ng-container>
          </ng-container>
        </div>

        <!-- Comment Template (recursive for nested comments) -->
        <ng-template #commentTemplate let-comment let-depth="depth">
          <div class="comment-item" [style.marginLeft.px]="depth * 32">
            <div class="comment-avatar">{{ getInitial(comment.name) }}</div>
            <div class="comment-body">
              <div class="comment-meta">
                <span class="comment-author">{{ comment.name }}</span>
                <!-- Comment Rating (US3) -->
                <app-star-rating
                  [rating]="getCommentRating(comment.id)"
                  [readonly]="false"
                  (ratingChange)="onCommentRatingChange(comment.id, $event)"
                  size="small"
                ></app-star-rating>
              </div>
              <p class="comment-text">{{ comment.body }}</p>
              <div class="comment-actions">
                <!-- Comment Like (US5) -->
                <app-like-button [commentId]="comment.id"></app-like-button>
                <button class="action-link" (click)="startReply(comment)">{{ 'Reply' | translate }}</button>
                <button class="action-link" (click)="startQuote(comment)">{{ 'Quote' | translate }}</button>
              </div>

              <!-- Reply Form -->
              <div class="reply-form" *ngIf="replyingTo?.id === comment.id">
                <div class="form-group">
                  <label>{{ 'Name' | translate }}</label>
                  <input type="text" [(ngModel)]="replyData.name" [placeholder]="'Your name' | translate" />
                </div>
                <div class="form-group">
                  <label>{{ 'Reply' | translate }}</label>
                  <textarea [(ngModel)]="replyData.body" rows="3" [placeholder]="'Write your reply...' | translate"></textarea>
                </div>
                <div class="form-actions">
                  <button class="btn-submit btn-sm" (click)="submitReply(comment)" [disabled]="!replyData.name || !replyData.body">{{ 'Reply' | translate }}</button>
                  <button class="btn-cancel btn-sm" (click)="replyingTo = null">{{ 'Cancel' | translate }}</button>
                </div>
              </div>

              <!-- Child Comments -->
              <ng-container *ngIf="comment.childComments && comment.childComments.length > 0">
                <ng-container *ngFor="let child of comment.childComments">
                  <ng-container *ngTemplateOutlet="commentTemplate; context: { $implicit: child, depth: depth + 1 }"></ng-container>
                </ng-container>
              </ng-container>
            </div>
          </div>
        </ng-template>

        <!-- Comment Pagination -->
        <div class="pagination" *ngIf="commentTotalPages > 1">
          <button class="page-btn" [disabled]="commentPage === 1" (click)="setCommentPage(commentPage - 1)">&lt;</button>
          <button
            class="page-btn"
            *ngFor="let p of commentPageNumbers"
            [class.active]="p === commentPage"
            (click)="setCommentPage(p)"
          >{{ p }}</button>
          <button class="page-btn" [disabled]="commentPage === commentTotalPages" (click)="setCommentPage(commentPage + 1)">&gt;</button>
        </div>
      </section>
    </div>

    <!-- Loading State -->
    <div class="loading-container" *ngIf="!game && !error">
      <p>{{ 'Loading game details...' | translate }}</p>
    </div>
    <div class="error-container" *ngIf="error">
      <p>{{ error | translate }}</p>
      <a routerLink="/games">{{ 'Back to Games' | translate }}</a>
    </div>
  `,
  styles: [`
    .page-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 32px 24px 48px;
    }
    .back-link {
      display: inline-block;
      color: #f57c00;
      text-decoration: none;
      font-size: 14px;
      font-weight: 500;
      margin-bottom: 24px;
      transition: color 0.2s;
    }
    .back-link:hover { color: #e65100; text-decoration: none; }

    /* Game Detail Grid */
    .game-detail-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 40px;
      margin-bottom: 48px;
    }

    /* Image Section */
    .image-wrapper {
      position: relative;
      border-radius: 6px;
      overflow: hidden;
      background: #111;
    }
    .game-main-image {
      width: 100%;
      height: 380px;
      object-fit: cover;
      display: block;
    }
    .img-nav {
      position: absolute;
      top: 50%;
      transform: translateY(-50%);
      background: rgba(0,0,0,0.5);
      color: #fff;
      border: none;
      padding: 12px 14px;
      font-size: 18px;
      cursor: pointer;
      transition: background 0.2s;
    }
    .img-nav:hover { background: rgba(0,0,0,0.75); }
    .img-nav-left { left: 0; border-radius: 0 4px 4px 0; }
    .img-nav-right { right: 0; border-radius: 4px 0 0 4px; }
    .play-btn {
      position: absolute;
      bottom: 16px;
      left: 16px;
      background: rgba(0,0,0,0.6);
      color: #fff;
      border: none;
      width: 44px;
      height: 44px;
      border-radius: 50%;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: background 0.2s;
    }
    .play-btn:hover { background: rgba(245,124,0,0.8); }
    .play-btn svg { width: 22px; height: 22px; }

    /* Info Section */
    .game-info-section { display: flex; flex-direction: column; gap: 16px; }
    .game-title {
      font-size: 28px;
      font-weight: 800;
      color: #222;
      margin: 0;
      line-height: 1.2;
    }

    /* Price Section (US6) */
    .price-section {
      display: flex;
      align-items: center;
      gap: 10px;
    }
    .price-original {
      text-decoration: line-through;
      color: #888;
      font-size: 16px;
    }
    .price-current {
      font-size: 24px;
      font-weight: 700;
      color: #222;
    }
    .price-current.discounted {
      color: #e65100;
    }
    .discount-tag {
      background: #e53935;
      color: #fff;
      font-size: 12px;
      font-weight: 700;
      padding: 3px 8px;
      border-radius: 3px;
    }

    /* Rating Row */
    .rating-row {
      display: flex;
      align-items: center;
      gap: 16px;
    }
    .comment-count-link {
      color: #777;
      font-size: 14px;
      cursor: pointer;
      transition: color 0.2s;
    }
    .comment-count-link:hover { color: #f57c00; text-decoration: underline; }

    /* Meta Info */
    .meta-info { display: flex; flex-direction: column; gap: 4px; }
    .publisher-name {
      font-size: 14px;
      font-weight: 600;
      color: #333;
      margin: 0;
    }
    .meta-detail {
      font-size: 13px;
      color: #777;
      margin: 0;
    }
    .meta-label {
      font-weight: 500;
      color: #555;
    }

    /* Tech Info */
    .tech-info-row {
      display: flex;
      gap: 32px;
    }
    .tech-item {
      display: flex;
      flex-direction: column;
      gap: 2px;
    }
    .tech-label { font-size: 13px; color: #777; font-weight: 500; }
    .tech-value { font-size: 14px; color: #333; }

    /* Genres */
    .genres-section { display: flex; flex-direction: column; gap: 6px; }
    .genres-label { font-size: 13px; color: #777; font-weight: 500; }
    .genre-chips { display: flex; flex-wrap: wrap; gap: 8px; }
    .genre-chip {
      background: #f0f0f0;
      color: #444;
      padding: 5px 14px;
      border-radius: 20px;
      font-size: 12px;
      font-weight: 500;
      border: 1px solid #ddd;
    }

    /* Add To Cart */
    .btn-add-to-cart {
      background: #f57c00;
      color: #fff;
      border: none;
      padding: 14px 36px;
      font-size: 15px;
      font-weight: 700;
      border-radius: 3px;
      cursor: pointer;
      align-self: flex-start;
      transition: background 0.2s;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }
    .btn-add-to-cart:hover { background: #e65100; }
    .btn-add-to-cart:disabled { background: #ccc; cursor: not-allowed; }

    /* Tax Info (US7) */
    .tax-info-section {
      background: #f9f9f9;
      border: 1px solid #eee;
      border-radius: 4px;
      padding: 16px;
      max-width: 280px;
    }
    .tax-row {
      display: flex;
      justify-content: space-between;
      padding: 4px 0;
      font-size: 14px;
      color: #555;
    }
    .tax-total {
      border-top: 1px solid #ddd;
      padding-top: 8px;
      margin-top: 4px;
      font-weight: 700;
      color: #222;
    }
    .tax-total .tax-value { color: #e65100; }

    /* Content Sections */
    .content-section {
      margin-bottom: 40px;
    }
    .section-heading {
      font-size: 22px;
      font-weight: 700;
      color: #222;
      margin: 0 0 16px 0;
      padding-bottom: 12px;
      border-bottom: 2px solid #f0f0f0;
    }
    .feature-list {
      padding-left: 24px;
      display: flex;
      flex-direction: column;
      gap: 8px;
    }
    .feature-list li {
      font-size: 14px;
      color: #555;
      line-height: 1.5;
    }
    .description-text {
      font-size: 14px;
      line-height: 1.7;
      color: #555;
      white-space: pre-line;
    }

    /* Comments Section */
    .comments-section {
      margin-top: 48px;
    }
    .comments-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
    }
    .comments-header-left {
      display: flex;
      align-items: center;
      gap: 16px;
    }
    .comments-header-left .section-heading {
      border-bottom: none;
      margin-bottom: 0;
      padding-bottom: 0;
    }
    .btn-write-comment {
      background: #f57c00;
      color: #fff;
      border: none;
      padding: 10px 24px;
      font-size: 14px;
      font-weight: 600;
      border-radius: 3px;
      cursor: pointer;
      transition: background 0.2s;
    }
    .btn-write-comment:hover { background: #e65100; }

    /* Comment Form */
    .comment-form, .reply-form {
      background: #f9f9f9;
      padding: 20px;
      border-radius: 6px;
      margin-bottom: 24px;
      border: 1px solid #eee;
    }
    .form-group {
      margin-bottom: 12px;
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    .form-group label {
      font-size: 13px;
      font-weight: 500;
      color: #555;
    }
    .form-group input, .form-group textarea {
      padding: 10px 12px;
      border: 1px solid #ccc;
      border-radius: 4px;
      font-size: 14px;
      font-family: inherit;
      width: 100%;
      box-sizing: border-box;
    }
    .form-group input:focus, .form-group textarea:focus {
      outline: none;
      border-color: #f57c00;
    }
    .form-actions {
      display: flex;
      gap: 10px;
    }
    .btn-submit {
      background: #f57c00;
      color: #fff;
      border: none;
      padding: 10px 24px;
      font-size: 13px;
      font-weight: 600;
      border-radius: 3px;
      cursor: pointer;
    }
    .btn-submit:hover { background: #e65100; }
    .btn-submit:disabled { background: #ccc; cursor: not-allowed; }
    .btn-cancel {
      background: #e0e0e0;
      color: #333;
      border: none;
      padding: 10px 24px;
      font-size: 13px;
      font-weight: 500;
      border-radius: 3px;
      cursor: pointer;
    }
    .btn-cancel:hover { background: #d5d5d5; }
    .btn-sm { padding: 6px 16px; font-size: 12px; }

    /* Comment Item */
    .comment-list {
      display: flex;
      flex-direction: column;
      gap: 20px;
    }
    .comment-item {
      display: flex;
      gap: 14px;
      padding-bottom: 20px;
      border-bottom: 1px solid #f0f0f0;
    }
    .comment-avatar {
      width: 42px;
      height: 42px;
      border-radius: 50%;
      background: #e0e0e0;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 16px;
      font-weight: 700;
      color: #666;
      flex-shrink: 0;
    }
    .comment-body {
      flex: 1;
      min-width: 0;
    }
    .comment-meta {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-bottom: 6px;
    }
    .comment-author {
      font-size: 14px;
      font-weight: 700;
      color: #222;
    }
    .comment-text {
      font-size: 14px;
      line-height: 1.6;
      color: #555;
      margin: 0 0 8px 0;
    }
    .comment-actions {
      display: flex;
      align-items: center;
      gap: 12px;
    }
    .action-link {
      background: none;
      border: none;
      font-size: 13px;
      font-weight: 500;
      color: #777;
      cursor: pointer;
      padding: 2px 6px;
      border-radius: 4px;
      transition: all 0.2s;
    }
    .action-link:hover { color: #f57c00; background: #fff3e0; }

    /* Pagination */
    .pagination {
      display: flex;
      justify-content: center;
      gap: 6px;
      margin-top: 32px;
    }
    .page-btn {
      width: 36px;
      height: 36px;
      border: 1px solid #ddd;
      background: #fff;
      color: #333;
      font-size: 13px;
      font-weight: 500;
      cursor: pointer;
      border-radius: 4px;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 0.2s;
    }
    .page-btn:hover { border-color: #f57c00; color: #f57c00; }
    .page-btn.active {
      background: #f57c00;
      color: #fff;
      border-color: #f57c00;
    }
    .page-btn:disabled { opacity: 0.4; cursor: not-allowed; }

    .loading-container, .error-container {
      max-width: 1200px;
      margin: 48px auto;
      padding: 0 24px;
      text-align: center;
      color: #777;
    }

    /* Responsive */
    @media (max-width: 860px) {
      .game-detail-grid {
        grid-template-columns: 1fr;
        gap: 24px;
      }
      .comments-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 12px;
      }
    }
  `]
})
export class GameDetailsComponent implements OnInit {
  private readonly language = inject(LanguageService);
  game: Game | null = null;
  gameImage: string = '';
  genres: Genre[] = [];
  platforms: Platform[] = [];
  get platformNames(): string {
    return this.platforms.map(p => p.type).join(', ');
  }
  publisher: Publisher | null = null;
  comments: Comment[] = [];
  error: string = '';
  addingToCart = false;

  // Game Rating (US4) - UI only
  gameRating: number = 4;

  // Tax (US7)
  taxRate = environment.taxRate;
  get taxPercent(): number { return Math.round(this.taxRate * 100); }
  get effectivePrice(): number {
    if (!this.game) return 0;
    return this.game.discount > 0
      ? Math.round(this.game.price * (1 - this.game.discount / 100) * 100) / 100
      : this.game.price;
  }
  get taxAmount(): number {
    return Math.round(this.effectivePrice * this.taxRate * 100) / 100;
  }
  get totalWithTax(): number {
    return Math.round((this.effectivePrice + this.taxAmount) * 100) / 100;
  }

  // Comments
  showCommentForm = false;
  newComment = { name: '', body: '' };
  replyingTo: Comment | null = null;
  replyData = { name: '', body: '' };
  commentPage = 1;
  commentsPerPage = 5;

  get commentTotalPages(): number {
    return Math.ceil(this.flatComments.length / this.commentsPerPage);
  }
  get commentPageNumbers(): number[] {
    const pages: number[] = [];
    for (let i = 1; i <= this.commentTotalPages; i++) pages.push(i);
    return pages;
  }
  get flatComments(): Comment[] {
    return this.comments;
  }
  get paginatedComments(): Comment[] {
    const start = (this.commentPage - 1) * this.commentsPerPage;
    return this.flatComments.slice(start, start + this.commentsPerPage);
  }
  get averageCommentRating(): number {
    if (this.comments.length === 0) return 0;
    let total = 0;
    let count = 0;
    const addRatings = (cmts: Comment[]) => {
      cmts.forEach(c => {
        total += this.uiStateService.getCommentRating(c.id, 4);
        count++;
        if (c.childComments) addRatings(c.childComments);
      });
    };
    addRatings(this.comments);
    return count > 0 ? Math.round((total / count) * 10) / 10 : 0;
  }

  private gameKey: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private gameService: GameService,
    private commentService: CommentService,
    private gameImageService: GameImageService,
    private cartService: CartService,
    private uiStateService: UiStateService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.gameKey = params['key'];
      this.loadGame();
    });
  }

  loadGame(): void {
    this.gameService.getGameByKey(this.gameKey).subscribe({
      next: (game) => {
        this.game = game;
        this.gameImage = this.gameImageService.getGameImage(game.key, game.name);
        this.gameRating = this.uiStateService.getGameRating(game.key, 4);
        this.loadRelatedData();
      },
      error: (err) => {
        this.error = 'Failed to load game details. The game may not exist.';
      }
    });
  }

  loadRelatedData(): void {
    forkJoin({
      genres: this.gameService.getGenresByGameKey(this.gameKey).pipe(catchError(() => of([]))),
      platforms: this.gameService.getPlatformsByGameKey(this.gameKey).pipe(catchError(() => of([]))),
      publisher: this.gameService.getPublisherByGameKey(this.gameKey).pipe(catchError(() => of(null))),
      comments: this.commentService.getCommentsByGameKey(this.gameKey).pipe(catchError(() => of([])))
    }).subscribe(result => {
      this.genres = result.genres || [];
      this.platforms = result.platforms || [];
      this.publisher = result.publisher;
      this.comments = result.comments || [];
    });
  }

  // Game Rating (US4)
  onGameRatingChange(rating: number): void {
    this.gameRating = rating;
    if (this.game) {
      this.uiStateService.setGameRating(this.game.key, rating);
    }
  }

  // Comment Rating (US3)
  getCommentRating(commentId: string): number {
    return this.uiStateService.getCommentRating(commentId, 4);
  }

  onCommentRatingChange(commentId: string, rating: number): void {
    this.uiStateService.setCommentRating(commentId, rating);
  }

  addToCart(): void {
    if (!this.game || this.addingToCart) return;
    this.addingToCart = true;
    this.cartService.addToCart(this.game.key).subscribe({
      next: () => {
        this.addingToCart = false;
      },
      error: () => {
        this.addingToCart = false;
        alert(this.language.translate('Failed to add to cart. Please try again.'));
      }
    });
  }

  scrollToComments(): void {
    const el = document.getElementById('commentsSection');
    if (el) el.scrollIntoView({ behavior: 'smooth' });
  }

  submitComment(): void {
    if (!this.newComment.name || !this.newComment.body) return;
    const request: CreateCommentRequest = {
      comment: {
        name: this.newComment.name,
        body: this.newComment.body
      }
    };
    this.commentService.addComment(this.gameKey, request).subscribe({
      next: () => {
        this.newComment = { name: '', body: '' };
        this.showCommentForm = false;
        this.loadComments();
      },
      error: () => alert(this.language.translate('Failed to submit comment.'))
    });
  }

  startReply(comment: Comment): void {
    this.replyingTo = comment;
    this.replyData = { name: '', body: '' };
  }

  startQuote(comment: Comment): void {
    this.replyingTo = comment;
    this.replyData = {
      name: '',
      body: `[${comment.name}] ${comment.body}\n\n`
    };
  }

  submitReply(parent: Comment): void {
    if (!this.replyData.name || !this.replyData.body) return;
    const request: CreateCommentRequest = {
      comment: {
        name: this.replyData.name,
        body: this.replyData.body
      },
      parentCommentId: parent.id,
      action: this.replyData.body.startsWith('[') ? 'Quote' : 'Reply'
    };
    this.commentService.addComment(this.gameKey, request).subscribe({
      next: () => {
        this.replyingTo = null;
        this.replyData = { name: '', body: '' };
        this.loadComments();
      },
      error: () => alert(this.language.translate('Failed to submit reply.'))
    });
  }

  loadComments(): void {
    this.commentService.getCommentsByGameKey(this.gameKey).subscribe({
      next: (comments) => { this.comments = comments || []; },
      error: () => {}
    });
  }

  setCommentPage(page: number): void {
    if (page >= 1 && page <= this.commentTotalPages) {
      this.commentPage = page;
    }
  }

  getInitial(name: string): string {
    return name ? name.charAt(0).toUpperCase() : '?';
  }
}
