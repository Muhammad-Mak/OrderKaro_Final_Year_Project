import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { appSettings } from '../../app.settings';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'token';
  private readonly USER_KEY = 'user';

  constructor(private http: HttpClient, private router: Router) {}

  login(credentials: { email: string; password: string }) {
    return this.http.post<any>(`${appSettings.apiBaseUrl}/auth/login`, credentials);
  }

  register(data: any) {
    return this.http.post<any>(`${appSettings.apiBaseUrl}/auth/register`, data);
  }

  saveAuthData(token: string, user: any) {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getUser(): any | null {
    const user = localStorage.getItem(this.USER_KEY);
    return user ? JSON.parse(user) : null;
  }

  getUserRole(): string {
    return this.getUser()?.role ?? '';
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}

// This service handles authentication-related tasks such as login, registration, and token management.
// It also provides methods to check if the user is logged in and to get the user's role.