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
}
