import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { appSettings } from '../../app.settings';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  constructor(private http: HttpClient) {}

  getAllCategories() {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/categories`);
  }

  createCategory(category: any) {
    return this.http.post(`${appSettings.apiBaseUrl}/categories`, category);
  }

  updateCategory(id: number, category: any) {
    return this.http.put(`${appSettings.apiBaseUrl}/categories/${id}`, category);
  }

  deleteCategory(id: number) {
    return this.http.delete(`${appSettings.apiBaseUrl}/categories/${id}`);
  }
}
