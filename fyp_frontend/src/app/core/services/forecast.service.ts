import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { appSettings } from '../../app.settings';

@Injectable({ providedIn: 'root' })
export class ForecastService {
  constructor(private http: HttpClient) {}

  getForecast(itemId: number): Observable<{ date: string; quantity: number }[]> {
    return this.http.get<{ date: string; quantity: number }[]>(`${appSettings.apiBaseUrl}/forecast/sales?itemId=${itemId}`);
  }
}
