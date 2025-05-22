import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { appSettings } from '../../app.settings';

@Injectable({ providedIn: 'root' })
export class OrderService {
  constructor(private http: HttpClient) {}

  getOrdersByUser(userId: number) {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/orders/user/${userId}`);
  }

  getAllOrders() {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/orders`);
  }
}
