// order.service.ts
// This service handles all HTTP requests related to orders.
// It supports fetching orders by user, retrieving all/active orders, and marking orders as completed.

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { appSettings } from '../../app.settings'; // Base API URL configuration

@Injectable({ providedIn: 'root' }) // Makes the service globally injectable
export class OrderService {
  constructor(private http: HttpClient) {}

  // GET: Fetch all orders placed by a specific user
  getOrdersByUser(userId: number) {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/orders/user/${userId}`);
  }

  // GET: Fetch all orders (Admin or Staff access)
  getAllOrders() {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/orders`);
  }

  // GET: Fetch only active (incomplete or in-progress) orders
  getActiveOrders() {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/orders/active`);
  }

  // PUT: Mark a specific order as completed
  markAsCompleted(orderId: number) {
    return this.http.put(`${appSettings.apiBaseUrl}/orders/${orderId}/complete`, {});
  }

  // PUT: Mark a specific order as prepared
  markAsPrepared(orderId: number) {
    return this.http.put(`${appSettings.apiBaseUrl}/orders/${orderId}/prepared`, {});
  }
}
