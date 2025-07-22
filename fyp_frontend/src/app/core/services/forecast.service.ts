// forecast.service.ts
// This service provides access to sales forecast data for a specific menu item.
// It makes a GET request to the backend API which returns an array of predicted daily quantities.

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { appSettings } from '../../app.settings'; // Contains API base URL

@Injectable({ providedIn: 'root' }) // Makes the service available app-wide without manual registration
export class ForecastService {
  constructor(private http: HttpClient) {}

  // GET: Retrieve 7-day sales forecast for a specific menu item
  // Returns an array of { date, quantity } pairs
  getForecast(itemId: number): Observable<{ date: string; quantity: number }[]> {
    return this.http.get<{ date: string; quantity: number }[]>(
      `${appSettings.apiBaseUrl}/forecast/sales?itemId=${itemId}`
    );
  }
}
