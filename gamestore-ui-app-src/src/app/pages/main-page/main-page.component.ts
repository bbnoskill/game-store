import { TranslatePipe } from '../../shared/pipes/translate.pipe';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { GameImageService } from '../../services/game-image.service';

@Component({
  selector: 'app-main-page',
  standalone: true,
  imports: [TranslatePipe, CommonModule, RouterModule],
  template: `
    <div class="main-page-container">
      <div class="intro-grid">
        <!-- Left Column: Article -->
        <div class="article-column">
          <h2 class="section-title">{{ 'Do You Know Who is John Rembo?' | translate }}</h2>
          <p class="article-lead">
            Donec urna sapien, porta nec condimentum id, elementum in quam. Curabitur a aliquet turpis, sagittis eleifend sem. Nam quis est eget neque euismod fringilla et sit amet mauris. Proin commodo sodales arcu, a volutpat augue facilisis et. Nam pellentesque finibus felis eget feugiat. Nulla convallis a nisl non vehicula. Aliquam erat volutpat. Vivamus id massa a lacus tincidunt tempor at eu ex.
          </p>
          <ul class="bullet-list">
            <li>Proin commodo sodales arcu, a volutpat augue facilisis et. Nam pellentesque finibus felis eget feugiat. Nulla convallis a nisl non vehicula. Aliquam erat volutpat.</li>
            <li>Nam pellentesque finibus felis eget feugiat. Nulla convallis a nisl non vehicula. Aliquam erat volutpat.</li>
            <li>Proin commodo sodales arcu, a volutpat. Nulla convallis a nisl non vehicula. Aliquam erat volutpat. Praesent nec tincidunt purus. Donec vel erat porta.</li>
          </ul>
          <a routerLink="/games" class="btn-orange">{{ 'Learn More' | translate }}</a>
        </div>

        <!-- Right Column: Featured Banner -->
        <div class="featured-column">
          <h2 class="section-title">{{ 'We Knocked Em\\'ll' | translate }}</h2>
          <div class="featured-card" routerLink="/games">
            <img [src]="vrImage" (error)="onImgError($event)" alt="Sony Playstation VR" class="featured-img" />
            <div class="featured-overlay">
              <h3 class="featured-title">Sony Playstation VR <span class="arrow">&gt;</span></h3>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .main-page-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 48px 24px;
    }
    .intro-grid {
      display: grid;
      grid-template-columns: 1.6fr 1fr;
      gap: 48px;
      align-items: flex-start;
    }
    .section-title {
      font-size: 26px;
      font-weight: 800;
      color: #222;
      margin: 0 0 20px 0;
      font-family: 'Roboto', sans-serif;
    }
    .article-lead {
      font-size: 14px;
      line-height: 1.6;
      color: #555;
      margin-bottom: 18px;
    }
    .bullet-list {
      list-style: none;
      padding: 0;
      margin: 0 0 28px 0;
      display: flex;
      flex-direction: column;
      gap: 12px;
    }
    .bullet-list li {
      position: relative;
      padding-left: 20px;
      font-size: 14px;
      line-height: 1.5;
      color: #555;
    }
    .bullet-list li::before {
      content: '•';
      position: absolute;
      left: 0;
      top: -2px;
      color: #f57c00;
      font-size: 18px;
    }
    .btn-orange {
      display: inline-block;
      background-color: #f57c00;
      color: #ffffff;
      text-decoration: none;
      padding: 12px 32px;
      font-size: 14px;
      font-weight: 600;
      border-radius: 2px;
      transition: background-color 0.2s ease;
    }
    .btn-orange:hover {
      background-color: #e65100;
    }

    /* Featured Card */
    .featured-card {
      position: relative;
      border-radius: 4px;
      overflow: hidden;
      cursor: pointer;
      box-shadow: 0 4px 12px rgba(0,0,0,0.12);
      transition: transform 0.2s ease, box-shadow 0.2s ease;
    }
    .featured-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 20px rgba(0,0,0,0.18);
    }
    .featured-img {
      width: 100%;
      height: 340px;
      object-fit: cover;
      display: block;
    }
    .featured-overlay {
      position: absolute;
      bottom: 0;
      left: 0;
      right: 0;
      padding: 24px;
      background: linear-gradient(0deg, rgba(0,0,0,0.85) 0%, transparent 100%);
    }
    .featured-title {
      color: #ffffff;
      font-size: 20px;
      font-weight: 700;
      margin: 0;
      display: flex;
      align-items: center;
      gap: 8px;
    }
    .arrow {
      color: #ffffff;
      font-weight: 800;
    }

    @media (max-width: 860px) {
      .intro-grid {
        grid-template-columns: 1fr;
        gap: 32px;
      }
    }
  `]
})
export class MainPageComponent {
  vrImage: string;

  constructor(private imageService: GameImageService) {
    this.vrImage = this.imageService.getFeaturedCardImage();
  }

  onImgError(event: Event): void {
    (event.target as HTMLImageElement).src = this.imageService.getFallbackImage();
  }
}
