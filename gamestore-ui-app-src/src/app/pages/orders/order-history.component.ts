import { TranslatePipe } from '../../shared/pipes/translate.pipe';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../../services/order.service';
import { OrderHistoryItem } from '../../models/order.model';

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [TranslatePipe, CommonModule, RouterModule, FormsModule],
  template: `
    <div class="page-container">
      <div class="history-header">
        <h1 class="page-title">{{ 'Order History' | translate }}</h1>
        <a routerLink="/orders" class="btn-back">{{ '< Active Orders' | translate }}</a>
      </div>

      <!-- Date Range Filter Form -->
      <div class="filter-card">
        <h3 class="filter-title">{{ 'Filter by Date Range' | translate }}</h3>
        <div class="filter-form">
          <div class="form-group">
            <label>{{ 'Start Date' | translate }}</label>
            <input type="date" [(ngModel)]="startDate" class="form-input" />
          </div>
          <div class="form-group">
            <label>{{ 'End Date' | translate }}</label>
            <input type="date" [(ngModel)]="endDate" class="form-input" />
          </div>
          <button class="btn-filter" (click)="applyFilter()">{{ 'Apply Filter' | translate }}</button>
          <button class="btn-reset" (click)="resetFilter()">{{ 'Reset' | translate }}</button>
        </div>
      </div>

      <!-- History Table -->
      <div class="history-table-wrapper" *ngIf="!loading && history.length > 0">
        <table class="history-table">
          <thead>
            <tr>
              <th>{{ 'Order ID' | translate }}</th>
              <th>{{ 'Date' | translate }}</th>
              <th>{{ 'Total Price' | translate }}</th>
              <th>{{ 'Actions' | translate }}</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let item of history">
              <td class="order-id">
                <a [routerLink]="['/order', item.id]">{{ item.id }}</a>
              </td>
              <td>{{ item.date | date:'medium' }}</td>
              <td class="total-cell">\${{ getItemTotal(item) | number:'1.2-2' }}</td>
              <td>
                <a [routerLink]="['/order', item.id]" class="btn-details">{{ 'View Details' | translate }}</a>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Empty State -->
      <div class="empty-state" *ngIf="!loading && history.length === 0">
        <p>{{ 'No order history found for the selected date range.' | translate }}</p>
      </div>

      <!-- Loading State -->
      <div class="loading-state" *ngIf="loading">
        <p>{{ 'Loading order history...' | translate }}</p>
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

    .history-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
      border-bottom: 2px solid #eee;
      padding-bottom: 16px;
    }

    .page-title {
      font-size: 32px;
      font-weight: 700;
      color: #222;
      margin: 0;
    }

    .btn-back {
      color: #f57c00;
      text-decoration: none;
      font-size: 14px;
      font-weight: 500;
    }

    .filter-card {
      background: #f9f9f9;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      padding: 20px 24px;
      margin-bottom: 32px;
    }

    .filter-title {
      font-size: 16px;
      font-weight: 600;
      color: #333;
      margin: 0 0 16px;
    }

    .filter-form {
      display: flex;
      align-items: flex-end;
      gap: 16px;
      flex-wrap: wrap;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }

    .form-group label {
      font-size: 13px;
      color: #666;
      font-weight: 500;
    }

    .form-input {
      padding: 8px 12px;
      border: 1px solid #ccc;
      border-radius: 4px;
      font-size: 14px;
    }

    .btn-filter {
      background: #f57c00;
      color: #fff;
      border: none;
      padding: 9px 20px;
      border-radius: 4px;
      font-size: 14px;
      font-weight: 500;
      cursor: pointer;
    }

    .btn-filter:hover {
      background: #e65100;
    }

    .btn-reset {
      background: #e0e0e0;
      color: #333;
      border: none;
      padding: 9px 16px;
      border-radius: 4px;
      font-size: 14px;
      cursor: pointer;
    }

    .history-table-wrapper {
      background: #fff;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      overflow: hidden;
      box-shadow: 0 2px 4px rgba(0,0,0,0.04);
    }

    .history-table {
      width: 100%;
      border-collapse: collapse;
      text-align: left;
    }

    .history-table th {
      background: #f8f9fa;
      padding: 14px 20px;
      font-size: 14px;
      font-weight: 600;
      color: #555;
      border-bottom: 1px solid #e0e0e0;
    }

    .history-table td {
      padding: 14px 20px;
      font-size: 14px;
      border-bottom: 1px solid #f0f0f0;
      color: #333;
    }

    .order-id a {
      color: #f57c00;
      font-weight: 600;
      text-decoration: none;
    }

    .total-cell {
      font-weight: 700;
      color: #222;
    }

    .btn-details {
      color: #f57c00;
      text-decoration: none;
      font-size: 13px;
      font-weight: 500;
    }

    .loading-state, .empty-state {
      text-align: center;
      padding: 48px;
      color: #666;
    }
  `]
})
export class OrderHistoryComponent implements OnInit {
  history: OrderHistoryItem[] = [];
  loading: boolean = true;
  startDate: string = '';
  endDate: string = '';

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadHistory();
  }

  loadHistory(): void {
    this.loading = true;
    this.orderService.getHistory(this.startDate, this.endDate).subscribe({
      next: (data) => {
        this.history = data || [];
        this.loading = false;
      },
      error: () => {
        this.history = [];
        this.loading = false;
      }
    });
  }

  applyFilter(): void {
    this.loadHistory();
  }

  resetFilter(): void {
    this.startDate = '';
    this.endDate = '';
    this.loadHistory();
  }

  getItemTotal(item: OrderHistoryItem): number {
    if (item.total !== undefined) return item.total;
    if (item.items && item.items.length > 0) {
      return item.items.reduce((sum, i) => {
        const discount = i.discount || 0;
        const price = discount > 0 ? i.price * (1 - discount / 100) : i.price;
        return sum + price * (i.quantity || 1);
      }, 0);
    }
    return 0;
  }
}
