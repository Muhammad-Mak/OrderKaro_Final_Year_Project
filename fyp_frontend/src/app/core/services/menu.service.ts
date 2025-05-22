import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { appSettings } from '../../app.settings';

@Injectable({ providedIn: 'root' })
export class MenuService {
  constructor(private http: HttpClient) {}

  getAllItems() {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/menuitems`);
  }

  createItem(item: any) {
    return this.http.post(`${appSettings.apiBaseUrl}/menuitems`, item);
  }

  updateItem(id: number, item: any) {
    return this.http.put(`${appSettings.apiBaseUrl}/menuitems/${id}`, item);
  }

  deleteItem(id: number) {
    return this.http.delete(`${appSettings.apiBaseUrl}/menuitems/${id}`);
  }
}
