// user.service.ts
// This service handles all HTTP requests related to user management.
// It includes operations for fetching all users, deleting users,
// updating user details, and topping up a user's RFID balance.

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { appSettings } from '../../app.settings'; // Base API URL configuration

@Injectable({ providedIn: 'root' }) // Makes the service available app-wide
export class UserService {
  constructor(private http: HttpClient) {}

  // GET: Fetch all users (Admin only)
  getAllUsers() {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/users`);
  }

  // DELETE: Remove a user by ID (Admin only)
  deleteUser(userId: number) {
    return this.http.delete(`${appSettings.apiBaseUrl}/users/${userId}`);
  }

  // POST: Top-up a user's RFID balance
  topUpBalance(studentId: string, amount: number) {
    return this.http.post<{ balance: number }>(
      `${appSettings.apiBaseUrl}/users/topup?studentId=${studentId}&amount=${amount}`,
      {} // Empty body, data is passed as query parameters
    );
  }

  // PUT: Update a user's profile (Admin only)
  updateUser(userId: number, data: any) {
    return this.http.put(`${appSettings.apiBaseUrl}/users/${userId}`, data);
  }
}
