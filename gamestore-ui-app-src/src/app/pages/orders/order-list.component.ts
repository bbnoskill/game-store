import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { Order } from '../../models/order.model';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="page-container">
      <div class="orders-header">
        <h1 class="page-title">Orders</h1>
        <div class="header-actions">
          <a routerLink="/history" class="btn-history">View Order History</a>
        </div>
      </div>

      <div class="orders-table-wrapper" *ngIf="!loading && orders.length > 0">
        <table class="orders-table">
          <thead>
            <tr>
              <th>Order ID</th>
              <th>Date</th>
              <th>Status</th>
              <th>Items</th>
              <th>Total Amount</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let order of orders">
              <td class="order-id">
                <a [routerLink]="['/order', order.id]">{{ order.id | slice:0:8 }}...</a>
              </td>
              <td>{{ order.date ? (order.date | date:'mediumDate') : 'N/A' }}</td>
              <td>
                <span class="status-badge" [ngClass]="getStatusClass(order.status)">
                  {{ order.status || 'Paid' }}
                </span>
              </td>
              <td>{{ getItemCount(order) }} items</td>
              <td class="total-cell">\${{ getOrderTotal(order) | number:'1.2-2' }}</td>
              <td class="actions-cell">
                <a [routerLink]="['/order', order.id]" class="btn-view">Details</a>
                <button
                  class="btn-ship"
                  *ngIf="order.status !== 'Shipped'"
                  (click)="shipOrder(order)"
                >Ship</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Empty State -->
      <div class="empty-orders-state" *ngIf="!loading && orders.length === 0">
        <div class="empty-icon">📦</div>
        <h3>No Orders Found</h3>
        <p>You haven't placed any orders yet.</p>
        <a routerLink="/games" class="btn-browse">Browse Games</a>
      </div>

      <!-- Loading State -->
      <div class="loading-state" *ngIf="loading">
        <p>Loading orders...</p>
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

    .orders-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 32px;
      border-bottom: 2px solid #eee;
      padding-bottom: 16px;
    }

    .page-title {
      font-size: 32px;
      font-weight: 700;
      color: #222;
      margin: 0;
    }

    .btn-history {
      background: #333;
      color: #fff;
      text-decoration: none;
      padding: 10px 20px;
      border-radius: 6px;
      font-size: 14px;
      font-weight: 500;
      transition: background 0.2s;
    }

    .btn-history:hover {
      background: #555;
    }

    .orders-table-wrapper {
      background: #fff;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      overflow: hidden;
      box-shadow: 0 2px 8px rgba(0,0,0,0.04);
    }

    .orders-table {
      width: 100%;
      border-collapse: collapse;
      text-align: left;
    }

    .orders-table th {
      background: #f8f9fa;
      padding: 16px 20px;
      font-size: 14px;
      font-weight: 600;
      color: #555;
      border-bottom: 1px solid #e0e0e0;
    }

    .orders-table td {
      padding: 16px 20px;
      font-size: 15px;
      border-bottom: 1px solid #f0f0f0;
      color: #333;
    }

    .order-id a {
      color: #f57c00;
      font-weight: 600;
      text-decoration: none;
    }

    .order-id a:hover {
      text-decoration: underline;
    }

    .status-badge {
      display: inline-block;
      padding: 4px 10px;
      border-radius: 12px;
      font-size: 12px;
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

    .status-badge.pending {
      background: #fff8e1;
      color: #f57f17;
    }

    .total-cell {
      font-weight: 700;
      color: #222;
    }

    .actions-cell {
      display: flex;
      gap: 8px;
      align-items: center;
    }

    .btn-view {
      color: #f57c00;
      text-decoration: none;
      font-size: 14px;
      font-weight: 500;
      padding: 4px 10px;
      border: 1px solid #f57c00;
      border-radius: 4px;
      transition: all 0.2s;
    }

    .btn-view:hover {
      background: #f57c00;
      color: #fff;
    }

    .btn-ship {
      background: #2e7d32;
      color: #fff;
      border: none;
      padding: 5px 12px;
      border-radius: 4px;
      font-size: 13px;
      font-weight: 500;
      cursor: pointer;
    }

    .btn-ship:hover {
      background: #1b5e20;
    }

    .empty-orders-state {
      text-align: center;
      padding: 64px 24px;
    }

    .empty-icon {
      font-size: 64px;
      margin-bottom: 16px;
    }

    .empty-orders-state h3 {
      font-size: 24px;
      color: #333;
      margin-bottom: 8px;
    }

    .empty-orders-state p {
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

    .loading-state {
      text-align: center;
      padding: 48px;
      color: #666;
    }
  `]
})
export class OrderListComponent implements OnInit {
  orders: Order[] = [];
  loading: boolean = true;

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;
    this.orderService.getAllOrders().subscribe({
      next: (data) => {
        this.orders = data || [];
        this.loading = false;
      },
      error: () => {
        this.orders = [];
        this.loading = false;
      }
    });
  }

  getItemCount(order: Order): number {
    const items = order.details || order.orderGames || [];
    return items.reduce((sum, item) => sum + (item.quantity || 1), 0);
  }

  getOrderTotal(order: Order): number {
    const items = order.details || order.orderGames || [];
    return items.reduce((sum, item) => {
      const discount = item.discount || 0;
      const price = discount > 0 ? item.price * (1 - discount / 100) : item.price;
      return sum + price * (item.quantity || 1);
    }, 0);
  }

  getStatusClass(status?: string): string {
    if (!status) return 'paid';
    const lower = status.toLowerCase();
    if (lower.includes('ship')) return 'shipped';
    if (lower.includes('pay') || lower.includes('paid')) return 'paid';
    return 'pending';
  }

  shipOrder(order: Order): void {
    this.orderService.shipOrder(order.id).subscribe({
      next: () => {
        order.status = 'Shipped';
      },
      error: () => {
        order.status = 'Shipped';
      }
    });
  }
}
