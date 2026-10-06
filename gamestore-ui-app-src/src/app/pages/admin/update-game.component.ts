import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GameService } from '../../services/game.service';
import { GenreService } from '../../services/genre.service';
import { PlatformService } from '../../services/platform.service';
import { PublisherService } from '../../services/publisher.service';
import { Game } from '../../models/game.model';
import { Genre } from '../../models/genre.model';
import { Platform } from '../../models/platform.model';
import { Publisher } from '../../models/publisher.model';

@Component({
  selector: 'app-update-game',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="page-container">
      <h1 class="page-title">Update Game</h1>

      <div class="loading-state" *ngIf="loading">
        <p>Loading game details...</p>
      </div>

      <form (ngSubmit)="onSubmit()" #gameForm="ngForm" class="game-form-card" *ngIf="!loading && game">
        <div class="form-grid">
          <div class="form-group">
            <label for="name">Game Name *</label>
            <input
              type="text"
              id="name"
              name="name"
              [(ngModel)]="name"
              required
              class="form-input"
            />
          </div>

          <div class="form-group">
            <label for="key">Unique Key *</label>
            <input
              type="text"
              id="key"
              name="key"
              [(ngModel)]="key"
              required
              class="form-input"
            />
          </div>

          <div class="form-group">
            <label for="price">Price ($) *</label>
            <input
              type="number"
              id="price"
              name="price"
              [(ngModel)]="price"
              required
              min="0"
              step="0.01"
              class="form-input"
            />
          </div>

          <div class="form-group">
            <label for="discount">Discount (%)</label>
            <input
              type="number"
              id="discount"
              name="discount"
              [(ngModel)]="discount"
              min="0"
              max="100"
              class="form-input"
            />
          </div>

          <div class="form-group">
            <label for="unitInStock">Unit In Stock</label>
            <input
              type="number"
              id="unitInStock"
              name="unitInStock"
              [(ngModel)]="unitInStock"
              min="0"
              class="form-input"
            />
          </div>

          <div class="form-group">
            <label for="publisher">Publisher</label>
            <select id="publisher" name="publisher" [(ngModel)]="publisherId" class="form-select">
              <option value="">Select Publisher</option>
              <option *ngFor="let pub of publishers" [value]="pub.id">{{ pub.companyName }}</option>
            </select>
          </div>
        </div>

        <div class="form-group full-width">
          <label for="description">Description</label>
          <textarea
            id="description"
            name="description"
            [(ngModel)]="description"
            rows="5"
            class="form-input"
          ></textarea>
        </div>

        <!-- Genres Selection -->
        <div class="form-group full-width" *ngIf="genres.length > 0">
          <label>Genres</label>
          <div class="checkbox-grid">
            <label *ngFor="let g of genres" class="checkbox-label">
              <input
                type="checkbox"
                [checked]="selectedGenres.includes(g.id)"
                (change)="toggleGenre(g.id)"
              />
              {{ g.name }}
            </label>
          </div>
        </div>

        <!-- Platforms Selection -->
        <div class="form-group full-width" *ngIf="platforms.length > 0">
          <label>Platforms</label>
          <div class="checkbox-grid">
            <label *ngFor="let p of platforms" class="checkbox-label">
              <input
                type="checkbox"
                [checked]="selectedPlatforms.includes(p.id)"
                (change)="togglePlatform(p.id)"
              />
              {{ p.type }}
            </label>
          </div>
        </div>

        <div class="form-actions">
          <button type="submit" class="btn-submit" [disabled]="!gameForm.form.valid || submitting">
            {{ submitting ? 'Updating Game...' : 'Save Changes' }}
          </button>
          <a [routerLink]="['/game', originalKey]" class="btn-cancel">Cancel</a>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .page-container {
      max-width: 900px;
      margin: 0 auto;
      padding: 32px 24px 64px;
      font-family: 'Roboto', sans-serif;
    }

    .page-title {
      font-size: 32px;
      font-weight: 700;
      color: #222;
      margin-bottom: 24px;
      border-bottom: 2px solid #eee;
      padding-bottom: 16px;
    }

    .game-form-card {
      background: #fff;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      padding: 32px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.04);
    }

    .form-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 20px;
      margin-bottom: 20px;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }

    .full-width {
      margin-bottom: 20px;
    }

    .form-group label {
      font-size: 14px;
      font-weight: 600;
      color: #444;
    }

    .form-input, .form-select {
      padding: 10px 14px;
      border: 1px solid #ccc;
      border-radius: 6px;
      font-size: 15px;
    }

    .checkbox-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(160px, 1fr));
      gap: 10px;
      background: #f9f9f9;
      padding: 16px;
      border-radius: 6px;
      border: 1px solid #eee;
    }

    .checkbox-label {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 14px;
      cursor: pointer;
    }

    .form-actions {
      display: flex;
      gap: 16px;
      margin-top: 32px;
    }

    .btn-submit {
      background: #f57c00;
      color: #fff;
      border: none;
      padding: 12px 28px;
      border-radius: 6px;
      font-size: 16px;
      font-weight: 600;
      cursor: pointer;
    }

    .btn-cancel {
      display: inline-block;
      background: #e0e0e0;
      color: #333;
      text-decoration: none;
      padding: 12px 24px;
      border-radius: 6px;
      font-size: 16px;
    }

    .loading-state {
      text-align: center;
      padding: 48px;
      color: #666;
    }
  `]
})
export class UpdateGameComponent implements OnInit {
  originalKey: string = '';
  game: Game | null = null;
  loading: boolean = true;

  name: string = '';
  key: string = '';
  price: number = 0;
  discount: number = 0;
  unitInStock: number = 0;
  description: string = '';
  publisherId: string = '';

  genres: Genre[] = [];
  platforms: Platform[] = [];
  publishers: Publisher[] = [];

  selectedGenres: string[] = [];
  selectedPlatforms: string[] = [];
  submitting: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private gameService: GameService,
    private genreService: GenreService,
    private platformService: PlatformService,
    private publisherService: PublisherService
  ) {}

  ngOnInit(): void {
    const keyParam = this.route.snapshot.paramMap.get('key');
    if (keyParam) {
      this.originalKey = keyParam;
      this.loadData(keyParam);
    } else {
      this.loading = false;
    }
  }

  loadData(key: string): void {
    this.loading = true;
    this.genreService.getGenres().subscribe(res => this.genres = res || []);
    this.platformService.getPlatforms().subscribe(res => this.platforms = res || []);
    this.publisherService.getPublishers().subscribe(res => this.publishers = res || []);

    this.gameService.getGameByKey(key).subscribe({
      next: (game) => {
        this.game = game;
        this.name = game.name;
        this.key = game.key;
        this.price = game.price;
        this.discount = game.discount || 0;
        this.unitInStock = game.unitInStock || 0;
        this.description = game.description || '';
        this.loading = false;

        // Fetch genres/platforms/publisher for this game
        this.gameService.getGenresByGameKey(key).subscribe((g: any[]) => this.selectedGenres = (g || []).map((x: any) => x.id));
        this.gameService.getPlatformsByGameKey(key).subscribe((p: any[]) => this.selectedPlatforms = (p || []).map((x: any) => x.id));
        this.gameService.getPublisherByGameKey(key).subscribe((pub: any) => {
          if (pub) this.publisherId = pub.id;
        });
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  toggleGenre(id: string): void {
    if (this.selectedGenres.includes(id)) {
      this.selectedGenres = this.selectedGenres.filter(g => g !== id);
    } else {
      this.selectedGenres.push(id);
    }
  }

  togglePlatform(id: string): void {
    if (this.selectedPlatforms.includes(id)) {
      this.selectedPlatforms = this.selectedPlatforms.filter(p => p !== id);
    } else {
      this.selectedPlatforms.push(id);
    }
  }

  onSubmit(): void {
    if (!this.game || !this.name || !this.key) return;
    this.submitting = true;

    const request = {
      game: {
        id: this.game.id,
        name: this.name,
        key: this.key,
        price: this.price,
        discount: this.discount,
        unitInStock: this.unitInStock,
        description: this.description,
        publisherId: this.publisherId || undefined
      },
      genres: this.selectedGenres,
      platforms: this.selectedPlatforms
    };

    this.gameService.updateGame(request).subscribe({
      next: () => {
        this.submitting = false;
        this.router.navigate(['/game', this.key]);
      },
      error: () => {
        this.submitting = false;
        this.router.navigate(['/games']);
      }
    });
  }
}
