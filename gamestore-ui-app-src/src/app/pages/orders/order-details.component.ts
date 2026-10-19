import { forkJoin } from 'rxjs';
import { TranslatePipe } from '../../shared/pipes/translate.pipe';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { Order, OrderItem } from '../../models/order.model';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-order-details',
  standalone: true,
  imports: [TranslatePipe, CommonModule, RouterModule],
  template: `
    <div class="page-container" *ngIf="!loading && order">
      <a routerLink="/orders" class="back-link">{{ '< Back to Orders' | translate }}</a>

      <div class="order-header-card">
        <div class="order-info-col">
          <h1 class="order-id-heading">{{ 'Order #' | translate }}{{ order.id }}</h1>
          <span class="order-date" *ngIf="order.date">{{ 'Date:' | translate }} {{ order.date | date:'medium' }}</span>
        </div>
        <div class="order-status-col">
          <span class="status-badge" [ngClass]="getStatusClass(order.status)">
            {{ (order.status || 'Paid') | translate }}
          </span>
        </div>
      </div>

      <div class="details-layout">
        <!-- Items Table -->
        <div class="items-card">
          <h2 class="card-title">{{ 'Ordered Items' | translate }}</h2>
          <table class="items-table">
            <thead>
              <tr>
                <th>{{ 'Item' | translate }}</th>
                <th>{{ 'Price' | translate }}</th>
                <th>{{ 'Discount' | translate }}</th>
                <th>{{ 'Quantity' | translate }}</th>
                <th class="text-right">{{ 'Total' | translate }}</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let item of items">
                <td class="item-name">{{ item.productName || item.productId }}</td>
                <td>\${{ item.price | number:'1.2-2' }}</td>
                <td>
                  <span class="discount-badge" *ngIf="item.discount > 0">-{{ item.discount }}%</span>
                  <span *ngIf="!item.discount || item.discount === 0">—</span>
                </td>
                <td>{{ item.quantity }}</td>
                <td class="text-right item-total">\${{ getItemTotal(item) | number:'1.2-2' }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Order Summary Sidebar -->
        <div class="summary-card">
          <h2 class="card-title">{{ 'Order Summary' | translate }}</h2>

          <div class="summary-row">
            <span>{{ 'Subtotal' | translate }}</span>
            <span>\${{ subtotal | number:'1.2-2' }}</span>
          </div>

          <div class="summary-row">
            <span>{{ 'Tax (20%)' | translate }}</span>
            <span>\${{ taxes | number:'1.2-2' }}</span>
          </div>

          <div class="summary-divider"></div>

          <div class="summary-row total-row">
            <span>{{ 'Grand Total' | translate }}</span>
            <span class="total-amount">\${{ total | number:'1.2-2' }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div class="loading-state" *ngIf="loading">
      <p>{{ 'Loading order details...' | translate }}</p>
    </div>

    <!-- Error State -->
    <div class="error-state" *ngIf="!loading && !order">
      <p>{{ 'Order not found.' | translate }}</p>
      <a routerLink="/orders" class="btn-browse">{{ 'Back to Orders' | translate }}</a>
    </div>
  `,
  styles: [`
    .page-container {
      max-width: 1200px;
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

    .back-link:hover {
      text-decoration: underline;
    }

    .order-header-card {
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: #fff;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      padding: 24px;
      margin-bottom: 32px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.04);
    }

    .order-id-heading {
      font-size: 24px;
      font-weight: 700;
      color: #222;
      margin: 0 0 6px;
    }

    .order-date {
      font-size: 14px;
      color: #666;
    }

    .status-badge {
      display: inline-block;
      padding: 6px 16px;
      border-radius: 16px;
      font-size: 14px;
      font-weight: 600;
    }

    .status-badge.paid {
      background: #e8f5e9;
      color: #2e7d32;
    }

    .status-badge.shipped {
      background: #e3f2fd;
      color: #1565c0;
    }

    .details-layout {
      display: grid;
      grid-template-columns: 1fr 340px;
      gap: 32px;
    }

    .items-card, .summary-card {
      background: #fff;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      padding: 24px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.04);
    }

    .card-title {
      font-size: 18px;
      font-weight: 700;
      color: #222;
      margin: 0 0 20px;
      border-bottom: 1px solid #eee;
      padding-bottom: 12px;
    }

    .items-table {
      width: 100%;
      border-collapse: collapse;
      text-align: left;
    }

    .items-table th {
      padding: 12px 16px;
      font-size: 13px;
      color: #666;
      border-bottom: 1px solid #e0e0e0;
    }

    .items-table td {
      padding: 14px 16px;
      font-size: 14px;
      border-bottom: 1px solid #f0f0f0;
      color: #333;
    }

    .item-name {
      font-weight: 600;
    }

    .discount-badge {
      background: #d32f2f;
      color: #fff;
      font-size: 11px;
      font-weight: 700;
      padding: 2px 6px;
      border-radius: 4px;
    }

    .text-right {
      text-align: right;
    }

    .item-total {
      font-weight: 700;
      color: #222;
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

    .total-amount {
      color: #f57c00;
      font-size: 22px;
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

    @media (max-width: 900px) {
      .details-layout {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class OrderDetailsComponent implements OnInit {
  order: Order | null = null;
  items: OrderItem[] = [];
  loading: boolean = true;

  subtotal: number = 0;
  taxes: number = 0;
  total: number = 0;

  constructor(
    private route: ActivatedRoute,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadOrderDetails(id);
    } else {
      this.loading = false;
    }
  }

  loadOrderDetails(id: string): void {
    this.loading = true;
    forkJoin({ order: this.orderService.getOrderById(id), details: this.orderService.getOrderDetails(id) }).subscribe({
      next: ({ order, details }) => {
        this.order = order;
        this.items = details;
        this.calculateTotals();
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  getItemTotal(item: OrderItem): number {
    const discount = item.discount || 0;
    const price = discount > 0 ? item.price * (1 - discount / 100) : item.price;
    return price * (item.quantity || 1);
  }

  calculateTotals(): void {
    this.subtotal = this.items.reduce((sum, item) => sum + this.getItemTotal(item), 0);
    this.taxes = this.subtotal * environment.taxRate;
    this.total = this.subtotal + this.taxes;
  }

  getStatusClass(status?: string): string {
    if (!status) return 'paid';
    const lower = status.toLowerCase();
    if (lower.includes('ship')) return 'shipped';
    return 'paid';
  }
}
