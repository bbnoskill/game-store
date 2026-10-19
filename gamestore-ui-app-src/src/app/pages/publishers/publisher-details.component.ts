import { TranslatePipe } from '../../shared/pipes/translate.pipe';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { PublisherService } from '../../services/publisher.service';
import { GameService } from '../../services/game.service';
import { Publisher } from '../../models/publisher.model';
import { Game } from '../../models/game.model';
import { GameImageService } from '../../services/game-image.service';

@Component({
  selector: 'app-publisher-details',
  standalone: true,
  imports: [TranslatePipe, CommonModule, RouterModule],
  template: `
    <div class="page-container" *ngIf="!loading && publisher">
      <a routerLink="/publishers" class="back-link">{{ '< Back to Publishers' | translate }}</a>

      <div class="publisher-card">
        <h1 class="publisher-title">{{ publisher.companyName }}</h1>

        <div class="publisher-info-row" *ngIf="publisher.homePage">
          <strong>{{ 'Website:' | translate }}</strong>
          <a [href]="publisher.homePage" target="_blank" class="external-link">{{ publisher.homePage }}</a>
        </div>

        <div class="publisher-description" *ngIf="publisher.description">
          <h3>{{ 'About Publisher' | translate }}</h3>
          <p>{{ publisher.description }}</p>
        </div>
      </div>

      <!-- Games by Publisher Section -->
      <div class="published-games-section">
        <h2>{{ 'Games by' | translate }} {{ publisher.companyName }}</h2>

        <div class="games-grid" *ngIf="games.length > 0">
          <div class="game-card" *ngFor="let game of games">
            <img [src]="getGameImage(game)" [alt]="game.name" class="game-cover" />
            <div class="game-card-body">
              <h3 class="game-title">
                <a [routerLink]="['/game', game.key]">{{ game.name }}</a>
              </h3>
              <div class="game-price">\${{ game.price | number:'1.2-2' }}</div>
            </div>
          </div>
        </div>

        <div class="no-games" *ngIf="games.length === 0">
          <p>{{ 'No games published by' | translate }} {{ publisher.companyName }} {{ 'yet.' | translate }}</p>
        </div>
      </div>
    </div>

    <div class="loading-state" *ngIf="loading">
      <p>{{ 'Loading publisher details...' | translate }}</p>
    </div>

    <div class="error-state" *ngIf="!loading && !publisher">
      <p>{{ 'Publisher not found.' | translate }}</p>
      <a routerLink="/publishers" class="btn-browse">{{ 'Back to Publishers' | translate }}</a>
    </div>
  `,
  styles: [`
    .page-container {
      max-width: 1100px;
      margin: 0 auto;
      padding: 32px 24px 64px;
      font-family: 'Roboto', sans-serif;
    }

    .back-link {
      display: inline-block;
      color: #f57c00;
      text-decoration: none;
      font-size: 14px;
      font-weight: 500;
      margin-bottom: 24px;
    }

    .publisher-card {
      background: #fff;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      padding: 32px;
      margin-bottom: 40px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.04);
    }

    .publisher-title {
      font-size: 32px;
      font-weight: 700;
      color: #222;
      margin: 0 0 16px;
    }

    .publisher-info-row {
      font-size: 15px;
      color: #555;
      margin-bottom: 16px;
    }

    .external-link {
      color: #f57c00;
      text-decoration: none;
      margin-left: 8px;
    }

    .publisher-description h3 {
      font-size: 18px;
      font-weight: 600;
      color: #333;
      margin: 16px 0 8px;
    }

    .publisher-description p {
      color: #555;
      line-height: 1.6;
      font-size: 15px;
    }

    .published-games-section h2 {
      font-size: 24px;
      font-weight: 700;
      color: #222;
      margin-bottom: 20px;
    }

    .games-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
      gap: 24px;
    }

    .game-card {
      background: #fff;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      overflow: hidden;
      box-shadow: 0 2px 4px rgba(0,0,0,0.04);
    }

    .game-cover {
      width: 100%;
      height: 160px;
      object-fit: cover;
    }

    .game-card-body {
      padding: 16px;
    }

    .game-title {
      font-size: 16px;
      font-weight: 600;
      margin: 0 0 8px;
    }

    .game-title a {
      color: #222;
      text-decoration: none;
    }

    .game-title a:hover {
      color: #f57c00;
    }

    .game-price {
      font-size: 16px;
      font-weight: 700;
      color: #f57c00;
    }

    .loading-state, .error-state {
      text-align: center;
      padding: 64px;
      color: #666;
    }

    .btn-browse {
      display: inline-block;
      background: #f57c00;
      color: #fff;
      text-decoration: none;
      padding: 10px 24px;
      border-radius: 6px;
      margin-top: 16px;
    }
  `]
})
export class PublisherDetailsComponent implements OnInit {
  publisher: Publisher | null = null;
  games: Game[] = [];
  loading: boolean = true;

  constructor(
    private route: ActivatedRoute,
    private publisherService: PublisherService,
    private gameService: GameService,
    private gameImageService: GameImageService
  ) {}

  ngOnInit(): void {
    const companyname = this.route.snapshot.paramMap.get('companyname');
    if (companyname) {
      this.loadPublisher(companyname);
    } else {
      this.loading = false;
    }
  }

  loadPublisher(companyname: string): void {
    this.loading = true;
    this.publisherService.getPublisherByCompanyName(companyname).subscribe({
      next: (pub) => {
        this.publisher = pub;
        this.loadGamesByPublisher(pub);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  loadGamesByPublisher(pub: Publisher): void {
    this.gameService.getGames({ publishers: [pub.companyName || pub.id] }).subscribe({
      next: (res: any) => {
        this.games = Array.isArray(res) ? res : (res.games || []);
      },
      error: () => {
        this.games = [];
      }
    });
  }

  getGameImage(game: Game): string {
    return this.gameImageService.getGameImage(game.key, game.name);
  }
}
