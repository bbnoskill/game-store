import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-publisher-details',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="page-container">
      <a routerLink="/publishers" class="back-link">&lt; Back to Publishers</a>
      <h2>Publisher Details</h2>
    </div>
  `,
  styles: [`
    .page-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 32px 24px;
    }
    .back-link {
      color: #f57c00;
      text-decoration: none;
    }
  `]
})
export class PublisherDetailsComponent {}
