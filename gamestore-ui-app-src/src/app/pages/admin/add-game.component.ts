import { TranslatePipe } from '../../shared/pipes/translate.pipe';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GameService } from '../../services/game.service';
import { GenreService } from '../../services/genre.service';
import { PlatformService } from '../../services/platform.service';
import { PublisherService } from '../../services/publisher.service';
import { Genre } from '../../models/genre.model';
import { Platform } from '../../models/platform.model';
import { Publisher } from '../../models/publisher.model';

@Component({
  selector: 'app-add-game',
  standalone: true,
  imports: [TranslatePipe, CommonModule, RouterModule, FormsModule],
  template: `
    <div class="page-container">
      <p *ngIf="errorMessage" role="alert">{{ errorMessage | translate }}</p>
      <h1 class="page-title">{{ 'Add New Game' | translate }}</h1>

      <form (ngSubmit)="onSubmit()" #gameForm="ngForm" class="game-form-card">
        <div class="form-grid">
          <div class="form-group">
            <label for="name">{{ 'Game Name *' | translate }}</label>
            <input
              type="text"
              id="name"
              name="name"
              [(ngModel)]="name"
              required
              class="form-input"
              [placeholder]="'e.g. Cyberpunk 2077' | translate"
              (input)="generateKey()"
            />
          </div>

          <div class="form-group">
            <label for="key">{{ 'Unique Key *' | translate }}</label>
            <input
              type="text"
              id="key"
              name="key"
              [(ngModel)]="key"
              required
              class="form-input"
              [placeholder]="'e.g. cyberpunk-2077' | translate"
            />
          </div>

          <div class="form-group">
            <label for="price">{{ 'Price ($) *' | translate }}</label>
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
            <label for="discount">{{ 'Discount (%)' | translate }}</label>
            <input
              type="number"
              id="discount"
              name="discount"
              [(ngModel)]="discount"
              min="0"
              max="100"
              class="form-input"
              placeholder="0"
            />
          </div>

          <div class="form-group">
            <label for="unitInStock">{{ 'Unit In Stock' | translate }}</label>
            <input
              type="number"
              id="unitInStock"
              name="unitInStock"
              [(ngModel)]="unitInStock"
              min="0"
              class="form-input"
              placeholder="10"
            />
          </div>

          <div class="form-group">
            <label for="publisher">{{ 'Publisher' | translate }}</label>
            <select id="publisher" name="publisher" [(ngModel)]="publisherId" class="form-select">
              <option value="">{{ 'Select Publisher' | translate }}</option>
              <option *ngFor="let pub of publishers" [value]="pub.id">{{ pub.companyName }}</option>
            </select>
          </div>
        </div>

        <div class="form-group full-width">
          <label for="description">{{ 'Description' | translate }}</label>
          <textarea
            id="description"
            name="description"
            [(ngModel)]="description"
            rows="5"
            class="form-input"
            [placeholder]="'Enter game description...' | translate"
          ></textarea>
        </div>

        <!-- Genres Selection -->
        <div class="form-group full-width" *ngIf="genres.length > 0">
          <label>{{ 'Genres' | translate }}</label>
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
          <label>{{ 'Platforms' | translate }}</label>
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
            {{ (submitting ? 'Adding Game...' : 'Add Game') | translate }}
          </button>
          <a routerLink="/games" class="btn-cancel">{{ 'Cancel' | translate }}</a>
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
  `]
})
export class AddGameComponent implements OnInit {
  errorMessage = '';
  name: string = '';
  key: string = '';
  price: number = 59.99;
  discount: number = 0;
  unitInStock: number = 10;
  description: string = '';
  publisherId: string = '';

  genres: Genre[] = [];
  platforms: Platform[] = [];
  publishers: Publisher[] = [];

  selectedGenres: string[] = [];
  selectedPlatforms: string[] = [];
  submitting: boolean = false;

  constructor(
    private gameService: GameService,
    private genreService: GenreService,
    private platformService: PlatformService,
    private publisherService: PublisherService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.genreService.getGenres().subscribe(res => this.genres = res || []);
    this.platformService.getPlatforms().subscribe(res => this.platforms = res || []);
    this.publisherService.getPublishers().subscribe(res => this.publishers = res || []);
  }

  generateKey(): void {
    if (this.name) {
      this.key = this.name.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/(^-|-$)/g, '');
    }
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
    if (!this.name || !this.key) return;
    this.errorMessage = '';
    this.submitting = true;

    const request = {
      game: {
        name: this.name,
        key: this.key,
        price: this.price,
        discount: this.discount,
        unitInStock: this.unitInStock,
        description: this.description,
        publisherId: this.publisherId || undefined
      },
      publisher: this.publisherId || undefined,
      genres: this.selectedGenres,
      platforms: this.selectedPlatforms
    };

    this.gameService.createGame(request).subscribe({
      next: () => {
        this.submitting = false;
        this.router.navigate(['/game', this.key]);
      },
      error: () => {
        this.submitting = false;
        this.errorMessage = 'Could not save changes. Please try again.';
      }
    });
  }
}
