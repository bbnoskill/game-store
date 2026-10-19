import { TranslatePipe } from '../../shared/pipes/translate.pipe';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [TranslatePipe, CommonModule, RouterModule, FormsModule],
  template: `
    <div class="page-container">
      <div class="login-card">
        <h2 class="login-title">{{ 'Sign In to GameStore' | translate }}</h2>

        <div class="alert alert-error" *ngIf="error">
          {{ error | translate }}
        </div>

        <form (ngSubmit)="onLogin()" #loginForm="ngForm">
          <div class="form-group">
            <label for="username">{{ 'Username or Email' | translate }}</label>
            <input
              type="text"
              id="username"
              name="username"
              [(ngModel)]="username"
              required
              class="form-input"
              [placeholder]="'Enter your username' | translate"
            />
          </div>

          <div class="form-group">
            <label for="password">{{ 'Password' | translate }}</label>
            <input
              type="password"
              id="password"
              name="password"
              [(ngModel)]="password"
              required
              class="form-input"
              [placeholder]="'Enter your password' | translate"
            />
          </div>

          <button type="submit" class="btn-submit" [disabled]="!loginForm.form.valid || loading">
            {{ (loading ? 'Signing In...' : 'Sign In') | translate }}
          </button>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .page-container {
      max-width: 440px;
      margin: 64px auto;
      padding: 0 24px;
      font-family: 'Roboto', sans-serif;
    }

    .login-card {
      background: #fff;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      padding: 32px;
      box-shadow: 0 4px 16px rgba(0,0,0,0.06);
    }

    .login-title {
      font-size: 24px;
      font-weight: 700;
      color: #222;
      margin: 0 0 24px;
      text-align: center;
    }

    .alert {
      padding: 12px 16px;
      border-radius: 6px;
      margin-bottom: 20px;
      font-size: 14px;
    }

    .alert-error {
      background: #ffebee;
      color: #c62828;
      border: 1px solid #ffcdd2;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 6px;
      margin-bottom: 20px;
    }

    .form-group label {
      font-size: 14px;
      font-weight: 500;
      color: #444;
    }

    .form-input {
      padding: 10px 14px;
      border: 1px solid #ccc;
      border-radius: 6px;
      font-size: 15px;
      transition: border-color 0.2s;
    }

    .form-input:focus {
      outline: none;
      border-color: #f57c00;
    }

    .btn-submit {
      width: 100%;
      background: #f57c00;
      color: #fff;
      border: none;
      padding: 12px;
      border-radius: 6px;
      font-size: 16px;
      font-weight: 600;
      cursor: pointer;
      transition: background 0.2s;
    }

    .btn-submit:hover:not(:disabled) {
      background: #e65100;
    }

    .btn-submit:disabled {
      background: #ccc;
      cursor: not-allowed;
    }
  `]
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  loading: boolean = false;
  error: string = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onLogin(): void {
    if (!this.username || !this.password) return;

    this.loading = true;
    this.error = '';

    this.authService.login({ model: { login: this.username, password: this.password } }).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/games']);
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.error?.message || 'Invalid username or password';
      }
    });
  }
}
