// active-orders.component.ts
// This component is responsible for displaying and managing active orders in real-time.
// It allows staff/admin to mark orders as "prepared", "completed", search through orders,
// and expand order details dynamically.

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../../core/services/order.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-active-orders',                              // Component selector tag
  standalone: true,                                           // Declared as a standalone component
  imports: [CommonModule, MatSnackBarModule, FormsModule],    // Required Angular/Common modules
  templateUrl: './active-orders.component.html',
  styleUrls: ['./active-orders.component.scss']
})
export class ActiveOrdersComponent implements OnInit {
  orders: any[] = [];                       // Stores all active orders fetched from the backend
  preparedMap: Record<number, boolean> = {}; // Tracks which orders are toggled as 'Prepared' (UI only)

  constructor(private orderService: OrderService, private snackBar: MatSnackBar) {}

  // Lifecycle hook: called once after component is initialized
  ngOnInit(): void {
    this.loadActiveOrders();
  }

  // Fetches the active orders from the backend
  loadActiveOrders() {
    this.orderService.getActiveOrders().subscribe(res => {
      this.orders = res;
    });
  }

  // Toggles "Prepared" status in UI (not persisted in backend)
  togglePrepared(orderId: number) {
    this.preparedMap[orderId] = !this.preparedMap[orderId];
  }

  // Marks an order as completed (persists to backend and updates UI)
  markCompleted(order: any) {
    this.orderService.markAsCompleted(order.orderId).subscribe(() => {
      // Remove the completed order from the active list
      this.orders = this.orders.filter(o => o.orderId !== order.orderId);
      // Show a success toast/snackbar
      this.snackBar.open('Order marked as completed ✅', 'Close', {
        duration: 3000,
        panelClass: 'snack-success'
      });
    });
  }

  searchOrderQuery: string = ''; // Two-way bound to search input field

  // Filters orders based on the search query (by order number, customer name, or status)
  filteredOrders(): any[] {
    const query = this.searchOrderQuery.toLowerCase();
    return this.orders.filter(order =>
      order.orderNumber.toLowerCase().includes(query) ||
      `${order.firstName ?? ''} ${order.lastName ?? ''}`.toLowerCase().includes(query) ||
      order.status.toLowerCase().includes(query)
    );
  }

  expandedOrderIds: Set<number> = new Set(); // Tracks expanded/collapsed state of order details

  // Toggles the expanded/collapsed state for a specific order
  toggleExpand(orderId: number) {
    if (this.expandedOrderIds.has(orderId)) {
      this.expandedOrderIds.delete(orderId);
    } else {
      this.expandedOrderIds.add(orderId);
    }
  }
}
