import { LanguageService } from '../../services/language.service';
import { TranslatePipe } from '../../shared/pipes/translate.pipe';
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PublisherService } from '../../services/publisher.service';
import { Publisher } from '../../models/publisher.model';

@Component({
  selector: 'app-publisher-list',
  standalone: true,
  imports: [TranslatePipe, CommonModule, RouterModule, FormsModule],
  template: `
    <div class="page-container">
      <div class="header-section">
        <h1 class="page-title">{{ 'Publishers' | translate }}</h1>
        <button class="btn-add" (click)="openAddModal()">{{ '+ Add Publisher' | translate }}</button>
      </div>

      <!-- Publishers Table -->
      <div class="table-wrapper" *ngIf="!loading && publishers.length > 0">
        <table class="data-table">
          <thead>
            <tr>
              <th>{{ 'Company Name' | translate }}</th>
              <th>{{ 'Home Page' | translate }}</th>
              <th>{{ 'Description' | translate }}</th>
              <th>{{ 'Actions' | translate }}</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let pub of publishers">
              <td class="name-cell">
                <a [routerLink]="['/publisher', pub.companyName]">{{ pub.companyName }}</a>
              </td>
              <td>
                <a *ngIf="pub.homePage" [href]="pub.homePage" target="_blank" class="external-link">
                  {{ pub.homePage }}
                </a>
                <span *ngIf="!pub.homePage">—</span>
              </td>
              <td class="desc-cell">{{ pub.description || '—' }}</td>
              <td class="actions-cell">
                <button class="btn-edit" (click)="openEditModal(pub)">{{ 'Edit' | translate }}</button>
                <button class="btn-delete" (click)="deletePublisher(pub)">{{ 'Delete' | translate }}</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="empty-state" *ngIf="!loading && publishers.length === 0">
        <p>{{ 'No publishers found.' | translate }}</p>
      </div>

      <div class="loading-state" *ngIf="loading">
        <p>{{ 'Loading publishers...' | translate }}</p>
      </div>

      <!-- Add/Edit Modal -->
      <div class="modal-overlay" *ngIf="showModal">
        <div class="modal-content">
          <div class="modal-header">
            <h3>{{ (editingPublisher ? 'Edit Publisher' : 'Add New Publisher') | translate }}</h3>
            <button class="modal-close" (click)="showModal = false">&times;</button>
          </div>

          <div class="modal-body">
            <div class="form-group">
              <label>{{ 'Company Name' | translate }}</label>
              <input type="text" [(ngModel)]="formCompanyName" [placeholder]="'e.g. Electronic Arts' | translate" class="form-input" />
            </div>

            <div class="form-group">
              <label>{{ 'Home Page URL' | translate }}</label>
              <input type="text" [(ngModel)]="formHomePage" placeholder="https://..." class="form-input" />
            </div>

            <div class="form-group">
              <label>{{ 'Description' | translate }}</label>
              <textarea [(ngModel)]="formDescription" rows="3" [placeholder]="'Company description...' | translate" class="form-input"></textarea>
            </div>

            <div class="modal-actions">
              <button class="btn-save" (click)="savePublisher()" [disabled]="!formCompanyName || saving">
                {{ (saving ? 'Saving...' : 'Save') | translate }}
              </button>
              <button class="btn-cancel" (click)="showModal = false">{{ 'Cancel' | translate }}</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-container {
      max-width: 1100px;
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

    .name-cell a {
      color: #f57c00;
      font-weight: 600;
      text-decoration: none;
    }

    .name-cell a:hover { text-decoration: underline; }

    .external-link {
      color: #1976d2;
      text-decoration: none;
    }

    .desc-cell {
      max-width: 300px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
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
      width: 480px;
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
export class PublisherListComponent implements OnInit {
  private readonly language = inject(LanguageService);
  publishers: Publisher[] = [];
  loading: boolean = true;
  showModal: boolean = false;
  editingPublisher: Publisher | null = null;
  formCompanyName: string = '';
  formHomePage: string = '';
  formDescription: string = '';
  saving: boolean = false;

  constructor(private publisherService: PublisherService) {}

  ngOnInit(): void {
    this.loadPublishers();
  }

  loadPublishers(): void {
    this.loading = true;
    this.publisherService.getPublishers().subscribe({
      next: (data) => {
        this.publishers = data || [];
        this.loading = false;
      },
      error: () => {
        this.publishers = [];
        this.loading = false;
      }
    });
  }

  openAddModal(): void {
    this.editingPublisher = null;
    this.formCompanyName = '';
    this.formHomePage = '';
    this.formDescription = '';
    this.showModal = true;
  }

  openEditModal(pub: Publisher): void {
    this.editingPublisher = pub;
    this.formCompanyName = pub.companyName;
    this.formHomePage = pub.homePage || '';
    this.formDescription = pub.description || '';
    this.showModal = true;
  }

  savePublisher(): void {
    if (!this.formCompanyName) return;
    this.saving = true;

    if (this.editingPublisher) {
      this.publisherService.updatePublisher({
        publisher: {
          id: this.editingPublisher.id,
          companyName: this.formCompanyName,
          homePage: this.formHomePage,
          description: this.formDescription
        }
      }).subscribe({
        next: () => {
          this.saving = false;
          this.showModal = false;
          this.loadPublishers();
        },
        error: () => {
          this.saving = false;
          alert(this.language.translate('Could not save changes. Please try again.'));
        }
      });
    } else {
      this.publisherService.createPublisher({
        publisher: {
          companyName: this.formCompanyName,
          homePage: this.formHomePage,
          description: this.formDescription
        }
      }).subscribe({
        next: () => {
          this.saving = false;
          this.showModal = false;
          this.loadPublishers();
        },
        error: () => {
          this.saving = false;
          alert(this.language.translate('Could not save changes. Please try again.'));
        }
      });
    }
  }

  deletePublisher(pub: Publisher): void {
    if (confirm(this.language.translate('Delete this item?'))) {
      this.publisherService.deletePublisher(pub.id).subscribe({
        next: () => this.loadPublishers(),
        error: () => alert(this.language.translate('Could not delete this item.'))
      });
    }
  }
}
