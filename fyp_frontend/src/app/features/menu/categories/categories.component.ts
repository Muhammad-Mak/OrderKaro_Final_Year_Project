// categories.component.ts
// This component is used for managing menu categories.
// It allows listing, filtering, creating, editing, and deleting categories using a reactive form.
// The component interacts with CategoryService to perform backend operations.

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CategoryService } from '../../../core/services/category.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-categories', // Used in template as <app-categories>
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.scss']
})
export class CategoriesComponent implements OnInit {
  categories: any[] = [];           // Holds all fetched categories
  form: FormGroup;                  // Reactive form for adding/editing a category
  editingId: number | null = null;  // Tracks the currently edited category (if any)

  constructor(private categoryService: CategoryService, private fb: FormBuilder) {
    // Initialize the form with default values
    this.form = this.fb.group({
      name: '',                     // Category name
      description: '',              // Category description (optional)
      imageUrl: '',                 // Optional image URL
      displayOrder: "Display Order", // Default display order (consider using number + validation)
      isActive: true                // Boolean toggle for active status
    });
  }

  ngOnInit(): void {
    this.loadCategories(); // Fetch categories on component initialization
  }

  // Fetch all categories from the server
  loadCategories() {
    this.categoryService.getAllCategories().subscribe(res => this.categories = res);
  }

  // Submit form handler — adds or updates a category based on `editingId`
  submit() {
    if (this.editingId) {
      // Update existing category
      this.categoryService.updateCategory(this.editingId, this.form.value).subscribe(() => {
        this.loadCategories();     // Refresh list
        this.editingId = null;     // Exit edit mode
        this.form.reset();         // Clear form
      });
    } else {
      // Create new category
      this.categoryService.createCategory(this.form.value).subscribe(() => {
        this.loadCategories();     // Refresh list
        this.form.reset();         // Clear form
      });
    }
  }

  // Populate form with data for the category being edited
  edit(cat: any) {
    this.editingId = cat.categoryId;
    this.form.patchValue(cat);
  }

  // Delete a category by ID
  delete(id: number) {
    this.categoryService.deleteCategory(id).subscribe(() => this.loadCategories());
  }

  // Cancel edit operation and reset form
  cancel() {
    this.editingId = null;
    this.form.reset();
  }

  searchQuery: string = ''; // Search query bound to the search input field

  // Returns a filtered list of categories matching the search query
  filteredCategories(): any[] {
    const q = this.searchQuery.toLowerCase();
    return this.categories.filter(c =>
      c.name.toLowerCase().includes(q) ||
      c.description?.toLowerCase().includes(q) // Optional chaining in case description is null
    );
  }
}
