import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { appSettings } from '../../app.settings';

@Injectable({ providedIn: 'root' })
export class AnalyticsService {
  constructor(private http: HttpClient) {}

  getTotals() {
    return this.http.get<any>(`${appSettings.apiBaseUrl}/analytics/totals`);
  }

  getTopItems() {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/analytics/popular-items`);
  }

  getUserCount() {
    return this.http.get<any>(`${appSettings.apiBaseUrl}/users/count`);
  }

  getOrdersPerWeek() {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/analytics/orders-per-week`);
  }

  getOrderTypeRatio() {
    return this.http.get<any>(`${appSettings.apiBaseUrl}/analytics/order-type-ratio`);
  }
}
