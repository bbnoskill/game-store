import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="login-container">
      <div class="login-card">
        <h2 class="login-title">Sign In</h2>
        <form (ngSubmit)="onLogin()">
          <div class="form-group">
            <label for="username">Username</label>
            <input type="text" id="username" name="username" [(ngModel)]="login" required />
          </div>
          <div class="form-group">
            <label for="password">Password</label>
            <input type="password" id="password" name="password" [(ngModel)]="password" required />
          </div>
          <button type="submit" class="btn-orange">Log In</button>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      max-width: 420px;
      margin: 48px auto;
      padding: 0 24px;
    }
    .login-card {
      background: #ffffff;
      padding: 32px;
      border-radius: 4px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }
    .login-title {
      font-size: 24px;
      font-weight: 700;
      margin-bottom: 24px;
      color: #222;
    }
    .form-group {
      margin-bottom: 16px;
      display: flex;
      flex-direction: column;
      gap: 6px;
    }
    .form-group label {
      font-size: 13px;
      color: #555;
    }
    .form-group input {
      padding: 10px;
      border: 1px solid #ccc;
      border-radius: 4px;
      font-size: 14px;
    }
    .btn-orange {
      width: 100%;
      background-color: #f57c00;
      color: #ffffff;
      border: none;
      padding: 12px;
      font-size: 14px;
      font-weight: 600;
      border-radius: 4px;
      cursor: pointer;
      margin-top: 8px;
    }
    .btn-orange:hover {
      background-color: #e65100;
    }
  `]
})
export class LoginComponent {
  login = '';
  password = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onLogin(): void {
    if (!this.login) return;
    this.authService.login({
      model: {
        login: this.login,
        password: this.password
      }
    }).subscribe({
      next: () => {
        this.router.navigate(['/games']);
      },
      error: (err) => {
        alert('Login failed: ' + (err?.error?.message || 'Invalid credentials'));
      }
    });
  }
}
