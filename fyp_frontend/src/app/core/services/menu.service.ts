// menu.service.ts
// This service provides CRUD operations for managing menu items.
// It communicates with the backend API to fetch, create, update, and delete menu entries.

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { appSettings } from '../../app.settings'; // Contains the base API URL

@Injectable({ providedIn: 'root' }) // Service is available throughout the app
export class MenuService {
  constructor(private http: HttpClient) {}

  // GET: Retrieve all menu items from the server
  getAllItems() {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/menuitems`);
  }

  // POST: Create a new menu item
  createItem(item: any) {
    return this.http.post(`${appSettings.apiBaseUrl}/menuitems`, item);
  }

  // PUT: Update an existing menu item by its ID
  updateItem(id: number, item: any) {
    return this.http.put(`${appSettings.apiBaseUrl}/menuitems/${id}`, item);
  }

  // DELETE: Delete a menu item by its ID
  deleteItem(id: number) {
    return this.http.delete(`${appSettings.apiBaseUrl}/menuitems/${id}`);
  }
}
