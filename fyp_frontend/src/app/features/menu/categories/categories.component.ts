import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CategoryService } from '../../../core/services/category.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.scss']
})
export class CategoriesComponent implements OnInit {
  categories: any[] = [];
  form: FormGroup;
  editingId: number | null = null;

  constructor(private categoryService: CategoryService, private fb: FormBuilder) {
    this.form = this.fb.group({
      name: '',
      description: '',
      imageUrl: '',
      displayOrder: 0,
      isActive: true
    });
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService.getAllCategories().subscribe(res => this.categories = res);
  }

  submit() {
    if (this.editingId) {
      this.categoryService.updateCategory(this.editingId, this.form.value).subscribe(() => {
        this.loadCategories();
        this.editingId = null;
        this.form.reset();
      });
    } else {
      this.categoryService.createCategory(this.form.value).subscribe(() => {
        this.loadCategories();
        this.form.reset();
      });
    }
  }

  edit(cat: any) {
    this.editingId = cat.categoryId;
    this.form.patchValue(cat);
  }

  delete(id: number) {
    this.categoryService.deleteCategory(id).subscribe(() => this.loadCategories());
  }

  cancel() {
    this.editingId = null;
    this.form.reset();
  }
  searchQuery: string = '';
  filteredCategories(): any[] {
  const q = this.searchQuery.toLowerCase();
  return this.categories.filter(c =>
    c.name.toLowerCase().includes(q) ||
    c.description?.toLowerCase().includes(q)
  );
}
}
