// auth.service.ts
// This service handles authentication operations: login, registration,
// token storage, role checks, and logout logic for the application.

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { appSettings } from '../../app.settings'; // Global API config

@Injectable({
  providedIn: 'root' // Available globally throughout the app
})
export class AuthService {
  // Keys used to store token and user info in localStorage
  private readonly TOKEN_KEY = 'token';
  private readonly USER_KEY = 'user';

  constructor(private http: HttpClient, private router: Router) {}

  // POST: Send credentials to login endpoint
  login(credentials: { email: string; password: string }) {
    return this.http.post<any>(`${appSettings.apiBaseUrl}/auth/login`, credentials);
  }

  // POST: Send registration data to register endpoint
  register(data: any) {
    return this.http.post<any>(`${appSettings.apiBaseUrl}/auth/register`, data);
  }

  // Save token and user object in localStorage after successful login
  saveAuthData(token: string, user: any) {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  // Clear auth data and redirect to login
  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.router.navigate(['/login']);
  }

  // Retrieve token from localStorage
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  // Retrieve and parse user object from localStorage
  getUser(): any | null {
    const user = localStorage.getItem(this.USER_KEY);
    return user ? JSON.parse(user) : null;
  }

  // Get current user's role (e.g., Admin, Staff, Customer)
  getUserRole(): string {
    return this.getUser()?.role ?? '';
  }

  // Determine if a user is currently logged in based on token presence
  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
