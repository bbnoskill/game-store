import { TranslatePipe } from '../../shared/pipes/translate.pipe';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { forkJoin, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { GameService } from '../../services/game.service';
import { GenreService } from '../../services/genre.service';
import { PlatformService } from '../../services/platform.service';
import { PublisherService } from '../../services/publisher.service';
import { GameImageService } from '../../services/game-image.service';
import { CartService } from '../../services/cart.service';

import { Game } from '../../models/game.model';
import { GameFilter, GameFilterResult } from '../../models/game-filter.model';
import { Genre } from '../../models/genre.model';
import { Platform } from '../../models/platform.model';
import { Publisher } from '../../models/publisher.model';

import { DiscountPricePipe } from '../../shared/pipes/discount-price.pipe';

@Component({
  selector: 'app-game-list',
  standalone: true,
  imports: [TranslatePipe, CommonModule, RouterModule, FormsModule, DiscountPricePipe],
  template: `
    <div class="page-container">
      <!-- Header row: Games title + Search Bar -->
      <div class="header-row">
        <h1 class="page-title">{{ 'Games' | translate }}</h1>
        <div class="search-box">
          <input
            type="text"
            [(ngModel)]="filter.name"
            [placeholder]="'Search for Game' | translate"
            (input)="applyFilter()"
            (keyup.enter)="applyFilter()"
          />
          <button type="button" class="search-btn" (click)="applyFilter()" [title]="'Search' | translate">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M15.5 14h-.79l-.28-.27A6.471 6.471 0 0 0 16 9.5 6.5 6.5 0 1 0 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/></svg>
          </button>
        </div>
      </div>

      <!-- Main Layout: Grid on Left, Refine Results Sidebar on Right -->
      <div class="content-layout">
        <!-- Left: Game Grid + Pagination -->
        <div class="main-content">
          <div class="game-grid" *ngIf="games.length > 0; else noGames">
            <div class="game-card" *ngFor="let game of games" [routerLink]="['/game', game.key]">
              <div class="game-image-wrap">
                <img [src]="getImageUrl(game.key, game.name)" (error)="handleImgError($event, game.key, game.name)" [alt]="game.name" class="game-img" />
                <span *ngIf="game.discount > 0" class="card-discount-tag">-{{ game.discount }}%</span>
              </div>
              <div class="game-card-body">
                <h3 class="game-name" [title]="game.name">{{ game.name }}</h3>
                <p class="game-platform">{{ getPlatformText(game) | translate }}</p>
                <div class="price-line">
                  <ng-container *ngIf="game.discount > 0; else regularPrice">
                    <span class="old-price">\${{ game.price | number:'1.2-2' }}</span>
                    <span class="discounted-price">\${{ game.price | discountPrice:game.discount | number:'1.2-2' }}</span>
                  </ng-container>
                  <ng-template #regularPrice>
                    <span class="current-price">\${{ game.price | number:'1.2-2' }}</span>
                  </ng-template>
                </div>
              </div>
            </div>
          </div>

          <ng-template #noGames>
            <div class="no-games-box">
              <p>{{ 'No games found matching the selected criteria.' | translate }}</p>
            </div>
          </ng-template>

          <!-- Pagination -->
          <div class="pagination-row" *ngIf="totalPages > 1">
            <button
              class="page-nav-btn"
              [disabled]="currentPage === 1"
              (click)="changePage(currentPage - 1)"
            >&lt;</button>
            <button
              *ngFor="let p of pageNumbers"
              class="page-num-btn"
              [class.active]="currentPage === p"
              (click)="changePage(p)"
            >{{ p }}</button>
            <button
              class="page-nav-btn"
              [disabled]="currentPage === totalPages"
              (click)="changePage(currentPage + 1)"
            >&gt;</button>
          </div>
        </div>

        <!-- Right: Refine Results Sidebar -->
        <aside class="sidebar">
          <h2 class="sidebar-title">{{ 'Refine Results' | translate }}</h2>

          <!-- Sorting -->
          <div class="filter-group">
            <label class="filter-label">{{ 'Sorting' | translate }}</label>
            <select class="filter-select" [(ngModel)]="filter.sortBy" (change)="applyFilter()">
              <option value="">{{ 'Most Popular' | translate }}</option>
              <option *ngFor="let opt of sortingOptions" [value]="opt">{{ opt | translate }}</option>
            </select>
          </div>

          <!-- Price Range -->
          <div class="filter-group">
            <label class="filter-label">{{ 'Price Range' | translate }}</label>
            <div class="price-row">
              <div class="price-col">
                <span class="price-tag">{{ 'From' | translate }}</span>
                <input type="number" class="price-input" [(ngModel)]="filter.minPrice" placeholder="0" (change)="applyFilter()" />
              </div>
              <div class="price-col">
                <span class="price-tag">{{ 'To' | translate }}</span>
                <input type="number" class="price-input" [(ngModel)]="filter.maxPrice" placeholder="100" (change)="applyFilter()" />
              </div>
            </div>
          </div>

          <!-- Publisher -->
          <div class="filter-group" *ngIf="publishers.length > 0">
            <label class="filter-label">{{ 'Publisher' | translate }}</label>
            <div class="checkbox-list">
              <label *ngFor="let pub of publishers" class="checkbox-item">
                <input
                  type="checkbox"
                  [checked]="isPublisherSelected(pub.id, pub.companyName)"
                  (change)="togglePublisher(pub.id, pub.companyName)"
                />
                <span>{{ pub.companyName }}</span>
              </label>
            </div>
          </div>

          <!-- Release Date -->
          <div class="filter-group" *ngIf="publishDateOptions.length > 0">
            <label class="filter-label">{{ 'Release Date' | translate }}</label>
            <div class="radio-list">
              <label class="radio-item">
                <input
                  type="radio"
                  name="pubDate"
                  [value]="''"
                  [(ngModel)]="filter.publishDate"
                  (change)="applyFilter()"
                />
                <span>{{ 'All' | translate }}</span>
              </label>
              <label *ngFor="let opt of publishDateOptions" class="radio-item">
                <input
                  type="radio"
                  name="pubDate"
                  [value]="opt"
                  [(ngModel)]="filter.publishDate"
                  (change)="applyFilter()"
                />
                <span>{{ formatPublishDateLabel(opt) | translate }}</span>
              </label>
            </div>
          </div>

          <!-- Genre -->
          <div class="filter-group" *ngIf="genres.length > 0">
            <label class="filter-label">{{ 'Genre' | translate }}</label>
            <div class="checkbox-list scrollable-list">
              <label *ngFor="let gen of genres" class="checkbox-item">
                <input
                  type="checkbox"
                  [checked]="isGenreSelected(gen.id, gen.name)"
                  (change)="toggleGenre(gen.id, gen.name)"
                />
                <span>{{ gen.name }}</span>
              </label>
            </div>
          </div>

          <!-- Platform -->
          <div class="filter-group" *ngIf="platforms.length > 0">
            <label class="filter-label">{{ 'Platform' | translate }}</label>
            <div class="checkbox-list">
              <label *ngFor="let plat of platforms" class="checkbox-item">
                <input
                  type="checkbox"
                  [checked]="isPlatformSelected(plat.id, plat.type)"
                  (change)="togglePlatform(plat.id, plat.type)"
                />
                <span>{{ plat.type }}</span>
              </label>
            </div>
          </div>
        </aside>
      </div>
    </div>
  `,
  styles: [`
    .page-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 32px 24px 64px;
      font-family: 'Roboto', sans-serif;
    }

    /* Header Row */
    .header-row {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 28px;
    }
    .page-title {
      font-size: 28px;
      font-weight: 800;
      color: #222;
      margin: 0;
    }
    .search-box {
      display: flex;
      align-items: center;
      background: #f5f5f5;
      border: 1px solid #ddd;
      border-radius: 4px;
      padding: 4px 8px;
      width: 280px;
      transition: border-color 0.2s;
    }
    .search-box:focus-within {
      border-color: #f57c00;
      background: #fff;
    }
    .search-box input {
      border: none;
      background: transparent;
      padding: 6px 8px;
      font-size: 14px;
      outline: none;
      width: 100%;
      font-family: inherit;
    }
    .search-btn {
      background: none;
      border: none;
      color: #777;
      cursor: pointer;
      display: flex;
      align-items: center;
      padding: 4px;
    }
    .search-btn svg {
      width: 18px;
      height: 18px;
    }
    .search-btn:hover {
      color: #f57c00;
    }

    /* Content Layout */
    .content-layout {
      display: grid;
      grid-template-columns: 1fr 280px;
      gap: 36px;
      align-items: flex-start;
    }

    /* Main Content & Game Grid */
    .main-content {
      min-width: 0;
    }
    .game-grid {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: 20px;
    }

    /* Game Card */
    .game-card {
      background: #ffffff;
      border-radius: 4px;
      overflow: hidden;
      cursor: pointer;
      display: flex;
      flex-direction: column;
      transition: transform 0.2s ease, box-shadow 0.2s ease;
      border: 1px solid #f0f0f0;
    }
    .game-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 6px 16px rgba(0, 0, 0, 0.1);
    }
    .game-image-wrap {
      position: relative;
      width: 100%;
      height: 280px;
      aspect-ratio: 2 / 3;
      background: #111;
      overflow: hidden;
    }
    .game-img {
      width: 100%;
      height: 100%;
      object-fit: cover;
      display: block;
    }
    .card-discount-tag {
      position: absolute;
      top: 8px;
      right: 8px;
      background: #e53935;
      color: #fff;
      font-size: 11px;
      font-weight: 700;
      padding: 2px 6px;
      border-radius: 3px;
    }
    .game-card-body {
      padding: 12px;
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    .game-name {
      font-size: 14px;
      font-weight: 700;
      color: #222;
      margin: 0;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
    .game-platform {
      font-size: 12px;
      color: #888;
      margin: 0;
    }
    .price-line {
      display: flex;
      align-items: center;
      gap: 6px;
      margin-top: 4px;
    }
    .old-price {
      text-decoration: line-through;
      color: #999;
      font-size: 12px;
    }
    .discounted-price {
      color: #e65100;
      font-weight: 700;
      font-size: 14px;
    }
    .current-price {
      color: #222;
      font-weight: 700;
      font-size: 14px;
    }

    .no-games-box {
      padding: 48px;
      text-align: center;
      color: #888;
      background: #f9f9f9;
      border-radius: 4px;
    }

    /* Pagination */
    .pagination-row {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 6px;
      margin-top: 36px;
    }
    .page-nav-btn, .page-num-btn {
      width: 32px;
      height: 32px;
      border: 1px solid #ddd;
      background: #fff;
      color: #444;
      font-size: 13px;
      font-weight: 600;
      border-radius: 3px;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 0.2s;
    }
    .page-nav-btn:hover:not(:disabled), .page-num-btn:hover {
      border-color: #f57c00;
      color: #f57c00;
    }
    .page-num-btn.active {
      background: #f57c00;
      color: #fff;
      border-color: #f57c00;
    }
    .page-nav-btn:disabled {
      opacity: 0.4;
      cursor: not-allowed;
    }

    /* Sidebar - Refine Results */
    .sidebar {
      background: #fafafa;
      border: 1px solid #eee;
      border-radius: 6px;
      padding: 24px;
    }
    .sidebar-title {
      font-size: 18px;
      font-weight: 800;
      color: #222;
      margin: 0 0 20px 0;
      padding-bottom: 12px;
      border-bottom: 1px solid #e0e0e0;
    }
    .filter-group {
      margin-bottom: 22px;
    }
    .filter-label {
      display: block;
      font-size: 13px;
      font-weight: 700;
      color: #444;
      margin-bottom: 8px;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }
    .filter-select {
      width: 100%;
      padding: 8px 10px;
      border: 1px solid #ccc;
      border-radius: 4px;
      background: #fff;
      font-size: 13px;
      color: #333;
      outline: none;
    }
    .filter-select:focus {
      border-color: #f57c00;
    }
    .price-row {
      display: flex;
      gap: 12px;
    }
    .price-col {
      flex: 1;
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    .price-tag {
      font-size: 11px;
      color: #888;
    }
    .price-input {
      width: 100%;
      padding: 6px 8px;
      border: 1px solid #ccc;
      border-radius: 4px;
      font-size: 13px;
      box-sizing: border-box;
      outline: none;
    }
    .price-input:focus {
      border-color: #f57c00;
    }

    .checkbox-list, .radio-list {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }
    .scrollable-list {
      max-height: 220px;
      overflow-y: auto;
      padding-right: 4px;
    }
    .checkbox-item, .radio-item {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 13px;
      color: #555;
      cursor: pointer;
    }
    .checkbox-item input, .radio-item input {
      accent-color: #f57c00;
      cursor: pointer;
    }
    .checkbox-item:hover, .radio-item:hover {
      color: #222;
    }

    /* Responsive */
    @media (max-width: 1100px) {
      .game-grid {
        grid-template-columns: repeat(3, 1fr);
      }
    }
    @media (max-width: 900px) {
      .content-layout {
        grid-template-columns: 1fr;
      }
      .game-grid {
        grid-template-columns: repeat(2, 1fr);
      }
      .sidebar {
        order: -1;
      }
    }
    @media (max-width: 600px) {
      .game-grid {
        grid-template-columns: 1fr;
      }
      .header-row {
        flex-direction: column;
        align-items: flex-start;
        gap: 16px;
      }
      .search-box {
        width: 100%;
      }
    }
  `]
})
export class GameListComponent implements OnInit, OnDestroy {
  games: Game[] = [];

  sortingOptions: string[] = [];
  publishDateOptions: string[] = [];
  paginationOptions: string[] = [];
  publishers: Publisher[] = [];
  genres: Genre[] = [];
  platforms: Platform[] = [];

  totalPages: number = 1;
  currentPage: number = 1;

  filter: GameFilter = {
    name: '',
    sortBy: '',
    minPrice: null,
    maxPrice: null,
    publishers: [],
    publishDate: '',
    genres: [],
    platforms: [],
    page: 1,
    pageSize: '12'
  };

  private destroy$ = new Subject<void>();

  get pageNumbers(): number[] {
    const pages: number[] = [];
    for (let i = 1; i <= this.totalPages; i++) {
      pages.push(i);
    }
    return pages;
  }

  constructor(
    private gameService: GameService,
    private genreService: GenreService,
    private platformService: PlatformService,
    private publisherService: PublisherService,
    private gameImageService: GameImageService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    this.loadOptions();
  }

  loadOptions(): void {
    forkJoin({
      publishers: this.publisherService.getPublishers(),
      genres: this.genreService.getGenres(),
      platforms: this.platformService.getPlatforms()
    }).pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (res) => {
        this.publishers = res.publishers || [];
        this.genres = res.genres || [];
        this.platforms = res.platforms || [];
        this.loadLocalOptions();
        this.applyFilter();
      },
      error: () => {
        this.loadLocalOptions();
        this.applyFilter();
      }
    });
  }

  loadLocalOptions(): void {
    this.gameService.getSortingOptions().pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => this.sortingOptions = res || [],
      error: () => this.sortingOptions = []
    });

    this.gameService.getPublishDateOptions().pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => this.publishDateOptions = res || [],
      error: () => this.publishDateOptions = []
    });

    this.gameService.getPaginationOptions().pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => this.paginationOptions = res || [],
      error: () => this.paginationOptions = ['10', '20', '50']
    });
  }

  applyFilter(): void {
    this.filter.page = 1;
    this.fetchGames();
  }

  fetchGames(): void {
    this.gameService.getGames(this.filter).pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (response) => {
        if (response && (response as any).games) {
          const res = response as GameFilterResult;
          this.games = res.games || [];
          this.totalPages = res.totalPages || 1;
          this.currentPage = res.currentPage || 1;
        } else if (Array.isArray(response)) {
          this.games = response;
          this.totalPages = 1;
          this.currentPage = 1;
        } else {
          this.games = [];
        }
      },
      error: () => {
        this.games = [];
      }
    });
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.filter.page = page;
    this.fetchGames();
  }

  togglePublisher(id: string, name?: string): void {
    this.toggleArrayItem(this.filter.publishers!, id);
    if (name) {
      this.toggleArrayItem(this.filter.publishers!, name);
    }
    this.applyFilter();
  }

  isPublisherSelected(id: string, name?: string): boolean {
    return !!(this.filter.publishers?.includes(id) || (name && this.filter.publishers?.includes(name)));
  }

  toggleGenre(id: string, name?: string): void {
    this.toggleArrayItem(this.filter.genres!, id);
    if (name) {
      this.toggleArrayItem(this.filter.genres!, name);
    }
    this.applyFilter();
  }

  isGenreSelected(id: string, name?: string): boolean {
    return !!(this.filter.genres?.includes(id) || (name && this.filter.genres?.includes(name)));
  }

  togglePlatform(id: string, type?: string): void {
    this.toggleArrayItem(this.filter.platforms!, id);
    if (type) {
      this.toggleArrayItem(this.filter.platforms!, type);
    }
    this.applyFilter();
  }

  isPlatformSelected(id: string, type?: string): boolean {
    return !!(this.filter.platforms?.includes(id) || (type && this.filter.platforms?.includes(type)));
  }

  private toggleArrayItem(array: string[], item: string): void {
    const idx = array.indexOf(item);
    if (idx > -1) {
      array.splice(idx, 1);
    } else {
      array.push(item);
    }
  }

  getImageUrl(key: string, name: string): string {
    return this.gameImageService.getGameImage(key, name);
  }

  handleImgError(event: Event, key?: string, name?: string): void {
    const img = event.target as HTMLImageElement;
    if (img && !img.dataset['fallback']) {
      img.dataset['fallback'] = 'true';
      img.src = this.gameImageService.getOnlineFallbackImage(key, name);
    }
  }

  getPlatformText(game: any): string {
    if (game.platforms && game.platforms.length > 0) {
      return game.platforms.map((p: any) => p.type || p).join(', ');
    }
    if (game.platformTypes && game.platformTypes.length > 0) {
      return game.platformTypes.join(', ');
    }
    return 'Desktop, Console';
  }

  formatPublishDateLabel(opt: string): string {
    if (!opt) return 'All';
    switch (opt.toLowerCase()) {
      case 'last week': return 'Last 7 Days';
      case 'last month': return 'Last 30 Days';
      case 'last year': return 'Last Year';
      case '2 years': return 'Last 2 Years';
      case '3 years': return 'Last 3 Years';
      default: return opt;
    }
  }

  addToCart(event: Event, key: string): void {
    event.stopPropagation();
    event.preventDefault();
    this.cartService.addToCart(key).subscribe({
      next: () => {},
      error: (err) => console.error('Add to cart failed', err)
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
