// items.component.ts
// This component allows an admin or staff user to manage menu items.
// It supports viewing, filtering, creating, updating, and deleting items in the menu,
// and connects to both MenuService and CategoryService for backend communication.

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MenuService } from '../../../core/services/menu.service';
import { CategoryService } from '../../../core/services/category.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-items',                                     // Used in HTML as <app-items>
  standalone: true,                                          // Declared as a standalone Angular component
  imports: [CommonModule, ReactiveFormsModule, FormsModule], // Enables forms and common directives
  templateUrl: './items.component.html',
  styleUrls: ['./items.component.scss']
})
export class ItemsComponent implements OnInit {
  items: any[] = [];              // Stores list of menu items
  categories: any[] = [];         // Stores list of categories

  form: FormGroup;                // Reactive form for item data
  editingId: number | null = null; // Tracks which item is being edited

  constructor(
    private menuService: MenuService,
    private categoryService: CategoryService,
    private fb: FormBuilder
  ) {
    // Initialize form with default values
    this.form = this.fb.group({
      name: '',
      description: '',
      price: "Price",             // Default placeholder — consider changing to a number with validation
      imageUrl: '',
      isAvailable: true,
      categoryId: null
    });
  }

  ngOnInit(): void {
    this.loadItems(); // Load all items when component mounts

    // Fetch all categories (used in dropdown/select in form)
    this.categoryService.getAllCategories().subscribe(res => this.categories = res);
  }

  // Fetch all items from the server
  loadItems() {
    this.menuService.getAllItems().subscribe(res => this.items = res);
  }

  // Submit form handler — handles both add and update
  submit() {
    if (this.editingId) {
      // If editing, call update endpoint
      this.menuService.updateItem(this.editingId, this.form.value).subscribe(() => {
        this.loadItems();
        this.editingId = null;
        this.form.reset(); // Clear form after update
      });
    } else {
      // If creating a new item
      this.menuService.createItem(this.form.value).subscribe(() => {
        this.loadItems();
        this.form.reset(); // Clear form after add
      });
    }
  }

  // Populate form with existing item data for editing
  edit(item: any) {
    this.editingId = item.menuItemId;
    this.form.patchValue(item);
  }

  // Delete a menu item by ID
  delete(id: number) {
    this.menuService.deleteItem(id).subscribe(() => this.loadItems());
  }

  // Cancel editing and reset form
  cancel() {
    this.editingId = null;
    this.form.reset();
  }

  searchQuery: string = ''; // Bound to search input field

  // Filters menu items based on name, description, or category
  filteredItems(): any[] {
    const q = this.searchQuery.toLowerCase();
    return this.items.filter(i =>
      i.name.toLowerCase().includes(q) ||
      i.description.toLowerCase().includes(q) ||
      i.categoryName.toLowerCase().includes(q)
    );
  }
}
