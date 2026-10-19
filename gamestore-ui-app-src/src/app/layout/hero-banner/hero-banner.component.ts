import { TranslatePipe } from '../../shared/pipes/translate.pipe';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-hero-banner',
  standalone: true,
  imports: [TranslatePipe, CommonModule],
  template: `
    <section class="hero-banner">
      <div class="hero-overlay"></div>
      <div class="hero-content">
        <h1 class="hero-title">{{ 'What Can Make Your Life Easier?' | translate }}</h1>
        <p class="hero-subtitle">{{ 'Empowered Solutions with Simple Process' | translate }}</p>
      </div>
    </section>
  `,
  styles: [`
    .hero-banner {
      position: relative;
      width: 100%;
      height: 140px;
      min-height: 120px;
      background: #111 url('https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=1920&q=80') center center / cover no-repeat;
      display: flex;
      align-items: flex-end;
      overflow: hidden;
    }
    .hero-overlay {
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: linear-gradient(
        180deg,
        rgba(10, 11, 13, 0.45) 0%,
        rgba(10, 11, 13, 0.75) 70%,
        rgba(10, 11, 13, 0.95) 100%
      );
    }
    .hero-content {
      position: relative;
      z-index: 2;
      max-width: 1200px;
      width: 100%;
      margin: 0 auto;
      padding: 0 24px 16px 24px;
    }
    .hero-title {
      font-size: 24px;
      font-weight: 800;
      color: #ffffff;
      margin: 0 0 4px 0;
      line-height: 1.15;
      font-family: 'Roboto', sans-serif;
    }
    .hero-subtitle {
      font-size: 13px;
      font-weight: 400;
      color: #e0e0e0;
      margin: 0;
      letter-spacing: 0.2px;
    }

    @media (max-width: 768px) {
      .hero-banner {
        height: 100px;
        min-height: 90px;
      }
      .hero-title {
        font-size: 18px;
      }
      .hero-subtitle {
        font-size: 12px;
      }
    }
  `]
})
export class HeroBannerComponent {}
