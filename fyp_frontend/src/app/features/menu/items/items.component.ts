import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MenuService } from '../../../core/services/menu.service';
import { CategoryService } from '../../../core/services/category.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-items',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './items.component.html',
  styleUrls: ['./items.component.scss']
})
export class ItemsComponent implements OnInit {
  items: any[] = [];
  categories: any[] = [];

  form: FormGroup;
  editingId: number | null = null;

  constructor(
    private menuService: MenuService,
    private categoryService: CategoryService,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      name: '',
      description: '',
      price: "Price",
      imageUrl: '',
      isAvailable: true,
      categoryId: null
    });
  }

  ngOnInit(): void {
    this.loadItems();
    this.categoryService.getAllCategories().subscribe(res => this.categories = res);
  }

  loadItems() {
    this.menuService.getAllItems().subscribe(res => this.items = res);
  }

  submit() {
    if (this.editingId) {
      this.menuService.updateItem(this.editingId, this.form.value).subscribe(() => {
        this.loadItems();
        this.editingId = null;
        this.form.reset();
      });
    } else {
      this.menuService.createItem(this.form.value).subscribe(() => {
        this.loadItems();
        this.form.reset();
      });
    }
  }

  edit(item: any) {
    this.editingId = item.menuItemId;
    this.form.patchValue(item);
  }

  delete(id: number) {
    this.menuService.deleteItem(id).subscribe(() => this.loadItems());
  }

  cancel() {
    this.editingId = null;
    this.form.reset();
  }
  searchQuery: string = '';

filteredItems(): any[] {
  const q = this.searchQuery.toLowerCase();
  return this.items.filter(i =>
    i.name.toLowerCase().includes(q) ||
    i.description.toLowerCase().includes(q) ||
    i.categoryName.toLowerCase().includes(q)
  );
}
}
