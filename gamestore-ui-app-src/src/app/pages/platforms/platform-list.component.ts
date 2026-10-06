import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PlatformService } from '../../services/platform.service';
import { Platform } from '../../models/platform.model';

@Component({
  selector: 'app-platform-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="page-container">
      <div class="header-section">
        <h1 class="page-title">Platforms</h1>
        <button class="btn-add" (click)="openAddModal()">+ Add New Platform</button>
      </div>

      <!-- Platforms Table -->
      <div class="table-wrapper" *ngIf="!loading && platforms.length > 0">
        <table class="data-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Platform Type</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let platform of platforms">
              <td class="id-cell">{{ platform.id }}</td>
              <td class="name-cell">{{ platform.type }}</td>
              <td class="actions-cell">
                <button class="btn-edit" (click)="openEditModal(platform)">Edit</button>
                <button class="btn-delete" (click)="deletePlatform(platform)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="empty-state" *ngIf="!loading && platforms.length === 0">
        <p>No platforms found.</p>
      </div>

      <div class="loading-state" *ngIf="loading">
        <p>Loading platforms...</p>
      </div>

      <!-- Add/Edit Modal -->
      <div class="modal-overlay" *ngIf="showModal">
        <div class="modal-content">
          <div class="modal-header">
            <h3>{{ editingPlatform ? 'Edit Platform' : 'Add New Platform' }}</h3>
            <button class="modal-close" (click)="showModal = false">&times;</button>
          </div>

          <div class="modal-body">
            <div class="form-group">
              <label>Platform Type</label>
              <input type="text" [(ngModel)]="formType" placeholder="e.g. PS5, Xbox Series X, PC" class="form-input" />
            </div>

            <div class="modal-actions">
              <button class="btn-save" (click)="savePlatform()" [disabled]="!formType || saving">
                {{ saving ? 'Saving...' : 'Save' }}
              </button>
              <button class="btn-cancel" (click)="showModal = false">Cancel</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-container {
      max-width: 900px;
      margin: 0 auto;
      padding: 32px 24px 64px;
      font-family: 'Roboto', sans-serif;
    }

    .header-section {
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

    .btn-add {
      background: #f57c00;
      color: #fff;
      border: none;
      padding: 10px 20px;
      border-radius: 6px;
      font-size: 14px;
      font-weight: 600;
      cursor: pointer;
    }

    .table-wrapper {
      background: #fff;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      overflow: hidden;
      box-shadow: 0 2px 4px rgba(0,0,0,0.04);
    }

    .data-table {
      width: 100%;
      border-collapse: collapse;
      text-align: left;
    }

    .data-table th {
      background: #f8f9fa;
      padding: 14px 20px;
      font-size: 14px;
      font-weight: 600;
      color: #555;
      border-bottom: 1px solid #e0e0e0;
    }

    .data-table td {
      padding: 14px 20px;
      font-size: 14px;
      border-bottom: 1px solid #f0f0f0;
      color: #333;
    }

    .name-cell {
      font-weight: 600;
    }

    .actions-cell {
      display: flex;
      gap: 8px;
    }

    .btn-edit {
      background: none;
      border: 1px solid #1976d2;
      color: #1976d2;
      padding: 4px 12px;
      border-radius: 4px;
      cursor: pointer;
    }

    .btn-delete {
      background: none;
      border: 1px solid #d32f2f;
      color: #d32f2f;
      padding: 4px 12px;
      border-radius: 4px;
      cursor: pointer;
    }

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
      width: 400px;
      padding: 24px;
    }

    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 6px;
      margin-bottom: 16px;
    }

    .form-input {
      padding: 10px;
      border: 1px solid #ccc;
      border-radius: 4px;
    }

    .modal-actions {
      display: flex;
      gap: 12px;
      margin-top: 24px;
    }

    .btn-save {
      flex: 1;
      background: #f57c00;
      color: #fff;
      border: none;
      padding: 10px;
      border-radius: 4px;
      cursor: pointer;
    }

    .btn-cancel {
      background: #e0e0e0;
      border: none;
      padding: 10px 16px;
      border-radius: 4px;
      cursor: pointer;
    }
  `]
})
export class PlatformListComponent implements OnInit {
  platforms: Platform[] = [];
  loading: boolean = true;
  showModal: boolean = false;
  editingPlatform: Platform | null = null;
  formType: string = '';
  saving: boolean = false;

  constructor(private platformService: PlatformService) {}

  ngOnInit(): void {
    this.loadPlatforms();
  }

  loadPlatforms(): void {
    this.loading = true;
    this.platformService.getPlatforms().subscribe({
      next: (data) => {
        this.platforms = data || [];
        this.loading = false;
      },
      error: () => {
        this.platforms = [];
        this.loading = false;
      }
    });
  }

  openAddModal(): void {
    this.editingPlatform = null;
    this.formType = '';
    this.showModal = true;
  }

  openEditModal(platform: Platform): void {
    this.editingPlatform = platform;
    this.formType = platform.type;
    this.showModal = true;
  }

  savePlatform(): void {
    if (!this.formType) return;
    this.saving = true;

    if (this.editingPlatform) {
      this.platformService.updatePlatform({
        platform: {
          id: this.editingPlatform.id,
          type: this.formType
        }
      }).subscribe({
        next: () => {
          this.saving = false;
          this.showModal = false;
          this.loadPlatforms();
        },
        error: () => {
          this.saving = false;
          this.showModal = false;
          this.loadPlatforms();
        }
      });
    } else {
      this.platformService.createPlatform({
        platform: {
          type: this.formType
        }
      }).subscribe({
        next: () => {
          this.saving = false;
          this.showModal = false;
          this.loadPlatforms();
        },
        error: () => {
          this.saving = false;
          this.showModal = false;
          this.loadPlatforms();
        }
      });
    }
  }

  deletePlatform(platform: Platform): void {
    if (confirm(`Are you sure you want to delete platform "${platform.type}"?`)) {
      this.platformService.deletePlatform(platform.id).subscribe({
        next: () => this.loadPlatforms(),
        error: () => this.loadPlatforms()
      });
    }
  }
}
