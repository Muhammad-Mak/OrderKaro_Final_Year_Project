// category.service.ts
// This service manages CRUD operations for menu categories.
// It communicates with the backend API to fetch, add, update, and delete categories.

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { appSettings } from '../../app.settings'; // Contains API base URL

@Injectable({ providedIn: 'root' }) // Auto-registers the service app-wide
export class CategoryService {
  constructor(private http: HttpClient) {}

  // GET: Retrieve all categories (for dropdowns, category tables, etc.)
  getAllCategories() {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/categories`);
  }

  // POST: Create a new category
  createCategory(category: any) {
    return this.http.post(`${appSettings.apiBaseUrl}/categories`, category);
  }

  // PUT: Update an existing category by ID
  updateCategory(id: number, category: any) {
    return this.http.put(`${appSettings.apiBaseUrl}/categories/${id}`, category);
  }

  // DELETE: Remove a category by ID
  deleteCategory(id: number) {
    return this.http.delete(`${appSettings.apiBaseUrl}/categories/${id}`);
  }
}
