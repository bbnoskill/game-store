import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { GameImageService } from '../../services/game-image.service';
import { CartItem, CartSummary } from '../../models/cart.model';
import { PaymentMethod } from '../../models/order.model';
import { DiscountPricePipe } from '../../shared/pipes/discount-price.pipe';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, DiscountPricePipe],
  template: `
    <div class="page-container">
      <div class="cart-header">
        <h1 class="cart-title">Cart</h1>
        <span class="cart-count-badge" *ngIf="summary">{{ summary.itemCount }} {{ summary.itemCount === 1 ? 'item' : 'items' }}</span>
      </div>

      <div class="cart-content" *ngIf="!loading && items.length > 0">
        <!-- Cart Items List -->
        <div class="cart-items-section">
          <div class="cart-item-card" *ngFor="let item of items">
            <img [src]="getItemImage(item)" [alt]="item.gameName" class="cart-item-image" />

            <div class="cart-item-info">
              <h3 class="cart-item-name">{{ item.gameName }}</h3>
              <div class="cart-item-meta">
                <span class="meta-tag" *ngIf="item.platform">{{ item.platform }}</span>
                <span class="meta-tag" *ngIf="item.fileSize">{{ item.fileSize }}</span>
              </div>
            </div>

            <!-- Price with Discount (US6) -->
            <div class="cart-item-price-block">
              <ng-container *ngIf="item.discount > 0">
                <span class="old-price">\${{ item.price | number:'1.2-2' }}</span>
                <span class="discount-badge">-{{ item.discount }}%</span>
                <span class="discounted-price">\${{ (item.price | discountPrice:item.discount) | number:'1.2-2' }}</span>
              </ng-container>
              <ng-container *ngIf="!item.discount || item.discount === 0">
                <span class="regular-price">\${{ item.price | number:'1.2-2' }}</span>
              </ng-container>
            </div>

            <!-- Quantity Controls -->
            <div class="quantity-controls">
              <button class="qty-btn" (click)="decreaseQuantity(item)" [disabled]="item.quantity <= 1">-</button>
              <span class="qty-value">{{ item.quantity }}</span>
              <button class="qty-btn" (click)="increaseQuantity(item)">+</button>
            </div>

            <!-- Item Subtotal -->
            <div class="item-subtotal">
              \${{ getItemSubtotal(item) | number:'1.2-2' }}
            </div>

            <!-- Remove Button -->
            <button class="remove-btn" (click)="removeItem(item)">Remove</button>
          </div>
        </div>

        <!-- Order Summary Sidebar -->
        <div class="order-summary-sidebar" *ngIf="summary">
          <div class="summary-card">
            <h2 class="summary-title">Order Summary</h2>

            <div class="summary-row">
              <span>Subtotal</span>
              <span>\${{ summary.subtotal | number:'1.2-2' }}</span>
            </div>

            <div class="summary-row">
              <span>Taxes (20%)</span>
              <span>\${{ summary.taxes | number:'1.2-2' }}</span>
            </div>

            <div class="summary-divider"></div>

            <div class="summary-row total-row">
              <span>Total</span>
              <span class="total-price">\${{ summary.total | number:'1.2-2' }}</span>
            </div>

            <button class="btn-checkout" (click)="openCheckoutModal()">Checkout</button>
          </div>
        </div>
      </div>

      <!-- Empty Cart State -->
      <div class="empty-cart-state" *ngIf="!loading && items.length === 0">
        <div class="empty-icon">🛒</div>
        <h3>Your Cart is Empty</h3>
        <p>Looks like you haven't added any games to your cart yet.</p>
        <a routerLink="/games" class="btn-browse">Browse Games</a>
      </div>

      <!-- Loading State -->
      <div class="loading-state" *ngIf="loading">
        <p>Loading your cart...</p>
      </div>

      <!-- Checkout Modal -->
      <div class="modal-overlay" *ngIf="showCheckoutModal">
        <div class="modal-content">
          <div class="modal-header">
            <h3>Checkout</h3>
            <button class="modal-close" (click)="showCheckoutModal = false">&times;</button>
          </div>

          <div class="modal-body">
            <div class="payment-methods-section">
              <h4>Select Payment Method</h4>
              <div class="payment-options">
                <label
                  *ngFor="let method of paymentMethods"
                  class="payment-option"
                  [class.selected]="selectedMethod === method.title"
                >
                  <input
                    type="radio"
                    name="paymentMethod"
                    [value]="method.title"
                    [(ngModel)]="selectedMethod"
                  />
                  <div class="payment-info">
                    <span class="payment-title">{{ method.title }}</span>
                    <span class="payment-desc">{{ method.description }}</span>
                  </div>
                </label>
              </div>
            </div>

            <!-- Total Amount Confirmation -->
            <div class="checkout-total-box" *ngIf="summary">
              <span>Total Amount to Pay:</span>
              <strong class="total-amount">\${{ summary.total | number:'1.2-2' }}</strong>
            </div>

            <div class="checkout-actions">
              <button
                class="btn-pay"
                (click)="processPayment()"
                [disabled]="!selectedMethod || paying"
              >
                {{ paying ? 'Processing...' : 'Confirm & Pay' }}
              </button>
              <button class="btn-cancel" (click)="showCheckoutModal = false">Cancel</button>
            </div>
          </div>
        </div>
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

    .cart-header {
      display: flex;
      align-items: center;
      gap: 16px;
      margin-bottom: 32px;
      border-bottom: 2px solid #eee;
      padding-bottom: 16px;
    }

    .cart-title {
      font-size: 32px;
      font-weight: 700;
      color: #222;
      margin: 0;
    }

    .cart-count-badge {
      background: #f57c00;
      color: #fff;
      font-size: 14px;
      font-weight: 500;
      padding: 4px 12px;
      border-radius: 12px;
    }

    .cart-content {
      display: grid;
      grid-template-columns: 1fr 340px;
      gap: 32px;
    }

    /* Cart Items Section */
    .cart-items-section {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .cart-item-card {
      display: flex;
      align-items: center;
      gap: 20px;
      background: #fff;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      padding: 16px 24px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.04);
    }

    .cart-item-image {
      width: 80px;
      height: 80px;
      object-fit: cover;
      border-radius: 6px;
      background: #f5f5f5;
    }

    .cart-item-info {
      flex: 1;
    }

    .cart-item-name {
      font-size: 18px;
      font-weight: 600;
      color: #222;
      margin: 0 0 6px;
    }

    .cart-item-meta {
      display: flex;
      gap: 8px;
    }

    .meta-tag {
      font-size: 12px;
      color: #666;
      background: #f0f0f0;
      padding: 2px 8px;
      border-radius: 4px;
    }

    /* Price Block (US6) */
    .cart-item-price-block {
      display: flex;
      flex-direction: column;
      align-items: flex-end;
      min-width: 100px;
    }

    .regular-price {
      font-size: 18px;
      font-weight: 700;
      color: #222;
    }

    .old-price {
      text-decoration: line-through;
      color: #999;
      font-size: 14px;
    }

    .discount-badge {
      background: #d32f2f;
      color: #fff;
      font-size: 11px;
      font-weight: 700;
      padding: 2px 6px;
      border-radius: 4px;
      margin: 2px 0;
    }

    .discounted-price {
      font-size: 18px;
      font-weight: 700;
      color: #e65100;
    }

    /* Quantity Controls */
    .quantity-controls {
      display: flex;
      align-items: center;
      border: 1px solid #ccc;
      border-radius: 4px;
      overflow: hidden;
    }

    .qty-btn {
      background: #f5f5f5;
      border: none;
      width: 32px;
      height: 32px;
      font-size: 16px;
      font-weight: bold;
      color: #333;
      cursor: pointer;
      transition: background 0.2s;
    }

    .qty-btn:hover:not(:disabled) {
      background: #e0e0e0;
    }

    .qty-btn:disabled {
      color: #ccc;
      cursor: not-allowed;
    }

    .qty-value {
      width: 36px;
      text-align: center;
      font-size: 14px;
      font-weight: 600;
    }

    .item-subtotal {
      font-size: 18px;
      font-weight: 700;
      color: #222;
      min-width: 90px;
      text-align: right;
    }

    .remove-btn {
      background: none;
      border: none;
      color: #f57c00;
      font-size: 14px;
      font-weight: 500;
      cursor: pointer;
      padding: 4px 8px;
    }

    .remove-btn:hover {
      color: #d32f2f;
      text-decoration: underline;
    }

    /* Order Summary Sidebar */
    .summary-card {
      background: #fff;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      padding: 24px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.06);
      position: sticky;
      top: 24px;
    }

    .summary-title {
      font-size: 20px;
      font-weight: 700;
      color: #222;
      margin: 0 0 20px;
      border-bottom: 1px solid #eee;
      padding-bottom: 12px;
    }

    .summary-row {
      display: flex;
      justify-content: space-between;
      font-size: 15px;
      color: #555;
      margin-bottom: 12px;
    }

    .summary-divider {
      height: 1px;
      background: #eee;
      margin: 16px 0;
    }

    .total-row {
      font-size: 18px;
      font-weight: 700;
      color: #222;
    }

    .total-price {
      color: #f57c00;
      font-size: 24px;
    }

    .btn-checkout {
      width: 100%;
      background: #f57c00;
      color: #fff;
      border: none;
      padding: 14px;
      border-radius: 6px;
      font-size: 16px;
      font-weight: 700;
      cursor: pointer;
      margin-top: 20px;
      transition: background 0.2s;
    }

    .btn-checkout:hover {
      background: #e65100;
    }

    /* Empty Cart */
    .empty-cart-state {
      text-align: center;
      padding: 64px 24px;
    }

    .empty-icon {
      font-size: 64px;
      margin-bottom: 16px;
    }

    .empty-cart-state h3 {
      font-size: 24px;
      color: #333;
      margin-bottom: 8px;
    }

    .empty-cart-state p {
      color: #666;
      margin-bottom: 24px;
    }

    .btn-browse {
      display: inline-block;
      background: #f57c00;
      color: #fff;
      text-decoration: none;
      padding: 12px 28px;
      border-radius: 6px;
      font-size: 16px;
      font-weight: 500;
    }

    /* Modal Styling */
    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100vw;
      height: 100vh;
      background: rgba(0,0,0,0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
    }

    .modal-content {
      background: #fff;
      border-radius: 8px;
      width: 480px;
      max-width: 90vw;
      padding: 24px;
      box-shadow: 0 4px 20px rgba(0,0,0,0.2);
    }

    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
      border-bottom: 1px solid #eee;
      padding-bottom: 12px;
    }

    .modal-header h3 {
      font-size: 20px;
      margin: 0;
    }

    .modal-close {
      background: none;
      border: none;
      font-size: 24px;
      cursor: pointer;
      color: #888;
    }

    .payment-options {
      display: flex;
      flex-direction: column;
      gap: 12px;
      margin: 16px 0 24px;
    }

    .payment-option {
      display: flex;
      align-items: flex-start;
      gap: 12px;
      border: 1px solid #ddd;
      border-radius: 6px;
      padding: 12px 16px;
      cursor: pointer;
      transition: border-color 0.2s;
    }

    .payment-option.selected {
      border-color: #f57c00;
      background: #fff3e0;
    }

    .payment-title {
      display: block;
      font-weight: 600;
      font-size: 15px;
    }

    .payment-desc {
      display: block;
      font-size: 13px;
      color: #666;
    }

    .checkout-total-box {
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: #f9f9f9;
      padding: 12px 16px;
      border-radius: 6px;
      margin-bottom: 24px;

    }

    .total-amount {
      color: #f57c00;
      font-size: 20px;
    }

    .checkout-actions {
      display: flex;
      gap: 12px;
    }

    .btn-pay {
      flex: 1;
      background: #f57c00;
      color: #fff;
      border: none;
      padding: 12px;
      border-radius: 6px;
      font-size: 15px;
      font-weight: 600;
      cursor: pointer;
    }

    .btn-pay:disabled {
      background: #ccc;
    }

    .btn-cancel {
      background: #f5f5f5;
      border: 1px solid #ccc;
      padding: 12px 20px;
      border-radius: 6px;
      font-size: 15px;
      cursor: pointer;
    }

    @media (max-width: 900px) {
      .cart-content {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class CartComponent implements OnInit {
  items: CartItem[] = [];
  summary: CartSummary | null = null;
  loading: boolean = true;

  showCheckoutModal: boolean = false;
  paymentMethods: PaymentMethod[] = [
    { title: 'Bank', description: 'Pay via direct bank transfer' },
    { title: 'IBox', description: 'Pay via IBox terminal' },
    { title: 'Visa', description: 'Pay with Credit or Debit card' }
  ];
  selectedMethod: string = 'Bank';
  paying: boolean = false;

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private gameImageService: GameImageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.loading = true;
    this.orderService.getCart().subscribe({
      next: (cart) => {
        this.loading = false;
        const rawDetails = cart ? (cart.details || cart.orderGames || []) : [];
        if (rawDetails.length > 0) {
          this.items = rawDetails.map((detail: any) => ({
            id: detail.id || detail.productId || detail.gameKey,
            productId: detail.productId || detail.gameKey,
            gameKey: detail.gameKey || detail.productId,
            gameName: detail.gameName || detail.productName || detail.name || 'Game',
            price: detail.price || 0,
            quantity: detail.quantity || 1,
            discount: detail.discount !== undefined ? detail.discount : (detail.discountPercentage || 0),
            platform: detail.platform || 'PC',
            fileSize: detail.fileSize || '15 GB'
          }));
        } else {
          this.items = this.cartService.getLocalCartItems();
        }
        this.recalculate();
      },
      error: () => {
        this.loading = false;
        this.items = this.cartService.getLocalCartItems();
        this.recalculate();
      }
    });
  }

  getItemImage(item: CartItem): string {
    return this.gameImageService.getGameImage(item.gameKey, item.gameName);
  }

  getItemSubtotal(item: CartItem): number {
    const unitPrice = item.discount > 0
      ? item.price * (1 - item.discount / 100)
      : item.price;
    return unitPrice * item.quantity;
  }

  increaseQuantity(item: CartItem): void {
    item.quantity++;
    this.cartService.updateLocalQuantity(item.id || item.gameKey || '', item.quantity);
    this.updateQuantityOnBackend(item);
  }

  decreaseQuantity(item: CartItem): void {
    if (item.quantity > 1) {
      item.quantity--;
      this.cartService.updateLocalQuantity(item.id || item.gameKey || '', item.quantity);
      this.updateQuantityOnBackend(item);
    }
  }

  private updateQuantityOnBackend(item: CartItem): void {
    this.recalculate();
    if (item.id) {
      this.orderService.updateOrderDetailQuantity(item.id, item.quantity).subscribe({
        next: () => {},
        error: () => {}
      });
    }
  }

  removeItem(item: CartItem): void {
    this.items = this.items.filter(i => i !== item);
    this.cartService.removeFromCart(item.gameKey || item.id || '').subscribe();
    this.recalculate();
  }

  recalculate(): void {
    this.summary = this.cartService.calculateSummary(this.items);
  }

  openCheckoutModal(): void {
    this.showCheckoutModal = true;
    this.orderService.getPaymentMethods().subscribe({
      next: (res) => {
        if (res && res.paymentMethods && res.paymentMethods.length > 0) {
          this.paymentMethods = res.paymentMethods;
          this.selectedMethod = res.paymentMethods[0].title;
        }
      },
      error: () => {}
    });
  }

  processPayment(): void {
    if (!this.selectedMethod) return;
    this.paying = true;

    this.orderService.pay({ method: this.selectedMethod }).subscribe({
      next: () => {
        this.paying = false;
        this.showCheckoutModal = false;
        this.items = [];
        this.cartService.clearCart();
        this.recalculate();
        this.router.navigate(['/orders']);
      },
      error: () => {
        this.paying = false;
        this.showCheckoutModal = false;
        this.items = [];
        this.cartService.clearCart();
        this.recalculate();
        this.router.navigate(['/orders']);
      }
    });
  }
}
