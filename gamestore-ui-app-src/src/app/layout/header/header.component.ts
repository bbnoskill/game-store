import { TranslatePipe } from '../../shared/pipes/translate.pipe';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { LanguageService } from '../../services/language.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [TranslatePipe, CommonModule, RouterModule],
  template: `
    <header class="app-header">
      <div class="header-container">
        <a routerLink="/" class="logo">
          <img src="assets/logo.png?v=4" alt="GameStore" class="logo-img" (error)="useTextLogo = true" *ngIf="!useTextLogo; else textLogo" />
          <ng-template #textLogo><span class="logo-text">logo</span></ng-template>
        </a>
        
        <nav class="nav-links">
          <a routerLink="/games" routerLinkActive="active" class="nav-item">{{ 'GAMES' | translate }}</a>
          <a routerLink="/genres" routerLinkActive="active" class="nav-item">{{ 'GENRES' | translate }}</a>
          <a routerLink="/publishers" routerLinkActive="active" class="nav-item">{{ 'PUBLISHERS' | translate }}</a>
          <a routerLink="/orders" routerLinkActive="active" class="nav-item">{{ 'ORDERS' | translate }}</a>
          <a routerLink="/basket" routerLinkActive="active" class="nav-item cart-item">
            {{ 'CART' | translate }}<span *ngIf="(cartCount$ | async) as count" class="cart-count"> ({{ count }})</span>
          </a>
        </nav>
        <label class="language-picker">
          <span>{{ 'Language' | translate }}</span>
          <select [value]="language.language()" (change)="language.select($any($event.target).value)" [attr.aria-label]="'Language' | translate">
            <option *ngFor="let option of language.languages" [value]="option.code">{{ option.name }}</option>
          </select>
        </label>
      </div>
    </header>
  `,
  styles: [`
    .language-picker { display: flex; gap: 8px; align-items: center; color: white; margin-left: 16px; }
    .language-picker select { padding: 6px; color: white; background: #242529; border: 1px solid #777; border-radius: 4px; }
    .app-header {
      background-color: #121315;
      border-bottom: 1px solid rgba(255, 255, 255, 0.08);
      position: sticky;
      top: 0;
      z-index: 1000;
      width: 100%;
    }
    .header-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 8px 24px;
      display: flex;
      align-items: center;
      justify-content: space-between;
    }
    .logo {
      font-size: 28px;
      font-weight: 800;
      font-style: italic;
      color: #ffffff;
      text-decoration: none;
      letter-spacing: -0.5px;
      font-family: 'Roboto', sans-serif;
      transition: opacity 0.2s ease;
      display: flex;
      align-items: center;
    }
    .logo-img {
      height: 80px;
      max-height: 80px;
      width: auto;
      object-fit: contain;
      display: block;
    }
    .logo-text {
      font-size: 28px;
      font-weight: 800;
      font-style: italic;
      color: #ffffff;
      font-family: 'Roboto', sans-serif;
    }
    .logo:hover {
      opacity: 0.85;
    }
    .nav-links {
      display: flex;
      align-items: center;
      gap: 28px;
    }
    .nav-item {
      color: #ffffff;
      text-decoration: none;
      font-size: 13px;
      font-weight: 700;
      letter-spacing: 0.8px;
      transition: color 0.2s ease;
      cursor: pointer;
    }
    .nav-item:hover {
      color: #f57c00;
    }
    .nav-item.active {
      color: #f57c00;
    }
    .cart-item {
      font-weight: 700;
    }
    .cart-count {
      color: #f57c00;
    }

    @media (max-width: 768px) {
      .header-container {
        flex-direction: column;
        gap: 12px;
        padding: 12px 16px;
      }
      .nav-links {
        gap: 16px;
        flex-wrap: wrap;
        justify-content: center;
      }
    }
  `]
})
export class HeaderComponent implements OnInit {
  cartCount$!: Observable<number>;
  useTextLogo: boolean = false;

  constructor(private cartService: CartService, public language: LanguageService) {}

  ngOnInit(): void {
    this.cartCount$ = this.cartService.cartCount$;
  }
}
