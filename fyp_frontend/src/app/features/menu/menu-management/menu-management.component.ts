import { Component } from '@angular/core';
import { ItemsComponent } from '../items/items.component';
import { CategoriesComponent } from '../categories/categories.component';

@Component({
  standalone: true,
  selector: 'app-menu-management',
  imports: [ItemsComponent, CategoriesComponent],
  template: `
    <h2>Menu Management</h2>
    <app-items></app-items>
    <hr />
    <app-categories></app-categories>
  `
})
export class MenuManagementComponent {}
