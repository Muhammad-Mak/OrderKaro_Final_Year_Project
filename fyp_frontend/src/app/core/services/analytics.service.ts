// analytics.service.ts
// This service provides access to various analytics endpoints related to users, orders, and menu items.
// It is primarily used by the Admin Dashboard to visualize trends, totals, and statistics.

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { appSettings } from '../../app.settings'; // Base API URL configuration

@Injectable({ providedIn: 'root' }) // Makes this service available app-wide
export class AnalyticsService {
  constructor(private http: HttpClient) {}

  // GET: Fetch total orders and total revenue
  getTotals() {
    return this.http.get<any>(`${appSettings.apiBaseUrl}/analytics/totals`);
  }

  // GET: Fetch top 6 most frequently ordered menu items
  getTopItems() {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/analytics/popular-items`);
  }

  // GET: Fetch total number of registered users
  getUserCount() {
    return this.http.get<any>(`${appSettings.apiBaseUrl}/users/count`);
  }

  // GET: Fetch number of orders placed each day of the current week
  getOrdersPerWeek() {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/analytics/orders-per-week`);
  }

  // GET: Fetch ratio of Pickup vs Delivery orders
  getOrderTypeRatio() {
    return this.http.get<any>(`${appSettings.apiBaseUrl}/analytics/order-type-ratio`);
  }
}
