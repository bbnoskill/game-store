import { LanguageService } from '../../services/language.service';
import { TranslatePipe } from '../../shared/pipes/translate.pipe';
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GenreService } from '../../services/genre.service';
import { Genre } from '../../models/genre.model';

@Component({
  selector: 'app-genre-list',
  standalone: true,
  imports: [TranslatePipe, CommonModule, RouterModule, FormsModule],
  template: `
    <div class="page-container">
      <div class="header-section">
        <h1 class="page-title">{{ 'Genres' | translate }}</h1>
        <button class="btn-add" (click)="openAddModal()">{{ '+ Add New Genre' | translate }}</button>
      </div>

      <!-- Genres Table -->
      <div class="table-wrapper" *ngIf="!loading && genres.length > 0">
        <table class="data-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>{{ 'Genre Name' | translate }}</th>
              <th>{{ 'Parent Genre' | translate }}</th>
              <th>{{ 'Actions' | translate }}</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let genre of genres">
              <td class="id-cell">{{ genre.id }}</td>
              <td class="name-cell">{{ genre.name }}</td>
              <td>{{ getParentName(genre.parentGenreId) }}</td>
              <td class="actions-cell">
                <button class="btn-edit" (click)="openEditModal(genre)">{{ 'Edit' | translate }}</button>
                <button class="btn-delete" (click)="deleteGenre(genre)">{{ 'Delete' | translate }}</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="empty-state" *ngIf="!loading && genres.length === 0">
        <p>{{ 'No genres found.' | translate }}</p>
      </div>

      <div class="loading-state" *ngIf="loading">
        <p>{{ 'Loading genres...' | translate }}</p>
      </div>

      <!-- Add/Edit Modal -->
      <div class="modal-overlay" *ngIf="showModal">
        <div class="modal-content">
          <div class="modal-header">
            <h3>{{ (editingGenre ? 'Edit Genre' : 'Add New Genre') | translate }}</h3>
            <button class="modal-close" (click)="showModal = false">&times;</button>
          </div>

          <div class="modal-body">
            <div class="form-group">
              <label>{{ 'Genre Name' | translate }}</label>
              <input type="text" [(ngModel)]="formName" [placeholder]="'Enter genre name' | translate" class="form-input" />
            </div>

            <div class="form-group">
              <label>{{ 'Parent Genre (Optional)' | translate }}</label>
              <select [(ngModel)]="formParentId" class="form-select">
                <option value="">{{ 'None (Top Level)' | translate }}</option>
                <option *ngFor="let g of parentCandidates" [value]="g.id">{{ g.name }}</option>
              </select>
            </div>

            <div class="modal-actions">
              <button class="btn-save" (click)="saveGenre()" [disabled]="!formName || saving">
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
      max-width: 1000px;
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

    /* Modal */
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
      width: 440px;
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

    .form-input, .form-select {
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
export class GenreListComponent implements OnInit {
  private readonly language = inject(LanguageService);
  genres: Genre[] = [];
  loading: boolean = true;
  showModal: boolean = false;
  editingGenre: Genre | null = null;
  formName: string = '';
  formParentId: string = '';
  saving: boolean = false;

  constructor(private genreService: GenreService) {}

  ngOnInit(): void {
    this.loadGenres();
  }

  loadGenres(): void {
    this.loading = true;
    this.genreService.getGenres().subscribe({
      next: (data) => {
        this.genres = data || [];
        this.loading = false;
      },
      error: () => {
        this.genres = [];
        this.loading = false;
      }
    });
  }

  getParentName(parentId?: string | null): string {
    if (!parentId) return '—';
    const parent = this.genres.find(g => g.id === parentId);
    return parent ? parent.name : '—';
  }

  get parentCandidates(): Genre[] {
    if (!this.editingGenre) return this.genres;
    return this.genres.filter(g => g.id !== this.editingGenre?.id);
  }

  openAddModal(): void {
    this.editingGenre = null;
    this.formName = '';
    this.formParentId = '';
    this.showModal = true;
  }

  openEditModal(genre: Genre): void {
    this.editingGenre = genre;
    this.formName = genre.name;
    this.formParentId = genre.parentGenreId || '';
    this.showModal = true;
  }

  saveGenre(): void {
    if (!this.formName) return;
    this.saving = true;

    if (this.editingGenre) {
      this.genreService.updateGenre({
        genre: {
          id: this.editingGenre.id,
          name: this.formName,
          parentGenreId: this.formParentId || undefined
        }
      }).subscribe({
        next: () => {
          this.saving = false;
          this.showModal = false;
          this.loadGenres();
        },
        error: () => {
          this.saving = false;
          alert(this.language.translate('Could not save changes. Please try again.'));
        }
      });
    } else {
      this.genreService.createGenre({
        genre: {
          name: this.formName,
          parentGenreId: this.formParentId || undefined
        }
      }).subscribe({
        next: () => {
          this.saving = false;
          this.showModal = false;
          this.loadGenres();
        },
        error: () => {
          this.saving = false;
          alert(this.language.translate('Could not save changes. Please try again.'));
        }
      });
    }
  }

  deleteGenre(genre: Genre): void {
    if (confirm(this.language.translate('Delete this item?'))) {
      this.genreService.deleteGenre(genre.id).subscribe({
        next: () => this.loadGenres(),
        error: () => alert(this.language.translate('Could not delete this item.'))
      });
    }
  }
}
