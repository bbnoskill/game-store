import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { ConfigService } from './config.service';
import { LoginRequest, LoginResponse } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'gamestore_auth_token';

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  login(credentials: LoginRequest): Observable<LoginResponse> {
    const url = this.configService.getApiUrl('loginApiUrl');
    return this.http.post<LoginResponse>(url, credentials).pipe(
      tap((res) => {
        if (res?.token) {
          this.setToken(res.token);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  checkAccess(target: string): Observable<boolean> {
    const url = this.configService.getApiUrl('checkAccessApiUrl');
    return this.http.post<boolean>(url, { target });
  }
}
