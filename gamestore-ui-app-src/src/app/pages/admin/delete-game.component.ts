import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { GameService } from '../../services/game.service';
import { Game } from '../../models/game.model';

@Component({
  selector: 'app-delete-game',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="page-container">
      <div class="delete-card">
        <h2 class="delete-title">Delete Game</h2>

        <div class="loading-state" *ngIf="loading">
          <p>Loading game details...</p>
        </div>

        <div class="confirm-box" *ngIf="!loading && game">
          <p class="warning-text">
            Are you sure you want to delete <strong>"{{ game.name }}"</strong>? This action cannot be undone.
          </p>

          <div class="game-info-box">
            <div><strong>Key:</strong> {{ game.key }}</div>
            <div><strong>Price:</strong> \${{ game.price | number:'1.2-2' }}</div>
          </div>

          <div class="action-buttons">
            <button class="btn-confirm-delete" (click)="confirmDelete()" [disabled]="deleting">
              {{ deleting ? 'Deleting...' : 'Yes, Delete Game' }}
            </button>
            <a [routerLink]="['/game', game.key]" class="btn-cancel">Cancel</a>
          </div>
        </div>

        <div class="error-state" *ngIf="!loading && !game">
          <p>Game not found.</p>
          <a routerLink="/games" class="btn-cancel">Back to Games</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-container {
      max-width: 500px;
      margin: 64px auto;
      padding: 0 24px;
      font-family: 'Roboto', sans-serif;
    }

    .delete-card {
      background: #fff;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      padding: 32px;
      box-shadow: 0 4px 16px rgba(0,0,0,0.06);
    }

    .delete-title {
      font-size: 24px;
      font-weight: 700;
      color: #d32f2f;
      margin: 0 0 20px;
      border-bottom: 1px solid #eee;
      padding-bottom: 12px;
    }

    .warning-text {
      font-size: 16px;
      color: #333;
      margin-bottom: 20px;
      line-height: 1.5;
    }

    .game-info-box {
      background: #f9f9f9;
      border: 1px solid #e0e0e0;
      padding: 16px;
      border-radius: 6px;
      margin-bottom: 24px;
      font-size: 14px;
    }

    .action-buttons {
      display: flex;
      gap: 16px;
    }

    .btn-confirm-delete {
      flex: 1;
      background: #d32f2f;
      color: #fff;
      border: none;
      padding: 12px;
      border-radius: 6px;
      font-size: 15px;
      font-weight: 600;
      cursor: pointer;
    }

    .btn-confirm-delete:hover:not(:disabled) {
      background: #b71c1c;
    }

    .btn-cancel {
      display: inline-block;
      background: #e0e0e0;
      color: #333;
      text-decoration: none;
      padding: 12px 20px;
      border-radius: 6px;
      font-size: 15px;
      text-align: center;
    }

    .loading-state, .error-state {
      text-align: center;
      padding: 32px;
      color: #666;
    }
  `]
})
export class DeleteGameComponent implements OnInit {
  game: Game | null = null;
  loading: boolean = true;
  deleting: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private gameService: GameService
  ) {}

  ngOnInit(): void {
    const key = this.route.snapshot.paramMap.get('key');
    if (key) {
      this.gameService.getGameByKey(key).subscribe({
        next: (game) => {
          this.game = game;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
    } else {
      this.loading = false;
    }
  }

  confirmDelete(): void {
    if (!this.game) return;
    this.deleting = true;

    this.gameService.deleteGame(this.game.key).subscribe({
      next: () => {
        this.deleting = false;
        this.router.navigate(['/games']);
      },
      error: () => {
        this.deleting = false;
        this.router.navigate(['/games']);
      }
    });
  }
}
