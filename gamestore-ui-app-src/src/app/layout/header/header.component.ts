import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <header class="app-header">
      <div class="header-container">
        <a routerLink="/" class="logo">
          <img src="assets/logo.png?v=4" alt="GameStore" class="logo-img" (error)="useTextLogo = true" *ngIf="!useTextLogo; else textLogo" />
          <ng-template #textLogo><span class="logo-text">logo</span></ng-template>
        </a>
        
        <nav class="nav-links">
          <a routerLink="/games" routerLinkActive="active" class="nav-item">GAMES</a>
          <a routerLink="/genres" routerLinkActive="active" class="nav-item">GENRES</a>
          <a routerLink="/publishers" routerLinkActive="active" class="nav-item">PUBLISHERS</a>
          <a routerLink="/orders" routerLinkActive="active" class="nav-item">ORDERS</a>
          <a routerLink="/basket" routerLinkActive="active" class="nav-item cart-item">
            CART<span *ngIf="(cartCount$ | async) as count" class="cart-count"> ({{ count }})</span>
          </a>
        </nav>
      </div>
    </header>
  `,
  styles: [`
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

  constructor(private cartService: CartService) {}

  ngOnInit(): void {
    this.cartCount$ = this.cartService.cartCount$;
  }
}
